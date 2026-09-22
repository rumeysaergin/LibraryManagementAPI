namespace LibraryManagementAPI.Application.DTOs.UserLibrary
{
    public class UserLibraryResponseDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid LibraryId { get; set; }
    }
}