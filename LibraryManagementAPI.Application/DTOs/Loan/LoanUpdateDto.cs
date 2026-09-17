namespace LibraryManagementAPI.Application.DTOs.Loan
{
    public class LoanUpdateDto
    {
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}