using LibraryManagementAPI.Application.DTOs.Loan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILibraryBookRepository _libraryBookRepository;
        private readonly IFineService _fineService;

        public LoanService(
            ILoanRepository loanRepository,
            ILibraryBookRepository libraryBookRepository,
            IFineService fineService)
        {
            _loanRepository = loanRepository;
            _libraryBookRepository = libraryBookRepository;
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
            var libraryBook = _libraryBookRepository.GetById(dto.LibraryBookId);

            if (libraryBook == null)
                throw new KeyNotFoundException("Library book record not found.");

            if (libraryBook.Quantity <= 0)
                throw new InvalidOperationException(
                    "Book is out of stock in this library.");

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                LibraryBookId = dto.LibraryBookId,
                BorrowDate = dto.BorrowDate,
                DueDate = dto.DueDate,
                ReturnDate = null,
                Status = "Borrowed",
                IsDeleted = false
            };

            libraryBook.Quantity--;

            _loanRepository.Add(loan);
            _libraryBookRepository.Update(libraryBook);
        }

        public void Update(Guid id, LoanUpdateDto dto)
        {
            var loan = _loanRepository.GetById(id);

            if (loan == null)
                throw new KeyNotFoundException("Loan not found.");

            var isFirstReturn =
                !loan.ReturnDate.HasValue &&
                dto.ReturnDate.HasValue;

            loan.ReturnDate = dto.ReturnDate;
            loan.DueDate = dto.DueDate;
            loan.Status = dto.Status;

            if (isFirstReturn)
            {
                var libraryBook =
                    _libraryBookRepository.GetById(loan.LibraryBookId);

                if (libraryBook == null)
                    throw new KeyNotFoundException(
                        "Library book record not found.");

                libraryBook.Quantity++;

                _libraryBookRepository.Update(libraryBook);

                if (loan.ReturnDate.Value.Date > loan.DueDate.Date)
                {
                    _loanRepository.Update(loan);
                    _fineService.CreateFineForLoan(loan.Id);
                    return;
                }
            }

            _loanRepository.Update(loan);
        }

        public void Delete(Guid id)
        {
            var loan = _loanRepository.GetById(id);

            if (loan == null)
                throw new KeyNotFoundException("Loan not found.");

            _loanRepository.Delete(loan);
        }
    }
}