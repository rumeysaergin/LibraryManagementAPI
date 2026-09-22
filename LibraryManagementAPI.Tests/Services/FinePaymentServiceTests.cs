using LibraryManagementAPI.Application.DTOs.FinePayment;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class FinePaymentServiceTests
    {
        private readonly Mock<IFinePaymentRepository> _repositoryMock;
        private readonly FinePaymentService _service;

        public FinePaymentServiceTests()
        {
            _repositoryMock = new Mock<IFinePaymentRepository>();
            _service = new FinePaymentService(_repositoryMock.Object);
        }

        private FinePayment CreateFinePayment()
        {
            return new FinePayment
            {
                Id = Guid.NewGuid(),
                FineId = Guid.NewGuid(),
                Amount = 40,
                PaymentDate = new DateTime(2026, 9, 24),
                PaymentMethod = "CreditCard",
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnFinePayments()
        {
            var payments = new List<FinePayment>
            {
                CreateFinePayment(),
                CreateFinePayment()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(payments);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnFinePayment_WhenPaymentExists()
        {
            var payment = CreateFinePayment();

            _repositoryMock
                .Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            var result = _service.GetById(payment.Id);

            Assert.NotNull(result);
            Assert.Equal(payment.Id, result!.Id);
            Assert.Equal(payment.FineId, result.FineId);
            Assert.Equal(payment.Amount, result.Amount);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenPaymentDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((FinePayment?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByFineId_ShouldReturnFinePayments()
        {
            var fineId = Guid.NewGuid();

            var payment1 = CreateFinePayment();
            payment1.FineId = fineId;

            var payment2 = CreateFinePayment();
            payment2.FineId = fineId;

            var payments = new List<FinePayment>
            {
                payment1,
                payment2
            };

            _repositoryMock
                .Setup(x => x.GetByFineId(fineId))
                .Returns(payments);

            var result = _service.GetByFineId(fineId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(fineId, x.FineId));
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new FinePaymentCreateDto
            {
                FineId = Guid.NewGuid(),
                Amount = 40,
                PaymentDate = new DateTime(2026, 9, 24),
                PaymentMethod = "CreditCard"
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<FinePayment>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateFinePayment_WhenPaymentExists()
        {
            var payment = CreateFinePayment();

            _repositoryMock
                .Setup(x => x.GetById(payment.Id))
                .Returns(payment);

            var dto = new FinePaymentUpdateDto
            {
                Amount = 50,
                PaymentDate = new DateTime(2026, 9, 25),
                PaymentMethod = "Cash"
            };

            _service.Update(payment.Id, dto);

            Assert.Equal(50, payment.Amount);
            Assert.Equal(new DateTime(2026, 9, 25), payment.PaymentDate);
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
                .Returns((FinePayment?)null);

            var dto = new FinePaymentUpdateDto
            {
                Amount = 50,
                PaymentDate = new DateTime(2026, 9, 25),
                PaymentMethod = "Cash"
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenPaymentExists()
        {
            var payment = CreateFinePayment();

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
                .Returns((FinePayment?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}