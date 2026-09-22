namespace LibraryManagementAPI.Domain.Entities
{
    public class Library
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<LibraryBook> LibraryBooks { get; set; } = new List<LibraryBook>();

        public ICollection<UserLibrary> UserLibraries { get; set; }
    = new List<UserLibrary>();

    }
}