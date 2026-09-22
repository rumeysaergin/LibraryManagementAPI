using LibraryManagementAPI.Application.DTOs.User;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _service = new UserService(_repositoryMock.Object);
        }

        private User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Kullanıcı",
                Email = "test@test.com",
                Password = "123456",
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnUsers()
        {
            var users = new List<User>
            {
                CreateUser(),
                CreateUser()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(users);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnUser_WhenUserExists()
        {
            var user = CreateUser();

            _repositoryMock
                .Setup(x => x.GetById(user.Id))
                .Returns(user);

            var result = _service.GetById(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result!.Id);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((User?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new UserCreateDto
            {
                FirstName = "Yeni",
                LastName = "Kullanıcı",
                Email = "yeni@test.com",
                Password = "123456"
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<User>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateUser_WhenUserExists()
        {
            var user = CreateUser();

            _repositoryMock
                .Setup(x => x.GetById(user.Id))
                .Returns(user);

            var dto = new UserUpdateDto
            {
                FirstName = "Güncel",
                LastName = "Kullanıcı",
                Email = "guncel@test.com"
            };

            _service.Update(user.Id, dto);

            Assert.Equal("Güncel", user.FirstName);
            Assert.Equal("Kullanıcı", user.LastName);
            Assert.Equal("guncel@test.com", user.Email);

            _repositoryMock.Verify(
                x => x.Update(user),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenUserDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((User?)null);

            var dto = new UserUpdateDto
            {
                FirstName = "Güncel",
                LastName = "Kullanıcı",
                Email = "guncel@test.com"
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenUserExists()
        {
            var user = CreateUser();

            _repositoryMock
                .Setup(x => x.GetById(user.Id))
                .Returns(user);

            _service.Delete(user.Id);

            _repositoryMock.Verify(
                x => x.Delete(user),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenUserDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((User?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}