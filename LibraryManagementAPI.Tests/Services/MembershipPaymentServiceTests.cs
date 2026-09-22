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
        private readonly MembershipPaymentService _service;

        public MembershipPaymentServiceTests()
        {
            _repositoryMock = new Mock<IMembershipPaymentRepository>();
            _service = new MembershipPaymentService(_repositoryMock.Object);
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

            var payments = new List<MembershipPayment>
            {
                payment1,
                payment2
            };

            _repositoryMock
                .Setup(x => x.GetByUserMembershipId(userMembershipId))
                .Returns(payments);

            var result = _service.GetByUserMembershipId(userMembershipId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(
                result,
                x => Assert.Equal(userMembershipId, x.UserMembershipId));
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new MembershipPaymentCreateDto
            {
                UserMembershipId = Guid.NewGuid(),
                Amount = 100,
                PaymentDate = new DateTime(2026, 9, 1),
                PaymentMethod = "CreditCard"
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<MembershipPayment>()),
                Times.Once);
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