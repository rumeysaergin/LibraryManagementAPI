using LibraryManagementAPI.Application.DTOs.MembershipPayment;

namespace LibraryManagementAPI.Application.Services
{
    public interface IMembershipPaymentService
    {
        List<MembershipPaymentResponseDto> GetAll();
        MembershipPaymentResponseDto? GetById(Guid id);
        List<MembershipPaymentResponseDto> GetByUserMembershipId(Guid userMembershipId);

        void Add(MembershipPaymentCreateDto dto);
        void Update(Guid id, MembershipPaymentUpdateDto dto);
        void Delete(Guid id);
    }
}