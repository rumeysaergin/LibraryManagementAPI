
using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;
using Xunit;

namespace LibraryManagementAPI.Tests.Services
{
    public class MembershipPaymentServiceTests
    {
        private readonly Mock<IMembershipPaymentRepository> _repositoryMock;
        private readonly Mock<IUserMembershipRepository> _userMembershipRepositoryMock;
        private readonly Mock<IMembershipPlanRepository> _membershipPlanRepositoryMock;
        private readonly MembershipPaymentService _service;

        public MembershipPaymentServiceTests()
        {
            _repositoryMock = new Mock<IMembershipPaymentRepository>();
            _userMembershipRepositoryMock = new Mock<IUserMembershipRepository>();
            _membershipPlanRepositoryMock = new Mock<IMembershipPlanRepository>();

            _service = new MembershipPaymentService(
                _repositoryMock.Object,
                _userMembershipRepositoryMock.Object,
                _membershipPlanRepositoryMock.Object);
        }

        [Fact]
        public void Add_ShouldCreatePendingPaymentUsingPlanPrice()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var plan = CreateMembershipPlan();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = plan.Id,
                PaymentMethod = "CreditCard"
            };

            _userMembershipRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserMembership>());

            _membershipPlanRepositoryMock
                .Setup(x => x.GetById(plan.Id))
                .Returns(plan);

            UserMembership? createdMembership = null;

            _userMembershipRepositoryMock
                .Setup(x => x.Add(It.IsAny<UserMembership>()))
                .Callback<UserMembership>(membership =>
                {
                    createdMembership = membership;
                });

            // Act
            _service.Add(dto, userId);

            // Assert: Üyelik oluşturulmuş olmalı.
            Assert.NotNull(createdMembership);

            Assert.Equal(userId, createdMembership.UserId);
            Assert.Equal(plan.Id, createdMembership.MembershipPlanId);
            Assert.False(createdMembership.IsActive);
            Assert.False(createdMembership.IsDeleted);

            // Assert: Ödeme üyeliğe bağlanmış olmalı.
            var payment = Assert.Single(
                createdMembership.MembershipPayments);

            Assert.Equal(
                createdMembership.Id,
                payment.UserMembershipId);

            Assert.Equal(plan.Price, payment.Amount);
            Assert.Equal("CreditCard", payment.PaymentMethod);
            Assert.False(payment.IsPaid);
            Assert.False(payment.IsDeleted);

            // Üyelik bir kez eklenmeli.
            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Once);

            // Ödeme ayrı repository çağrısıyla eklenmiyor.
            _repositoryMock.Verify(
                x => x.Add(It.IsAny<MembershipPayment>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenPlanDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = planId,
                PaymentMethod = "CreditCard"
            };

            _userMembershipRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserMembership>());

            _membershipPlanRepositoryMock
                .Setup(x => x.GetById(planId))
                .Returns((MembershipPlan?)null);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto, userId));

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Never);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<MembershipPayment>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenUserIdIsEmpty()
        {
            // Arrange
            var plan = CreateMembershipPlan();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = plan.Id,
                PaymentMethod = "CreditCard"
            };

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(
                () => _service.Add(dto, Guid.Empty));

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenMembershipPlanIdIsEmpty()
        {
            // Arrange
            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = Guid.Empty,
                PaymentMethod = "CreditCard"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => _service.Add(dto, Guid.NewGuid()));

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenPaymentMethodIsEmpty()
        {
            // Arrange
            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = Guid.NewGuid(),
                PaymentMethod = " "
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => _service.Add(dto, Guid.NewGuid()));

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenUserHasActiveMembership()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var plan = CreateMembershipPlan();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = plan.Id,
                PaymentMethod = "CreditCard"
            };

            _userMembershipRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserMembership>
                {
                    new UserMembership
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        MembershipPlanId = plan.Id,
                        StartDate = DateTime.UtcNow.AddDays(-5),
                        EndDate = DateTime.UtcNow.AddDays(25),
                        IsActive = true,
                        IsDeleted = false
                    }
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _service.Add(dto, userId));

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrow_WhenUserHasPendingPayment()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var plan = CreateMembershipPlan();
            var membershipId = Guid.NewGuid();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = plan.Id,
                PaymentMethod = "CreditCard"
            };

            var existingMembership = new UserMembership
            {
                Id = membershipId,
                UserId = userId,
                MembershipPlanId = plan.Id,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                IsActive = false,
                IsDeleted = false
            };

            _userMembershipRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserMembership>
                {
                    existingMembership
                });

            _repositoryMock
                .Setup(x => x.GetByUserMembershipId(membershipId))
                .Returns(new List<MembershipPayment>
                {
                    new MembershipPayment
                    {
                        Id = Guid.NewGuid(),
                        UserMembershipId = membershipId,
                        Amount = plan.Price,
                        PaymentMethod = "CreditCard",
                        PaymentDate = DateTime.UtcNow,
                        IsPaid = false,
                        IsDeleted = false
                    }
                });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _service.Add(dto, userId));

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.IsAny<UserMembership>()),
                Times.Never);
        }

        private static MembershipPlan CreateMembershipPlan()
        {
            return new MembershipPlan
            {
                Id = Guid.NewGuid(),
                Price = 100,
                DurationInDays = 30,
                MaxBorrowLimit = 3
            };
        }
    }
}
