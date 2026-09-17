namespace LibraryManagementAPI.Application.DTOs.FinePayment
{
    public class FinePaymentUpdateDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}