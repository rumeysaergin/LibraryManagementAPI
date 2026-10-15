
namespace LibraryManagementAPI.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public bool IsDeleted { get; set; }

        public ICollection<UserMembership> UserMemberships { get; set; }
            = new List<UserMembership>();

        public ICollection<Loan> Loans { get; set; }
            = new List<Loan>();

        public ICollection<UserLibrary> UserLibraries { get; set; }
            = new List<UserLibrary>();
    }
}
