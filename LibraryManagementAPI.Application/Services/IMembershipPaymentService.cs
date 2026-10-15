
using LibraryManagementAPI.Application.DTOs.MembershipPayment;

namespace LibraryManagementAPI.Application.Services
{
    public interface IMembershipPaymentService
    {
        List<MembershipPaymentResponseDto> GetAll();

        MembershipPaymentResponseDto? GetById(Guid id);

        List<MembershipPaymentResponseDto> GetByUserMembershipId(
            Guid userMembershipId);

        List<MembershipPaymentResponseDto> GetByUserId(Guid userId);

        void Add(MembershipPaymentCreateDto dto, Guid userId);

        void Update(Guid id, MembershipPaymentUpdateDto dto);

        void MarkAsPaid(Guid id);

        void Delete(Guid id);
    }
}
