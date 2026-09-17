namespace LibraryManagementAPI.Application.DTOs.MembershipPayment
{
    public class MembershipPaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserMembershipId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}