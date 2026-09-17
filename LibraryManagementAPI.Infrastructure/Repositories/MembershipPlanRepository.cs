using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class MembershipPlanRepository : IMembershipPlanRepository
    {
        private readonly LibraryManagementDbContext _context;

        public MembershipPlanRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<MembershipPlan> GetAll()
        {
            return _context.MembershipPlans
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public MembershipPlan? GetById(Guid id)
        {
            return _context.MembershipPlans
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public void Add(MembershipPlan membershipPlan)
        {
            _context.MembershipPlans.Add(membershipPlan);
            _context.SaveChanges();
        }

        public void Update(MembershipPlan membershipPlan)
        {
            _context.MembershipPlans.Update(membershipPlan);
            _context.SaveChanges();
        }

        public void Delete(MembershipPlan membershipPlan)
        {
            membershipPlan.IsDeleted = true;
            _context.MembershipPlans.Update(membershipPlan);
            _context.SaveChanges();
        }
    }
}