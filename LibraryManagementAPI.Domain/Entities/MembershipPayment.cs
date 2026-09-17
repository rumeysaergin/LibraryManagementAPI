namespace LibraryManagementAPI.Domain.Entities
{
    public class MembershipPayment
    {
        public Guid Id { get; set; }

        public Guid UserMembershipId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public UserMembership UserMembership { get; set; } = null!;
    }
}