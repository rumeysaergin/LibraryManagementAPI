using LibraryManagementAPI.Application.DTOs.FinePayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class FinePaymentService : IFinePaymentService
    {
        private readonly IFinePaymentRepository _finePaymentRepository;

        public FinePaymentService(IFinePaymentRepository finePaymentRepository)
        {
            _finePaymentRepository = finePaymentRepository;
        }

        public List<FinePaymentResponseDto> GetAll()
        {
            return _finePaymentRepository.GetAll()
                .Select(x => new FinePaymentResponseDto
                {
                    Id = x.Id,
                    FineId = x.FineId,
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate,
                    PaymentMethod = x.PaymentMethod
                })
                .ToList();
        }

        public FinePaymentResponseDto? GetById(Guid id)
        {
            var payment = _finePaymentRepository.GetById(id);

            if (payment == null)
                return null;

            return new FinePaymentResponseDto
            {
                Id = payment.Id,
                FineId = payment.FineId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod
            };
        }

        public List<FinePaymentResponseDto> GetByFineId(Guid fineId)
        {
            return _finePaymentRepository.GetByFineId(fineId)
                .Select(x => new FinePaymentResponseDto
                {
                    Id = x.Id,
                    FineId = x.FineId,
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate,
                    PaymentMethod = x.PaymentMethod
                })
                .ToList();
        }

        public void Add(FinePaymentCreateDto dto)
        {
            var payment = new FinePayment
            {
                Id = Guid.NewGuid(),
                FineId = dto.FineId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                IsDeleted = false
            };

            _finePaymentRepository.Add(payment);
        }

        public void Update(Guid id, FinePaymentUpdateDto dto)
        {
            var payment = _finePaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException("Fine payment not found.");

            payment.Amount = dto.Amount;
            payment.PaymentDate = dto.PaymentDate;
            payment.PaymentMethod = dto.PaymentMethod;

            _finePaymentRepository.Update(payment);
        }

        public void Delete(Guid id)
        {
            var payment = _finePaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException("Fine payment not found.");

            _finePaymentRepository.Delete(payment);
        }
    }
}