namespace LibraryManagementAPI.Domain.Entities
{
    public class LibraryBook
    {
        public Guid Id { get; set; }

        public Guid LibraryId { get; set; }

        public Guid BookId { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }

        public Library Library { get; set; } = null!;

        public Book Book { get; set; } = null!;

        public ICollection<Loan> Loans { get; set; }
            = new List<Loan>();
    }
}