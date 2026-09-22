using LibraryManagementAPI.Application.DTOs.Library;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class LibraryServiceTests
    {
        private readonly Mock<ILibraryRepository> _repositoryMock;
        private readonly LibraryService _service;

        public LibraryServiceTests()
        {
            _repositoryMock = new Mock<ILibraryRepository>();
            _service = new LibraryService(_repositoryMock.Object);
        }

        private Library CreateLibrary()
        {
            return new Library
            {
                Id = Guid.NewGuid(),
                Name = "Bursa Merkez Kütüphanesi",
                Address = "Bursa",
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnLibraries()
        {
            var libraries = new List<Library>
            {
                CreateLibrary(),
                CreateLibrary()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(libraries);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnLibrary_WhenLibraryExists()
        {
            var library = CreateLibrary();

            _repositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            var result = _service.GetById(library.Id);

            Assert.NotNull(result);
            Assert.Equal(library.Id, result.Id);
            Assert.Equal(library.Name, result.Name);
            Assert.Equal(library.Address, result.Address);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenLibraryDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Library?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new LibraryCreateDto
            {
                Name = "Yeni Kütüphane",
                Address = "Nilüfer / Bursa"
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.Is<Library>(library =>
                    library.Name == dto.Name &&
                    library.Address == dto.Address &&
                    library.IsDeleted == false)),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateLibrary_WhenLibraryExists()
        {
            var library = CreateLibrary();

            var dto = new LibraryUpdateDto
            {
                Name = "Güncellenmiş Kütüphane",
                Address = "Osmangazi / Bursa"
            };

            _repositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            _service.Update(library.Id, dto);

            Assert.Equal(dto.Name, library.Name);
            Assert.Equal(dto.Address, library.Address);

            _repositoryMock.Verify(
                x => x.Update(library),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenLibraryDoesNotExist()
        {
            var id = Guid.NewGuid();

            var dto = new LibraryUpdateDto
            {
                Name = "Güncellenmiş Kütüphane",
                Address = "Bursa"
            };

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Library?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));

            _repositoryMock.Verify(
                x => x.Update(It.IsAny<Library>()),
                Times.Never);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenLibraryExists()
        {
            var library = CreateLibrary();

            _repositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            _service.Delete(library.Id);

            _repositoryMock.Verify(
                x => x.Delete(library),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenLibraryDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Library?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));

            _repositoryMock.Verify(
                x => x.Delete(It.IsAny<Library>()),
                Times.Never);
        }
    }
}