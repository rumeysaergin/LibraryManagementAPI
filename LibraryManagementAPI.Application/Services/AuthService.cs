using LibraryManagementAPI.Application.DTOs.Auth;
using LibraryManagementAPI.Application.Repositories;

namespace LibraryManagementAPI.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public LoginResponseDto Login(LoginRequestDto dto)
        {
            var user = _userRepository.GetByEmail(dto.Email);

            if (user == null || user.Password != dto.Password)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtService.GenerateToken(user.Id, user.Email);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email
            };
        }
    }
}