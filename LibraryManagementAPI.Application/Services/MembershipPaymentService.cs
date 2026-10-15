
using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class MembershipPaymentService : IMembershipPaymentService
    {
        private readonly IMembershipPaymentRepository
            _membershipPaymentRepository;

        private readonly IUserMembershipRepository
            _userMembershipRepository;

        public MembershipPaymentService(
            IMembershipPaymentRepository membershipPaymentRepository,
            IUserMembershipRepository userMembershipRepository)
        {
            _membershipPaymentRepository = membershipPaymentRepository;
            _userMembershipRepository = userMembershipRepository;
        }

        public List<MembershipPaymentResponseDto> GetAll()
        {
            return _membershipPaymentRepository.GetAll()
                .Select(MapToResponseDto)
                .ToList();
        }

        public MembershipPaymentResponseDto? GetById(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            return payment == null
                ? null
                : MapToResponseDto(payment);
        }

        public List<MembershipPaymentResponseDto>
            GetByUserMembershipId(Guid userMembershipId)
        {
            return _membershipPaymentRepository
                .GetByUserMembershipId(userMembershipId)
                .Select(MapToResponseDto)
                .ToList();
        }

        public List<MembershipPaymentResponseDto> GetByUserId(Guid userId)
        {
            var memberships = _userMembershipRepository.GetByUserId(userId);

            return memberships
                .SelectMany(membership =>
                    _membershipPaymentRepository
                        .GetByUserMembershipId(membership.Id))
                .Select(MapToResponseDto)
                .ToList();
        }

        public void Add(MembershipPaymentCreateDto dto, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new UnauthorizedAccessException(
                    "Kullanıcı kimliği doğrulanamadı.");

            if (dto.UserMembershipId == Guid.Empty)
                throw new ArgumentException(
                    "Üyelik bilgisi zorunludur.");

            var membership = _userMembershipRepository
                .GetById(dto.UserMembershipId);

            if (membership == null || membership.UserId != userId)
                throw new UnauthorizedAccessException(
                    "Bu üyelik için ödeme oluşturma yetkiniz yok.");

            if (dto.Amount <= 0)
                throw new ArgumentException(
                    "Ödeme tutarı sıfırdan büyük olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
                throw new ArgumentException(
                    "Ödeme yöntemi zorunludur.");

            var payment = new MembershipPayment
            {
                Id = Guid.NewGuid(),
                UserMembershipId = membership.Id,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                IsPaid = false,
                IsDeleted = false
            };

            _membershipPaymentRepository.Add(payment);
        }

        public void Update(Guid id, MembershipPaymentUpdateDto dto)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException(
                    "Üyelik ödeme kaydı bulunamadı.");

            if (payment.IsPaid)
                throw new InvalidOperationException(
                    "Tamamlanmış bir ödemenin bilgileri değiştirilemez.");

            if (dto.Amount <= 0)
                throw new ArgumentException(
                    "Ödeme tutarı sıfırdan büyük olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
                throw new ArgumentException(
                    "Ödeme yöntemi zorunludur.");

            payment.Amount = dto.Amount;
            payment.PaymentDate = dto.PaymentDate;
            payment.PaymentMethod = dto.PaymentMethod;

            _membershipPaymentRepository.Update(payment);
        }

        public void MarkAsPaid(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException(
                    "Üyelik ödeme kaydı bulunamadı.");

            if (payment.IsPaid)
                return;

            payment.IsPaid = true;

            _membershipPaymentRepository.Update(payment);
        }

        public void Delete(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
                throw new KeyNotFoundException(
                    "Üyelik ödeme kaydı bulunamadı.");

            if (payment.IsPaid)
                throw new InvalidOperationException(
                    "Tamamlanmış ödeme kaydı silinemez.");

            _membershipPaymentRepository.Delete(payment);
        }

        private static MembershipPaymentResponseDto MapToResponseDto(
            MembershipPayment payment)
        {
            return new MembershipPaymentResponseDto
            {
                Id = payment.Id,
                UserMembershipId = payment.UserMembershipId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                IsPaid = payment.IsPaid
            };
        }
    }
}
