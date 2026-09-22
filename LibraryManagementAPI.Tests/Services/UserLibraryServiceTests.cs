using LibraryManagementAPI.Application.DTOs.UserLibrary;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;
using Xunit;

namespace LibraryManagementAPI.Tests.Services
{
    public class UserLibraryServiceTests
    {
        private readonly Mock<IUserLibraryRepository> _userLibraryRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILibraryRepository> _libraryRepositoryMock;

        private readonly UserLibraryService _service;

        public UserLibraryServiceTests()
        {
            _userLibraryRepositoryMock = new Mock<IUserLibraryRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _libraryRepositoryMock = new Mock<ILibraryRepository>();

            _service = new UserLibraryService(
                _userLibraryRepositoryMock.Object,
                _userRepositoryMock.Object,
                _libraryRepositoryMock.Object);
        }

        [Fact]
        public void Add_ShouldAddUserLibrary_WhenUserAndLibraryExist()
        {
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            var library = new Library
            {
                Id = libraryId
            };

            _userRepositoryMock
                .Setup(x => x.GetById(userId))
                .Returns(user);

            _libraryRepositoryMock
                .Setup(x => x.GetById(libraryId))
                .Returns(library);

            _userLibraryRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserLibrary>());

            var dto = new UserLibraryCreateDto
            {
                UserId = userId,
                LibraryId = libraryId
            };

            _service.Add(dto);

            _userLibraryRepositoryMock.Verify(
                x => x.Add(It.Is<UserLibrary>(ul =>
                    ul.UserId == userId &&
                    ul.LibraryId == libraryId &&
                    !ul.IsDeleted)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldThrow_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(x => x.GetById(userId))
                .Returns((User?)null);

            var dto = new UserLibraryCreateDto
            {
                UserId = userId,
                LibraryId = libraryId
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto));

            _userLibraryRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserLibrary>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenLibraryDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            _userRepositoryMock
                .Setup(x => x.GetById(userId))
                .Returns(user);

            _libraryRepositoryMock
                .Setup(x => x.GetById(libraryId))
                .Returns((Library?)null);

            var dto = new UserLibraryCreateDto
            {
                UserId = userId,
                LibraryId = libraryId
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto));

            _userLibraryRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserLibrary>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenUserAlreadyRegisteredToLibrary()
        {
            var userId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            var library = new Library
            {
                Id = libraryId
            };

            var existingUserLibrary = new UserLibrary
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LibraryId = libraryId,
                IsDeleted = false
            };

            _userRepositoryMock
                .Setup(x => x.GetById(userId))
                .Returns(user);

            _libraryRepositoryMock
                .Setup(x => x.GetById(libraryId))
                .Returns(library);

            _userLibraryRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserLibrary>
                {
                    existingUserLibrary
                });

            var dto = new UserLibraryCreateDto
            {
                UserId = userId,
                LibraryId = libraryId
            };

            Assert.Throws<InvalidOperationException>(
                () => _service.Add(dto));

            _userLibraryRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserLibrary>()),
                Times.Never);
        }

        [Fact]
        public void GetByUserId_ShouldReturnUserLibraries()
        {
            var userId = Guid.NewGuid();

            var data = new List<UserLibrary>
            {
                new UserLibrary
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LibraryId = Guid.NewGuid()
                },
                new UserLibrary
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LibraryId = Guid.NewGuid()
                }
            };

            _userLibraryRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(data);

            var result = _service.GetByUserId(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(userId, x.UserId));
        }

        [Fact]
        public void GetByLibraryId_ShouldReturnUserLibraries()
        {
            var libraryId = Guid.NewGuid();

            var data = new List<UserLibrary>
            {
                new UserLibrary
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    LibraryId = libraryId
                },
                new UserLibrary
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    LibraryId = libraryId
                }
            };

            _userLibraryRepositoryMock
                .Setup(x => x.GetByLibraryId(libraryId))
                .Returns(data);

            var result = _service.GetByLibraryId(libraryId);

            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(libraryId, x.LibraryId));
        }

        [Fact]
        public void Update_ShouldUpdateLibrary()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var oldLibraryId = Guid.NewGuid();
            var newLibraryId = Guid.NewGuid();

            var entity = new UserLibrary
            {
                Id = id,
                UserId = userId,
                LibraryId = oldLibraryId,
                IsDeleted = false
            };

            var library = new Library
            {
                Id = newLibraryId
            };

            _userLibraryRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns(entity);

            _libraryRepositoryMock
                .Setup(x => x.GetById(newLibraryId))
                .Returns(library);

            var dto = new UserLibraryUpdateDto
            {
                LibraryId = newLibraryId
            };

            _service.Update(id, dto);

            Assert.Equal(newLibraryId, entity.LibraryId);

            _userLibraryRepositoryMock.Verify(
                x => x.Update(entity),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldSoftDeleteUserLibrary()
        {
            var id = Guid.NewGuid();

            var entity = new UserLibrary
            {
                Id = id,
                UserId = Guid.NewGuid(),
                LibraryId = Guid.NewGuid(),
                IsDeleted = false
            };

            _userLibraryRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns(entity);

            _service.Delete(id);

            _userLibraryRepositoryMock.Verify(
                x => x.Delete(entity),
                Times.Once);
        }
    }
}