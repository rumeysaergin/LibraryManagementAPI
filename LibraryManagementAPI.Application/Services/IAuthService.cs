using LibraryManagementAPI.Application.DTOs.Auth;

namespace LibraryManagementAPI.Application.Services
{
    public interface IAuthService
    {
        LoginResponseDto Login(LoginRequestDto dto);
    }
}