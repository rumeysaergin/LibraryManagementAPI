namespace LibraryManagementAPI.Application.DTOs.LibraryBook
{
    public class LibraryBookResponseDto
    {
        public Guid Id { get; set; }

        public Guid LibraryId { get; set; }

        public Guid BookId { get; set; }

        public int Quantity { get; set; }
    }
}