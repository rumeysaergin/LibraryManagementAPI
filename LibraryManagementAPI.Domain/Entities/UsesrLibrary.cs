namespace LibraryManagementAPI.Domain.Entities
{
    public class UserLibrary
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsDeleted { get; set; }

        public User User { get; set; } = null!;

        public Library Library { get; set; } = null!;
    }
}