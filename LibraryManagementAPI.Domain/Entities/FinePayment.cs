namespace LibraryManagementAPI.Domain.Entities
{
    public class FinePayment
    {
        public Guid Id { get; set; }

        public Guid FineId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public Fine Fine { get; set; } = null!;
    }
}