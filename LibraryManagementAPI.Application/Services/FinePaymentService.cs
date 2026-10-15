using LibraryManagementAPI.Application.DTOs.FinePayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class FinePaymentService : IFinePaymentService
    {
        private readonly IFinePaymentRepository _finePaymentRepository;
        private readonly IFineRepository _fineRepository;

        public FinePaymentService(
            IFinePaymentRepository finePaymentRepository,
            IFineRepository fineRepository)
        {
            _finePaymentRepository = finePaymentRepository;
            _fineRepository = fineRepository;
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
            var fine = _fineRepository.GetById(dto.FineId);

            if (fine == null)
                throw new KeyNotFoundException("Fine not found.");

            if (fine.IsPaid)
                throw new InvalidOperationException(
                    "This fine has already been paid.");

            if (dto.Amount != fine.Amount)
                throw new InvalidOperationException(
                    "Payment amount does not match the fine amount.");

            var payment = new FinePayment
            {
                Id = Guid.NewGuid(),
                FineId = dto.FineId,
                Amount = fine.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                IsDeleted = false
            };

            _finePaymentRepository.Add(payment);

            fine.IsPaid = true;

            _fineRepository.Update(fine);
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