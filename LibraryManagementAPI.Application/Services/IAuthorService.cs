using LibraryManagementAPI.Application.DTOs.Author;

namespace LibraryManagementAPI.Application.Services
{
    public interface IAuthorService
    {
        List<AuthorResponseDto> GetAll();
        AuthorResponseDto? GetById(Guid id);

        void Add(AuthorCreateDto dto);
        void Update(Guid id, AuthorUpdateDto dto);
        void Delete(Guid id);
    }
}