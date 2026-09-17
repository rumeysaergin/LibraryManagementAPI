using LibraryManagementAPI.Application.DTOs.User;

namespace LibraryManagementAPI.Application.Services
{
    public interface IUserService
    {
        List<UserResponseDto> GetAll();
        UserResponseDto? GetById(Guid id);

        UserResponseDto? GetByEmail(string email);

        void Add(UserCreateDto dto);
        void Update(Guid id, UserUpdateDto dto);
        void Delete(Guid id);
    }
}