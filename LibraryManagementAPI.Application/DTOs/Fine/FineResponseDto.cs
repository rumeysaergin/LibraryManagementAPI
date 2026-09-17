namespace LibraryManagementAPI.Application.DTOs.Fine
{
    public class FineResponseDto
    {
        public Guid Id { get; set; }
        public Guid LoanId { get; set; }
        public decimal Amount { get; set; }
        public DateTime FineDate { get; set; }
        public bool IsPaid { get; set; }
    }
}