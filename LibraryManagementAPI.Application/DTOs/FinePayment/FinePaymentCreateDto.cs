namespace LibraryManagementAPI.Application.DTOs.FinePayment
{
    public class FinePaymentCreateDto
    {
        public Guid FineId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}