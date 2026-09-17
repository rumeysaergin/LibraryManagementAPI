namespace LibraryManagementAPI.Application.DTOs.MembershipPlan
{
    public class MembershipPlanCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int MaxBorrowLimit { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
    }
}