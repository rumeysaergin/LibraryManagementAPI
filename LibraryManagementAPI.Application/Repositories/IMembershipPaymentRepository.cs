using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IMembershipPaymentRepository
    {
        List<MembershipPayment> GetAll();

        MembershipPayment? GetById(Guid id);

        List<MembershipPayment> GetByUserMembershipId(
            Guid userMembershipId);

        void Add(MembershipPayment membershipPayment);

        void Update(MembershipPayment membershipPayment);

        void Delete(MembershipPayment membershipPayment);
    }
}