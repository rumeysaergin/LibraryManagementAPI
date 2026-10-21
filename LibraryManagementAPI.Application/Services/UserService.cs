
using LibraryManagementAPI.Application.DTOs.User;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserResponseDto> GetAll()
        {
            return _userRepository.GetAll()
                .Select(x => new UserResponseDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Role = x.Role
                })
                .ToList();
        }

        public UserResponseDto? GetById(Guid id)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
                return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public UserResponseDto? GetByEmail(string email)
        {
            var user = _userRepository.GetByEmail(email);

            if (user == null)
                return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public void Add(UserCreateDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Role = string.IsNullOrWhiteSpace(dto.Role)
                    ? "User"
                    : dto.Role,
                IsDeleted = false
            };

            _userRepository.Add(user);
        }

        public void Update(Guid id, UserUpdateDto dto)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = dto.Role;
            }

            _userRepository.Update(user);
        }

        public void Delete(Guid id)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            _userRepository.Delete(user);
        }
    }
}
