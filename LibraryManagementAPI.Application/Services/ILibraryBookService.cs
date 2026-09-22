using LibraryManagementAPI.Application.DTOs.LibraryBook;

namespace LibraryManagementAPI.Application.Services
{
    public interface ILibraryBookService
    {
        List<LibraryBookResponseDto> GetAll();

        LibraryBookResponseDto? GetById(Guid id);

        List<LibraryBookResponseDto> GetByLibraryId(Guid libraryId);

        List<LibraryBookResponseDto> GetByBookId(Guid bookId);

        void Add(LibraryBookCreateDto dto);

        void Update(Guid id, LibraryBookUpdateDto dto);

        void Delete(Guid id);
    }
}