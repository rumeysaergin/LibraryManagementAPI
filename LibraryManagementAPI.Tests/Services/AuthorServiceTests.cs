using LibraryManagementAPI.Application.DTOs.Author;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class AuthorServiceTests
    {
        private readonly Mock<IAuthorRepository> _repositoryMock;
        private readonly AuthorService _service;

        public AuthorServiceTests()
        {
            _repositoryMock = new Mock<IAuthorRepository>();
            _service = new AuthorService(_repositoryMock.Object);
        }

        private Author CreateAuthor()
        {
            return new Author
            {
                Id = Guid.NewGuid(),
                Name = "Test Yazar",
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnAuthors()
        {
            var authors = new List<Author>
            {
                CreateAuthor(),
                CreateAuthor()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(authors);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnAuthor_WhenAuthorExists()
        {
            var author = CreateAuthor();

            _repositoryMock
                .Setup(x => x.GetById(author.Id))
                .Returns(author);

            var result = _service.GetById(author.Id);

            Assert.NotNull(result);
            Assert.Equal(author.Id, result!.Id);
            Assert.Equal(author.Name, result.Name);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenAuthorDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Author?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new AuthorCreateDto
            {
                Name = "Yeni Yazar"
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<Author>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateAuthor_WhenAuthorExists()
        {
            var author = CreateAuthor();

            _repositoryMock
                .Setup(x => x.GetById(author.Id))
                .Returns(author);

            var dto = new AuthorUpdateDto
            {
                Name = "Güncellenmiş Yazar"
            };

            _service.Update(author.Id, dto);

            Assert.Equal("Güncellenmiş Yazar", author.Name);

            _repositoryMock.Verify(
                x => x.Update(author),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenAuthorDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Author?)null);

            var dto = new AuthorUpdateDto
            {
                Name = "Güncellenmiş Yazar"
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenAuthorExists()
        {
            var author = CreateAuthor();

            _repositoryMock
                .Setup(x => x.GetById(author.Id))
                .Returns(author);

            _service.Delete(author.Id);

            _repositoryMock.Verify(
                x => x.Delete(author),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenAuthorDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Author?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}