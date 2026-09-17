using LibraryManagementAPI.Application.DTOs.MembershipPlan;

namespace LibraryManagementAPI.Application.Services
{
    public interface IMembershipPlanService
    {
        List<MembershipPlanResponseDto> GetAll();
        MembershipPlanResponseDto? GetById(Guid id);

        void Add(MembershipPlanCreateDto dto);
        void Update(Guid id, MembershipPlanUpdateDto dto);
        void Delete(Guid id);
    }
}