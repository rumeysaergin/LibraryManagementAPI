namespace LibraryManagementAPI.Application.DTOs.UserMembership
{
    public class UserMembershipResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MembershipPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}