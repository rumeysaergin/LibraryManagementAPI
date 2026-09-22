using LibraryManagementAPI.Application.DTOs.UserLibrary;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class UserLibraryService : IUserLibraryService
    {
        private readonly IUserLibraryRepository _userLibraryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILibraryRepository _libraryRepository;

        public UserLibraryService(
            IUserLibraryRepository userLibraryRepository,
            IUserRepository userRepository,
            ILibraryRepository libraryRepository)
        {
            _userLibraryRepository = userLibraryRepository;
            _userRepository = userRepository;
            _libraryRepository = libraryRepository;
        }

        public List<UserLibraryResponseDto> GetAll()
        {
            return _userLibraryRepository.GetAll()
                .Select(MapToDto)
                .ToList();
        }

        public UserLibraryResponseDto? GetById(Guid id)
        {
            var entity = _userLibraryRepository.GetById(id);

            return entity == null ? null : MapToDto(entity);
        }

        public List<UserLibraryResponseDto> GetByUserId(Guid userId)
        {
            return _userLibraryRepository.GetByUserId(userId)
                .Select(MapToDto)
                .ToList();
        }

        public List<UserLibraryResponseDto> GetByLibraryId(Guid libraryId)
        {
            return _userLibraryRepository.GetByLibraryId(libraryId)
                .Select(MapToDto)
                .ToList();
        }

        public void Add(UserLibraryCreateDto dto)
        {
            var user = _userRepository.GetById(dto.UserId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var library = _libraryRepository.GetById(dto.LibraryId);

            if (library == null)
                throw new KeyNotFoundException("Library not found.");

            var existing = _userLibraryRepository
                .GetByUserId(dto.UserId)
                .Any(x => x.LibraryId == dto.LibraryId);

            if (existing)
                throw new InvalidOperationException(
                    "User is already registered to this library.");

            var entity = new UserLibrary
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                LibraryId = dto.LibraryId,
                IsDeleted = false
            };

            _userLibraryRepository.Add(entity);
        }

        public void Update(Guid id, UserLibraryUpdateDto dto)
        {
            var entity = _userLibraryRepository.GetById(id);

            if (entity == null)
                throw new KeyNotFoundException("UserLibrary not found.");

            var library = _libraryRepository.GetById(dto.LibraryId);

            if (library == null)
                throw new KeyNotFoundException("Library not found.");

            entity.LibraryId = dto.LibraryId;

            _userLibraryRepository.Update(entity);
        }

        public void Delete(Guid id)
        {
            var entity = _userLibraryRepository.GetById(id);

            if (entity == null)
                throw new KeyNotFoundException("UserLibrary not found.");

            _userLibraryRepository.Delete(entity);
        }

        private static UserLibraryResponseDto MapToDto(UserLibrary entity)
        {
            return new UserLibraryResponseDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                LibraryId = entity.LibraryId
            };
        }
    }
}