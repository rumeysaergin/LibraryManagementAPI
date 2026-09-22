namespace LibraryManagementAPI.Application.DTOs.LibraryBook
{
    public class LibraryBookCreateDto
    {
        public Guid LibraryId { get; set; }

        public Guid BookId { get; set; }

        public int Quantity { get; set; }
    }
}