using LibraryManagementAPI.Application.DTOs.Author;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public List<AuthorResponseDto> GetAll()
        {
            return _authorRepository.GetAll()
                .Select(x => new AuthorResponseDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        public AuthorResponseDto? GetById(Guid id)
        {
            var author = _authorRepository.GetById(id);

            if (author == null)
                return null;

            return new AuthorResponseDto
            {
                Id = author.Id,
                Name = author.Name
            };
        }

        public void Add(AuthorCreateDto dto)
        {
            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsDeleted = false
            };

            _authorRepository.Add(author);
        }

        public void Update(Guid id, AuthorUpdateDto dto)
        {
            var author = _authorRepository.GetById(id);

            if (author == null)
                throw new KeyNotFoundException("Author not found.");

            author.Name = dto.Name;

            _authorRepository.Update(author);
        }

        public void Delete(Guid id)
        {
            var author = _authorRepository.GetById(id);

            if (author == null)
                throw new KeyNotFoundException("Author not found.");

            _authorRepository.Delete(author);
        }
    }
}