namespace LibraryManagementAPI.Application.DTOs.Book
{
    public class BookUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public Guid AuthorId { get; set; }
        public Guid CategoryId { get; set; }
    }
}