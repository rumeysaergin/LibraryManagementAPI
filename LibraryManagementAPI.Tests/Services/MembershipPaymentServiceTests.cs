
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
        private readonly MembershipPaymentService _service;

        public MembershipPaymentServiceTests()
        {
            _repositoryMock = new Mock<IMembershipPaymentRepository>();
            _userMembershipRepositoryMock = new Mock<IUserMembershipRepository>();

            _service = new MembershipPaymentService(
                _repositoryMock.Object,
                _userMembershipRepositoryMock.Object);
        }

        private MembershipPayment CreateMembershipPayment()
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

        [Fact]
        public void GetAll_ShouldReturnMembershipPayments()
        {
            var payments = new List<MembershipPayment>
            {
                CreateMembershipPayment(),
                CreateMembershipPayment()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(payments);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnMembershipPayment_WhenPaymentExists()
        {
            var payment = CreateMembershipPayment();

            _repositoryMock
                .Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            var result = _service.GetById(payment.Id);

            Assert.NotNull(result);
            Assert.Equal(payment.Id, result!.Id);
            Assert.Equal(payment.UserMembershipId, result.UserMembershipId);
            Assert.Equal(payment.Amount, result.Amount);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByUserMembershipId_ShouldReturnMembershipPayments()
        {
            var userMembershipId = Guid.NewGuid();

            var payment1 = CreateMembershipPayment();
            payment1.UserMembershipId = userMembershipId;

            var payment2 = CreateMembershipPayment();
            payment2.UserMembershipId = userMembershipId;

            _repositoryMock
                .Setup(x => x.GetByUserMembershipId(userMembershipId))
                .Returns(new List<MembershipPayment> { payment1, payment2 });

            var result = _service.GetByUserMembershipId(userMembershipId);

            Assert.Equal(2, result.Count);
            Assert.All(
                result,
                x => Assert.Equal(userMembershipId, x.UserMembershipId));
        }

        [Fact]
        public void GetByUserId_ShouldReturnOnlyPaymentsForUsersMemberships()
        {
            var userId = Guid.NewGuid();
            var membershipId1 = Guid.NewGuid();
            var membershipId2 = Guid.NewGuid();

            var memberships = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = membershipId1,
                    UserId = userId,
                    IsDeleted = false
                },
                new UserMembership
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
        public void Add_ShouldCallRepositoryAdd_WhenMembershipBelongsToUser()
        {
            var userId = Guid.NewGuid();
            var membershipId = Guid.NewGuid();

            var membership = new UserMembership
            {
                Id = membershipId,
                UserId = userId,
                IsDeleted = false
            };

            var dto = new MembershipPaymentCreateDto
            {
                UserMembershipId = membershipId,
                Amount = 100,
                PaymentDate = new DateTime(2026, 9, 1),
                PaymentMethod = "CreditCard"
            };

            _userMembershipRepositoryMock
                .Setup(x => x.GetById(membershipId))
                .Returns(membership);

            _service.Add(dto, userId);

            _repositoryMock.Verify(
                x => x.Add(It.Is<MembershipPayment>(p =>
                    p.UserMembershipId == membershipId &&
                    p.Amount == 100 &&
                    p.IsPaid == false)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldThrow_WhenMembershipBelongsToAnotherUser()
        {
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var membershipId = Guid.NewGuid();

            var membership = new UserMembership
            {
                Id = membershipId,
                UserId = anotherUserId,
                IsDeleted = false
            };

            var dto = new MembershipPaymentCreateDto
            {
                UserMembershipId = membershipId,
                Amount = 100,
                PaymentMethod = "CreditCard"
            };

            _userMembershipRepositoryMock
                .Setup(x => x.GetById(membershipId))
                .Returns(membership);

            Assert.Throws<UnauthorizedAccessException>(
                () => _service.Add(dto, userId));

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<MembershipPayment>()),
                Times.Never);
        }

        [Fact]
        public void Update_ShouldUpdateMembershipPayment_WhenPaymentExists()
        {
            var payment = CreateMembershipPayment();

            _repositoryMock
                .Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            var dto = new MembershipPaymentUpdateDto
            {
                Amount = 150,
                PaymentDate = new DateTime(2026, 9, 5),
                PaymentMethod = "Cash"
            };

            _service.Update(payment.Id, dto);

            Assert.Equal(150, payment.Amount);
            Assert.Equal(new DateTime(2026, 9, 5), payment.PaymentDate);
            Assert.Equal("Cash", payment.PaymentMethod);

            _repositoryMock.Verify(
                x => x.Update(payment),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            var dto = new MembershipPaymentUpdateDto
            {
                Amount = 150,
                PaymentDate = new DateTime(2026, 9, 5),
                PaymentMethod = "Cash"
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenPaymentExists()
        {
            var payment = CreateMembershipPayment();

            _repositoryMock
                .Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            _service.Delete(payment.Id);

            _repositoryMock.Verify(
                x => x.Delete(payment),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((MembershipPayment?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}
