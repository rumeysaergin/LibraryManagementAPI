using LibraryManagementAPI.Application.DTOs.Library;

namespace LibraryManagementAPI.Application.Services
{
    public interface ILibraryService
    {
        List<LibraryResponseDto> GetAll();
        LibraryResponseDto? GetById(Guid id);

        void Add(LibraryCreateDto dto);
        void Update(Guid id, LibraryUpdateDto dto);
        void Delete(Guid id);
    }
}