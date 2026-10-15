using LibraryManagementAPI.Application.DTOs.Fine;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository _fineRepository;
        private readonly ILoanRepository _loanRepository;

        public FineService(
            IFineRepository fineRepository,
            ILoanRepository loanRepository)
        {
            _fineRepository = fineRepository;
            _loanRepository = loanRepository;
        }

        public List<FineResponseDto> GetAll()
        {
            return _fineRepository.GetAll()
                .Select(x => new FineResponseDto
                {
                    Id = x.Id,
                    LoanId = x.LoanId,
                    Amount = x.Amount,
                    FineDate = x.FineDate,
                    IsPaid = x.IsPaid
                })
                .ToList();
        }

        public FineResponseDto? GetById(Guid id)
        {
            var fine = _fineRepository.GetById(id);

            if (fine == null)
                return null;

            return new FineResponseDto
            {
                Id = fine.Id,
                LoanId = fine.LoanId,
                Amount = fine.Amount,
                FineDate = fine.FineDate,
                IsPaid = fine.IsPaid
            };
        }

        public List<FineResponseDto> GetByLoanId(Guid loanId)
        {
            return _fineRepository.GetByLoanId(loanId)
                .Select(x => new FineResponseDto
                {
                    Id = x.Id,
                    LoanId = x.LoanId,
                    Amount = x.Amount,
                    FineDate = x.FineDate,
                    IsPaid = x.IsPaid
                })
                .ToList();
        }

        public void Add(FineCreateDto dto)
        {
            CreateFineForLoan(dto.LoanId);
        }

        public void CreateFineForLoan(Guid loanId)
        {
            var loan = _loanRepository.GetById(loanId);

            if (loan == null)
                throw new KeyNotFoundException("Loan not found.");

            var existingFine = _fineRepository
                .GetByLoanId(loanId)
                .FirstOrDefault();

            if (existingFine != null)
                throw new InvalidOperationException(
                    "A fine has already been created for this loan.");

            if (!loan.ReturnDate.HasValue)
                throw new InvalidOperationException(
                    "Fine can only be created after the book is returned.");

            if (loan.ReturnDate.Value <= loan.DueDate)
                throw new InvalidOperationException(
                    "No fine is required because the book was returned on time.");

            var lateDays =
                (loan.ReturnDate.Value.Date - loan.DueDate.Date).Days;

            var fineAmount = lateDays * 10;

            var fine = new Fine
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                Amount = fineAmount,
                FineDate = DateTime.Now,
                IsPaid = false,
                IsDeleted = false
            };

            _fineRepository.Add(fine);
        }

        public void Update(Guid id, FineUpdateDto dto)
        {
            var fine = _fineRepository.GetById(id);

            if (fine == null)
                throw new KeyNotFoundException("Fine not found.");

            fine.Amount = dto.Amount;
            fine.IsPaid = dto.IsPaid;

            _fineRepository.Update(fine);
        }

        public void Delete(Guid id)
        {
            var fine = _fineRepository.GetById(id);

            if (fine == null)
                throw new KeyNotFoundException("Fine not found.");

            _fineRepository.Delete(fine);
        }
    }
}