using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class MembershipPaymentService : IMembershipPaymentService
    {
        private readonly IMembershipPaymentRepository _membershipPaymentRepository;

        public MembershipPaymentService(IMembershipPaymentRepository membershipPaymentRepository)
        {
            _membershipPaymentRepository = membershipPaymentRepository;
        }

        public List<MembershipPaymentResponseDto> GetAll()
        {
            return _membershipPaymentRepository.GetAll()
                .Select(x => new MembershipPaymentResponseDto
                {
                    Id = x.Id,
                    UserMembershipId = x.UserMembershipId,
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate,
                    PaymentMethod = x.PaymentMethod
                })
                .ToList();
        }

        public MembershipPaymentResponseDto? GetById(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
                return null;

            return new MembershipPaymentResponseDto
            {
                Id = payment.Id,
                UserMembershipId = payment.UserMembershipId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod
            };
        }

        public List<MembershipPaymentResponseDto> GetByUserMembershipId(Guid userMembershipId)
        {
            return _membershipPaymentRepository.GetByUserMembershipId(userMembershipId)
                .Select(x => new MembershipPaymentResponseDto
                {
                    Id = x.Id,
                    UserMembershipId = x.UserMembershipId,
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate,
                    PaymentMethod = x.PaymentMethod
                })
                .ToList();
        }

        public void Add(MembershipPaymentCreateDto dto)
        {
            var payment = new MembershipPayment
            {
                Id = Guid.NewGuid(),
                UserMembershipId = dto.UserMembershipId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                IsDeleted = false
            };

            _membershipPaymentRepository.Add(payment);
        }

        public void Update(Guid id, MembershipPaymentUpdateDto dto)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException("Membership payment not found.");

            payment.Amount = dto.Amount;
            payment.PaymentDate = dto.PaymentDate;
            payment.PaymentMethod = dto.PaymentMethod;

            _membershipPaymentRepository.Update(payment);
        }

        public void Delete(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException("Membership payment not found.");

            _membershipPaymentRepository.Delete(payment);
        }
    }
}