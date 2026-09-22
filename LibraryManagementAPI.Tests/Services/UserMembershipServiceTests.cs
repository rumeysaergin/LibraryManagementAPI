using LibraryManagementAPI.Application.DTOs.UserMembership;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class UserMembershipServiceTests
    {
        private readonly Mock<IUserMembershipRepository> _repositoryMock;
        private readonly UserMembershipService _service;

        public UserMembershipServiceTests()
        {
            _repositoryMock = new Mock<IUserMembershipRepository>();
            _service = new UserMembershipService(_repositoryMock.Object);
        }

        private UserMembership CreateUserMembership()
        {
            return new UserMembership
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                MembershipPlanId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 10, 1),
                IsActive = true,
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnUserMemberships()
        {
            var memberships = new List<UserMembership>
            {
                CreateUserMembership(),
                CreateUserMembership()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(memberships);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnUserMembership_WhenMembershipExists()
        {
            var membership = CreateUserMembership();

            _repositoryMock
                .Setup(x => x.GetById(membership.Id))
                .Returns(membership);

            var result = _service.GetById(membership.Id);

            Assert.NotNull(result);
            Assert.Equal(membership.Id, result!.Id);
            Assert.Equal(membership.UserId, result.UserId);
            Assert.Equal(membership.MembershipPlanId, result.MembershipPlanId);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenMembershipDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((UserMembership?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByUserId_ShouldReturnUserMemberships()
        {
            var userId = Guid.NewGuid();

            var membership1 = CreateUserMembership();
            membership1.UserId = userId;

            var membership2 = CreateUserMembership();
            membership2.UserId = userId;

            var memberships = new List<UserMembership>
            {
                membership1,
                membership2
            };

            _repositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(memberships);

            var result = _service.GetByUserId(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(userId, x.UserId));
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new UserMembershipCreateDto
            {
                UserId = Guid.NewGuid(),
                MembershipPlanId = Guid.NewGuid(),
                StartDate = new DateTime(2026, 9, 1),
                EndDate = new DateTime(2026, 10, 1),
                IsActive = true
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateUserMembership_WhenMembershipExists()
        {
            var membership = CreateUserMembership();

            _repositoryMock
                .Setup(x => x.GetById(membership.Id))
                .Returns(membership);

            var dto = new UserMembershipUpdateDto
            {
                EndDate = new DateTime(2026, 11, 1),
                IsActive = false
            };

            _service.Update(membership.Id, dto);

            Assert.Equal(new DateTime(2026, 11, 1), membership.EndDate);
            Assert.False(membership.IsActive);

            _repositoryMock.Verify(
                x => x.Update(membership),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenMembershipDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((UserMembership?)null);

            var dto = new UserMembershipUpdateDto
            {
                EndDate = new DateTime(2026, 11, 1),
                IsActive = false
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenMembershipExists()
        {
            var membership = CreateUserMembership();

            _repositoryMock
                .Setup(x => x.GetById(membership.Id))
                .Returns(membership);

            _service.Delete(membership.Id);

            _repositoryMock.Verify(
                x => x.Delete(membership),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenMembershipDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((UserMembership?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}