namespace LibraryManagementAPI.Application.DTOs.MembershipPayment
{
    public class MembershipPaymentCreateDto
    {
        public Guid UserMembershipId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}