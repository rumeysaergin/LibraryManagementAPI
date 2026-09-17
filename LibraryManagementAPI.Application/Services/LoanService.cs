using LibraryManagementAPI.Application.DTOs.Loan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IFineService _fineService;

        public LoanService(
            ILoanRepository loanRepository,
            IFineService fineService)
        {
            _loanRepository = loanRepository;
            _fineService = fineService;
        }

        public List<LoanResponseDto> GetAll()
        {
            return _loanRepository.GetAll()
                .Select(x => new LoanResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    BookId = x.BookId,
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
                BookId = loan.BookId,
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
                    BookId = x.BookId,
                    BorrowDate = x.BorrowDate,
                    DueDate = x.DueDate,
                    ReturnDate = x.ReturnDate,
                    Status = x.Status
                })
                .ToList();
        }

        public List<LoanResponseDto> GetByBookId(Guid bookId)
        {
            return _loanRepository.GetByBookId(bookId)
                .Select(x => new LoanResponseDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    BookId = x.BookId,
                    BorrowDate = x.BorrowDate,
                    DueDate = x.DueDate,
                    ReturnDate = x.ReturnDate,
                    Status = x.Status
                })
                .ToList();
        }

        public void Add(LoanCreateDto dto)
        {
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                BookId = dto.BookId,
                BorrowDate = dto.BorrowDate,
                DueDate = dto.DueDate,
                Status = dto.Status,
                IsDeleted = false
            };

            _loanRepository.Add(loan);
        }

        public void Update(Guid id, LoanUpdateDto dto)
        {
            var loan = _loanRepository.GetById(id);

            if (loan == null)
                throw new KeyNotFoundException("Loan not found.");

            loan.DueDate = dto.DueDate;
            loan.ReturnDate = dto.ReturnDate;
            loan.Status = dto.Status;

            if (dto.ReturnDate.HasValue && dto.ReturnDate.Value > dto.DueDate)
            {
                var lateDays = (dto.ReturnDate.Value.Date - dto.DueDate.Date).Days;
                var fineAmount = lateDays * 10;

                _fineService.CreateFineForLoan(loan.Id, fineAmount);
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