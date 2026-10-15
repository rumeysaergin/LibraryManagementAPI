
using LibraryManagementAPI.Application.DTOs.Loan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILibraryBookRepository _libraryBookRepository;
        private readonly IUserMembershipRepository _userMembershipRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;
        private readonly IMembershipPaymentRepository _membershipPaymentRepository;
        private readonly IUserLibraryRepository _userLibraryRepository;
        private readonly IFineService _fineService;

        public LoanService(
            ILoanRepository loanRepository,
            ILibraryBookRepository libraryBookRepository,
            IUserMembershipRepository userMembershipRepository,
            IMembershipPlanRepository membershipPlanRepository,
            IMembershipPaymentRepository membershipPaymentRepository,
            IUserLibraryRepository userLibraryRepository,
            IFineService fineService)
        {
            _loanRepository = loanRepository;
            _libraryBookRepository = libraryBookRepository;
            _userMembershipRepository = userMembershipRepository;
            _membershipPlanRepository = membershipPlanRepository;
            _membershipPaymentRepository = membershipPaymentRepository;
            _userLibraryRepository = userLibraryRepository;
            _fineService = fineService;
        }

        public List<LoanResponseDto> GetAll()
        {
            return _loanRepository.GetAll()
                .Select(x => new LoanResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    LibraryBookId = x.LibraryBookId,
                    BorrowDate = x.BorrowDate,
                    DueDate = x.DueDate,
                    ReturnDate = x.ReturnDate,
                    Status = x.Status
                })
                .ToList();
        }

        public LoanResponseDto? GetById(Guid id)
        {
            var loan = _loanRepository.GetById(id);

            if (loan == null)
                return null;

            return new LoanResponseDto
            {
                Id = loan.Id,
                UserId = loan.UserId,
                LibraryBookId = loan.LibraryBookId,
                BorrowDate = loan.BorrowDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status
            };
        }

        public List<LoanResponseDto> GetByUserId(Guid userId)
        {
            return _loanRepository.GetByUserId(userId)
                .Select(x => new LoanResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    LibraryBookId = x.LibraryBookId,
                    BorrowDate = x.BorrowDate,
                    DueDate = x.DueDate,
                    ReturnDate = x.ReturnDate,
                    Status = x.Status
                })
                .ToList();
        }

        public List<LoanResponseDto> GetByLibraryBookId(Guid libraryBookId)
        {
            return _loanRepository.GetByLibraryBookId(libraryBookId)
                .Select(x => new LoanResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    LibraryBookId = x.LibraryBookId,
                    BorrowDate = x.BorrowDate,
                    DueDate = x.DueDate,
                    ReturnDate = x.ReturnDate,
                    Status = x.Status
                })
                .ToList();
        }

        public void Add(LoanCreateDto dto)
        {
            var now = DateTime.Now;

            // 1. Kullanıcının aktif ve geçerli üyeliklerini bul.
            var activeMemberships = _userMembershipRepository
                .GetByUserId(dto.UserId)
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.StartDate <= now &&
                    x.EndDate >= now)
                .ToList();

            if (!activeMemberships.Any())
            {
                throw new InvalidOperationException(
                    "User does not have an active membership.");
            }

            // 2. Aktif üyeliklerden en az birinin ödemesi tamamlanmış olmalı.
            var activeMembership = activeMemberships
                .FirstOrDefault(m =>
                    _membershipPaymentRepository
                        .GetByUserMembershipId(m.Id)
                        .Any(p => !p.IsDeleted && p.IsPaid));

            if (activeMembership == null)
            {
                throw new InvalidOperationException(
                    "Membership payment has not been completed. " +
                    "Please complete your membership payment before borrowing a book.");
            }

            // 3. Ödünç alınmak istenen kütüphane-kitap kaydını bul.
            var libraryBook = _libraryBookRepository
                .GetById(dto.LibraryBookId);

            if (libraryBook == null || libraryBook.IsDeleted)
            {
                throw new KeyNotFoundException(
                    "Library book record not found or is no longer available.");
            }

            // 4. Kullanıcı, kitabın bulunduğu kütüphaneye üye olmalı.
            var userLibraries = _userLibraryRepository
                .GetByUserId(dto.UserId)
                .Where(x => !x.IsDeleted)
                .ToList();

            var isMemberOfLibrary = userLibraries
                .Any(x => x.LibraryId == libraryBook.LibraryId);

            if (!isMemberOfLibrary)
            {
                throw new InvalidOperationException(
                    "You cannot borrow a book from a library you are not a member of.");
            }

            // 5. Kitabın stok durumunu kontrol et.
            if (libraryBook.Quantity <= 0)
            {
                throw new InvalidOperationException(
                    "Book is out of stock in this library.");
            }

            // 6. Ödeme yapılmış aktif üyeliğin planını bul.
            var membershipPlan = _membershipPlanRepository
                .GetById(activeMembership.MembershipPlanId);

            if (membershipPlan == null)
            {
                throw new KeyNotFoundException(
                    "Membership plan not found.");
            }

            if (membershipPlan.MaxBorrowLimit <= 0)
            {
                throw new InvalidOperationException(
                    "Membership plan does not allow borrowing.");
            }

            if (membershipPlan.DurationInDays <= 0)
            {
                throw new InvalidOperationException(
                    "Membership plan must have a valid loan duration.");
            }

            // 7. Kullanıcının hâlen iade etmediği kitapları say.
            var activeLoanCount = _loanRepository
                .GetByUserId(dto.UserId)
                .Count(x =>
                    !x.IsDeleted &&
                    !x.ReturnDate.HasValue &&
                    x.Status == "Borrowed");

            if (activeLoanCount >= membershipPlan.MaxBorrowLimit)
            {
                throw new InvalidOperationException(
                    $"You have reached your borrowing limit. " +
                    $"Your membership plan allows a maximum of " +
                    $"{membershipPlan.MaxBorrowLimit} active loans.");
            }

            // 8. Ödünç alma kaydını oluştur.
            var borrowDate = now;
            var dueDate = borrowDate.AddDays(
                membershipPlan.DurationInDays);

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                LibraryBookId = dto.LibraryBookId,
                BorrowDate = borrowDate,
                DueDate = dueDate,
                ReturnDate = null,
                Status = "Borrowed",
                IsDeleted = false
            };

            // 9. Kaydı oluştur ve stok miktarını güncelle.
            libraryBook.Quantity--;

            _loanRepository.Add(loan);
            _libraryBookRepository.Update(libraryBook);
        }

        public void ReturnLoan(Guid id)
        {
            var loan = _loanRepository.GetById(id);

            if (loan == null || loan.IsDeleted)
            {
                throw new KeyNotFoundException(
                    "Loan not found.");
            }

            if (loan.ReturnDate.HasValue || loan.Status == "Returned")
            {
                throw new InvalidOperationException(
                    "This loan has already been returned.");
            }

            if (loan.Status != "Borrowed")
            {
                throw new InvalidOperationException(
                    "Only active loans can be returned.");
            }

            var libraryBook = _libraryBookRepository
                .GetById(loan.LibraryBookId);

            if (libraryBook == null || libraryBook.IsDeleted)
            {
                throw new KeyNotFoundException(
                    "Library book record not found.");
            }

            var returnDate = DateTime.Now;

            loan.ReturnDate = returnDate;
            loan.Status = "Returned";

            libraryBook.Quantity++;

            _loanRepository.Update(loan);
            _libraryBookRepository.Update(libraryBook);

            // Gecikme varsa ceza oluştur.
            if (returnDate.Date > loan.DueDate.Date)
            {
                _fineService.CreateFineForLoan(loan.Id);
            }
        }

        public void Delete(Guid id)
        {
            var loan = _loanRepository.GetById(id);

            if (loan == null || loan.IsDeleted)
            {
                throw new KeyNotFoundException(
                    "Loan not found.");
            }

            if (!loan.ReturnDate.HasValue &&
                loan.Status == "Borrowed")
            {
                throw new InvalidOperationException(
                    "An active loan must be returned before it can be deleted.");
            }

            _loanRepository.Delete(loan);
        }
    }
}