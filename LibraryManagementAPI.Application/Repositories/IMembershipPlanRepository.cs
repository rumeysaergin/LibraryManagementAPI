using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IMembershipPlanRepository
    {
        List<MembershipPlan> GetAll();
        MembershipPlan? GetById(Guid id);

        void Add(MembershipPlan membershipPlan);
        void Update(MembershipPlan membershipPlan);
        void Delete(MembershipPlan membershipPlan);
    }
}