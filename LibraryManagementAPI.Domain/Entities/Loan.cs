namespace LibraryManagementAPI.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid BookId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;

        public Book Book { get; set; } = null!;

        public ICollection<Fine> Fines { get; set; }
            = new List<Fine>();
    }
}