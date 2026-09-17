namespace LibraryManagementAPI.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public Guid AuthorId { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsDeleted { get; set; }

        public Author Author { get; set; } = null!;

        public Category Category { get; set; } = null!;

        public ICollection<Loan> Loans { get; set; }
            = new List<Loan>();
    }
}