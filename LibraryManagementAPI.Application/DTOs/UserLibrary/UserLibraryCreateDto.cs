namespace LibraryManagementAPI.Application.DTOs.UserLibrary
{
    public class UserLibraryCreateDto
    {
        public Guid UserId { get; set; }

        public Guid LibraryId { get; set; }
    }
}