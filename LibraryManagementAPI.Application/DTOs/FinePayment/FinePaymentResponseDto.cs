namespace LibraryManagementAPI.Application.DTOs.FinePayment
{
    public class FinePaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid FineId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}