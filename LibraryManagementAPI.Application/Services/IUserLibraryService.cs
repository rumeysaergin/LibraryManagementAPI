using LibraryManagementAPI.Application.DTOs.UserLibrary;

namespace LibraryManagementAPI.Application.Services
{
    public interface IUserLibraryService
    {
        List<UserLibraryResponseDto> GetAll();
        UserLibraryResponseDto? GetById(Guid id);
        List<UserLibraryResponseDto> GetByUserId(Guid userId);
        List<UserLibraryResponseDto> GetByLibraryId(Guid libraryId);

        void Add(UserLibraryCreateDto dto);
        void Update(Guid id, UserLibraryUpdateDto dto);
        void Delete(Guid id);
    }
}