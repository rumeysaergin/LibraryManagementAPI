namespace LibraryManagementAPI.Application.DTOs.UserMembership
{
    public class UserMembershipUpdateDto
    {
        public Guid MembershipPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}