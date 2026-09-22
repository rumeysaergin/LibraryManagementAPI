using LibraryManagementAPI.Application.DTOs.Book;

namespace LibraryManagementAPI.Application.Services
{
    public interface IBookService
    {
        List<BookResponseDto> GetAll();

        BookResponseDto? GetById(Guid id);

        List<BookResponseDto> GetByAuthorId(Guid authorId);

        List<BookResponseDto> GetByCategoryId(Guid categoryId);

        void Add(BookCreateDto dto);

        void Update(Guid id, BookUpdateDto dto);

        void Delete(Guid id);
    }
}