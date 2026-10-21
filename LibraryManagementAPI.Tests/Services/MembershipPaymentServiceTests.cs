
using LibraryManagementAPI.Application.DTOs.MembershipPayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

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

        private static MembershipPayment CreateMembershipPayment()
        {
            return new MembershipPayment
            {
                Id = Guid.NewGuid(),
                UserMembershipId = Guid.NewGuid(),
                Amount = 100,
                PaymentDate = new DateTime(2026, 9, 1),
                PaymentMethod = "CreditCard",
                IsPaid = false,
                IsDeleted = false
            };
        }

        private static MembershipPlan CreateMembershipPlan(Guid? id = null)
        {
            return new MembershipPlan
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Premium",
                MaxBorrowLimit = 10,
                Price = 100,
                DurationInDays = 30,
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnMembershipPayments()
        {
            var payments = new List<MembershipPayment>
            {
                CreateMembershipPayment(),
                CreateMembershipPayment()
            };

            _repositoryMock.Setup(x => x.GetAll()).Returns(payments);

            var result = _service.GetAll();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnPayment_WhenPaymentExists()
        {
            var payment = CreateMembershipPayment();

            _repositoryMock.Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            var result = _service.GetById(payment.Id);

            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.Id);
            Assert.Equal(payment.UserMembershipId, result.UserMembershipId);
            Assert.Equal(payment.Amount, result.Amount);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            Assert.Null(_service.GetById(id));
        }

        [Fact]
        public void GetByUserMembershipId_ShouldReturnPayments()
        {
            var membershipId = Guid.NewGuid();

            var payment1 = CreateMembershipPayment();
            payment1.UserMembershipId = membershipId;

            var payment2 = CreateMembershipPayment();
            payment2.UserMembershipId = membershipId;

            _repositoryMock
                .Setup(x => x.GetByUserMembershipId(membershipId))
                .Returns(new List<MembershipPayment> { payment1, payment2 });

            var result = _service.GetByUserMembershipId(membershipId);

            Assert.Equal(2, result.Count);
            Assert.All(result, x =>
                Assert.Equal(membershipId, x.UserMembershipId));
        }

        [Fact]
        public void GetByUserId_ShouldReturnPaymentsForUsersMemberships()
        {
            var userId = Guid.NewGuid();
            var membershipId1 = Guid.NewGuid();
            var membershipId2 = Guid.NewGuid();

            var memberships = new List<UserMembership>
            {
                new()
                {
                    Id = membershipId1,
                    UserId = userId,
                    IsDeleted = false
                },
                new()
                {
                    Id = membershipId2,
                    UserId = userId,
                    IsDeleted = false
                }
            };

            var payment1 = CreateMembershipPayment();
            payment1.UserMembershipId = membershipId1;

            var payment2 = CreateMembershipPayment();
            payment2.UserMembershipId = membershipId2;

            _userMembershipRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(memberships);

            _repositoryMock
                .Setup(x => x.GetByUserMembershipId(membershipId1))
                .Returns(new List<MembershipPayment> { payment1 });

            _repositoryMock
                .Setup(x => x.GetByUserMembershipId(membershipId2))
                .Returns(new List<MembershipPayment> { payment2 });

            var result = _service.GetByUserId(userId);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Id == payment1.Id);
            Assert.Contains(result, x => x.Id == payment2.Id);
        }

        [Fact]
        public void Add_ShouldCreatePendingPaymentUsingPlanPrice()
        {
            var userId = Guid.NewGuid();
            var plan = CreateMembershipPlan();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = plan.Id,
                PaymentMethod = "CreditCard"
            };

            _membershipPlanRepositoryMock
                .Setup(x => x.GetById(plan.Id))
                .Returns(plan);

            _service.Add(dto, userId);

            _userMembershipRepositoryMock.Verify(
                x => x.Add(It.Is<UserMembership>(m =>
                    m.UserId == userId &&
                    m.MembershipPlanId == plan.Id &&
                    !m.IsActive &&
                    !m.IsDeleted)),
                Times.Once);

            _repositoryMock.Verify(
                x => x.Add(It.Is<MembershipPayment>(p =>
                    p.Amount == plan.Price &&
                    p.PaymentMethod == "CreditCard" &&
                    !p.IsPaid &&
                    !p.IsDeleted)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldThrow_WhenPlanDoesNotExist()
        {
            var planId = Guid.NewGuid();

            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = planId,
                PaymentMethod = "CreditCard"
            };

            _membershipPlanRepositoryMock
                .Setup(x => x.GetById(planId))
                .Returns((MembershipPlan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto, Guid.NewGuid()));

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
            var dto = new MembershipPaymentCreateDto
            {
                MembershipPlanId = Guid.NewGuid(),
                PaymentMethod = "CreditCard"
            };

            Assert.Throws<UnauthorizedAccessException>(
                () => _service.Add(dto, Guid.Empty));
        }

        [Fact]
        public void MarkAsPaid_ShouldActivateMembershipAndSetDates()
        {
            var plan = CreateMembershipPlan();
            var membership = new UserMembership
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                MembershipPlanId = plan.Id,
                IsActive = false,
                IsDeleted = false
            };

            var payment = CreateMembershipPayment();
            payment.UserMembershipId = membership.Id;

            _repositoryMock.Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            _userMembershipRepositoryMock
                .Setup(x => x.GetById(membership.Id))
                .Returns(membership);

            _membershipPlanRepositoryMock
                .Setup(x => x.GetById(plan.Id))
                .Returns(plan);

            _service.MarkAsPaid(payment.Id);

            Assert.True(payment.IsPaid);
            Assert.True(membership.IsActive);
            Assert.Equal(
                membership.StartDate.AddDays(plan.DurationInDays),
                membership.EndDate);

            _repositoryMock.Verify(
                x => x.Update(payment),
                Times.Once);

            _userMembershipRepositoryMock.Verify(
                x => x.Update(membership),
                Times.Once);
        }

        [Fact]
        public void MarkAsPaid_ShouldThrow_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.MarkAsPaid(id));
        }

        [Fact]
        public void Update_ShouldThrow_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            var dto = new MembershipPaymentUpdateDto
            {
                Amount = 150,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash"
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepository_WhenPaymentIsPending()
        {
            var payment = CreateMembershipPayment();

            _repositoryMock.Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            _service.Delete(payment.Id);

            _repositoryMock.Verify(
                x => x.Delete(payment),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrow_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}
