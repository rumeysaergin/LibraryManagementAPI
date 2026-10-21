
namespace LibraryManagementAPI.Application.DTOs.MembershipPayment
{
    public class MembershipPaymentCreateDto
    {
        public Guid MembershipPlanId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;
    }
}
