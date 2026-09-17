namespace LibraryManagementAPI.Application.DTOs.Loan
{
    public class LoanCreateDto
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}