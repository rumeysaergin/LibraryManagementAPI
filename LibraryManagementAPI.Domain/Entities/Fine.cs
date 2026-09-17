namespace LibraryManagementAPI.Domain.Entities
{
    public class Fine
    {
        public Guid Id { get; set; }

        public Guid LoanId { get; set; }

        public decimal Amount { get; set; }

        public DateTime FineDate { get; set; }

        public bool IsPaid { get; set; }

        public bool IsDeleted { get; set; }

        public Loan Loan { get; set; } = null!;

        public ICollection<FinePayment> FinePayments { get; set; }
            = new List<FinePayment>();
    }
}