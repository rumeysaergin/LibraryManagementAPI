using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class MembershipPaymentService : IMembershipPaymentService
    {
        private readonly IMembershipPaymentRepository _membershipPaymentRepository;
        private readonly IUserMembershipRepository _userMembershipRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public MembershipPaymentService(
            IMembershipPaymentRepository membershipPaymentRepository,
            IUserMembershipRepository userMembershipRepository,
            IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPaymentRepository = membershipPaymentRepository;
            _userMembershipRepository = userMembershipRepository;
            _membershipPlanRepository = membershipPlanRepository;
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

            return payment == null ? null : MapToResponseDto(payment);
        }

        public List<MembershipPaymentResponseDto> GetByUserMembershipId(
            Guid userMembershipId)
        {
            return _membershipPaymentRepository
                .GetByUserMembershipId(userMembershipId)
                .Select(MapToResponseDto)
                .ToList();
        }

        public List<MembershipPaymentResponseDto> GetByUserId(Guid userId)
        {
            var memberships =
                _userMembershipRepository.GetByUserId(userId)
                ?? new List<UserMembership>();

            return memberships
                .SelectMany(membership =>
                    _membershipPaymentRepository
                        .GetByUserMembershipId(membership.Id)
                    ?? new List<MembershipPayment>())
                .Select(MapToResponseDto)
                .ToList();
        }

        public void Add(MembershipPaymentCreateDto dto, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı kimliği doğrulanamadı.");
            }

            if (dto.MembershipPlanId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Üyelik planı seçilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                throw new ArgumentException(
                    "Ödeme yöntemi zorunludur.");
            }

            var memberships =
                _userMembershipRepository.GetByUserId(userId)
                ?? new List<UserMembership>();

            var hasActiveMembership = memberships.Any(membership =>
                membership.IsActive &&
                membership.EndDate > DateTime.UtcNow);

            if (hasActiveMembership)
            {
                throw new InvalidOperationException(
                    "Aktif bir üyeliğiniz bulunmaktadır. Yeni paket satın almak için mevcut üyeliğinizin sona ermesini beklemelisiniz.");
            }

            var hasPendingPayment = memberships
                .SelectMany(membership =>
                    _membershipPaymentRepository
                        .GetByUserMembershipId(membership.Id)
                    ?? new List<MembershipPayment>())
                .Any(payment => !payment.IsPaid && !payment.IsDeleted);

            if (hasPendingPayment)
            {
                throw new InvalidOperationException(
                    "Onay bekleyen bir ödemeniz bulunmaktadır. Yeni paket satın almadan önce mevcut ödemenizin sonuçlanmasını beklemelisiniz.");
            }

            var plan = _membershipPlanRepository.GetById(
                dto.MembershipPlanId);

            if (plan == null)
            {
                throw new KeyNotFoundException(
                    "Seçilen üyelik planı bulunamadı.");
            }

            if (plan.Price <= 0 || plan.DurationInDays <= 0)
            {
                throw new InvalidOperationException(
                    "Üyelik planının fiyatı veya süresi geçersiz.");
            }

            var now = DateTime.UtcNow;

            var membership = new UserMembership
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MembershipPlanId = plan.Id,
                StartDate = now,
                EndDate = now,
                IsActive = false,
                IsDeleted = false
            };

            var payment = new MembershipPayment
            {
                Id = Guid.NewGuid(),
                UserMembershipId = membership.Id,
                Amount = plan.Price,
                PaymentDate = now,
                PaymentMethod = dto.PaymentMethod.Trim(),
                IsPaid = false,
                IsDeleted = false
            };

            membership.MembershipPayments.Add(payment);

            // Üyelik ve ilişkili ödeme birlikte kaydedilir.
            _userMembershipRepository.Add(membership);
        }

        public void Update(Guid id, MembershipPaymentUpdateDto dto)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
            {
                throw new KeyNotFoundException(
                    "Üyelik ödeme kaydı bulunamadı.");
            }

            if (payment.IsPaid)
            {
                throw new InvalidOperationException(
                    "Onaylanmış bir ödemenin bilgileri değiştirilemez.");
            }

            if (dto.Amount <= 0)
            {
                throw new ArgumentException(
                    "Ödeme tutarı sıfırdan büyük olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                throw new ArgumentException(
                    "Ödeme yöntemi zorunludur.");
            }

            var membership = _userMembershipRepository.GetById(
                payment.UserMembershipId);

            if (membership == null)
            {
                throw new KeyNotFoundException(
                    "Ödemeye bağlı üyelik bulunamadı.");
            }

            var plan = _membershipPlanRepository.GetById(
                membership.MembershipPlanId);

            if (plan == null)
            {
                throw new KeyNotFoundException(
                    "Üyelik planı bulunamadı.");
            }

            payment.Amount = plan.Price;
            payment.PaymentMethod = dto.PaymentMethod.Trim();

            _membershipPaymentRepository.Update(payment);
        }

        public void MarkAsPaid(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
            {
                throw new KeyNotFoundException(
                    "Üyelik ödeme kaydı bulunamadı.");
            }

            if (payment.IsPaid)
            {
                return;
            }

            var membership = _userMembershipRepository.GetById(
                payment.UserMembershipId);

            if (membership == null)
            {
                throw new KeyNotFoundException(
                    "Ödemeye bağlı üyelik bulunamadı.");
            }

            var plan = _membershipPlanRepository.GetById(
                membership.MembershipPlanId);

            if (plan == null)
            {
                throw new KeyNotFoundException(
                    "Üyelik planı bulunamadı.");
            }

            var startDate = DateTime.UtcNow;

            membership.StartDate = startDate;
            membership.EndDate = startDate.AddDays(plan.DurationInDays);
            membership.IsActive = true;

            _userMembershipRepository.Update(membership);

            payment.IsPaid = true;

            _membershipPaymentRepository.Update(payment);
        }

        public void Delete(Guid id)
        {
            var payment = _membershipPaymentRepository.GetById(id);

            if (payment == null)
            {
                throw new KeyNotFoundException(
                    "Üyelik ödeme kaydı bulunamadı.");
            }

            if (payment.IsPaid)
            {
                throw new InvalidOperationException(
                    "Onaylanmış ödeme kaydı silinemez.");
            }

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