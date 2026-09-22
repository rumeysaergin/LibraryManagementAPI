using LibraryManagementAPI.Application.DTOs.Library;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ILibraryRepository _libraryRepository;

        public LibraryService(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public List<LibraryResponseDto> GetAll()
        {
            return _libraryRepository.GetAll()
                .Select(x => new LibraryResponseDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address
                })
                .ToList();
        }

        public LibraryResponseDto? GetById(Guid id)
        {
            var library = _libraryRepository.GetById(id);

            if (library == null)
                return null;

            return new LibraryResponseDto
            {
                Id = library.Id,
                Name = library.Name,
                Address = library.Address
            };
        }

        public void Add(LibraryCreateDto dto)
        {
            var library = new Library
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                IsDeleted = false
            };

            _libraryRepository.Add(library);
        }

        public void Update(Guid id, LibraryUpdateDto dto)
        {
            var library = _libraryRepository.GetById(id);

            if (library == null)
                throw new KeyNotFoundException("Library not found.");

            library.Name = dto.Name;
            library.Address = dto.Address;

            _libraryRepository.Update(library);
        }

        public void Delete(Guid id)
        {
            var library = _libraryRepository.GetById(id);

            if (library == null)
                throw new KeyNotFoundException("Library not found.");

            _libraryRepository.Delete(library);
        }
    }
}