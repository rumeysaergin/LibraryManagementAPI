using LibraryManagementAPI.Application.DTOs.Fine;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class FineService : IFineService
    {
        private readonly IFineRepository _fineRepository;

        public FineService(IFineRepository fineRepository)
        {
            _fineRepository = fineRepository;
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
            var fine = new Fine
            {
                Id = Guid.NewGuid(),
                LoanId = dto.LoanId,
                Amount = dto.Amount,
                FineDate = dto.FineDate,
                IsPaid = dto.IsPaid,
                IsDeleted = false
            };

            _fineRepository.Add(fine);
        }

        public void CreateFineForLoan(Guid loanId, decimal amount)
        {
            var fine = new Fine
            {
                Id = Guid.NewGuid(),
                LoanId = loanId,
                Amount = amount,
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