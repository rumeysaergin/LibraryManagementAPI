using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class MembershipPaymentRepository : IMembershipPaymentRepository
    {
        private readonly LibraryManagementDbContext _context;

        public MembershipPaymentRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<MembershipPayment> GetAll()
        {
            return _context.MembershipPayments
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public MembershipPayment? GetById(Guid id)
        {
            return _context.MembershipPayments
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<MembershipPayment> GetByUserMembershipId(Guid userMembershipId)
        {
            return _context.MembershipPayments
                .Where(x => x.UserMembershipId == userMembershipId && !x.IsDeleted)
                .ToList();
        }

        public void Add(MembershipPayment membershipPayment)
        {
            _context.MembershipPayments.Add(membershipPayment);
            _context.SaveChanges();
        }

        public void Update(MembershipPayment membershipPayment)
        {
            _context.MembershipPayments.Update(membershipPayment);
            _context.SaveChanges();
        }

        public void Delete(MembershipPayment membershipPayment)
        {
            membershipPayment.IsDeleted = true;
            _context.MembershipPayments.Update(membershipPayment);
            _context.SaveChanges();
        }
    }
}