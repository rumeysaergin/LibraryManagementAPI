namespace LibraryManagementAPI.Domain.Entities
{
    public class UserMembership
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid MembershipPlanId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;

        public MembershipPlan MembershipPlan { get; set; } = null!;

        public ICollection<MembershipPayment> MembershipPayments { get; set; }
            = new List<MembershipPayment>();
    }
}