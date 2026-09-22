namespace LibraryManagementAPI.Application.DTOs.Book
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public Guid AuthorId { get; set; }

        public Guid CategoryId { get; set; }
    }
}