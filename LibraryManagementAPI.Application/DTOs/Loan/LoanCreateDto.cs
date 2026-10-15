
namespace LibraryManagementAPI.Application.DTOs.Loan
{
    public class LoanCreateDto
    {
        public Guid UserId { get; set; }

        public Guid LibraryBookId { get; set; }
    }
}