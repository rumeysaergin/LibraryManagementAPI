namespace LibraryManagementAPI.Application.DTOs.Loan
{
    public class LoanCreateDto
    {
        public Guid UserId { get; set; }

        public Guid LibraryBookId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }
    }
}