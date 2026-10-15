using LibraryManagementAPI.Application.DTOs.Fine;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class FineServiceTests
    {
        private readonly Mock<IFineRepository> _fineRepositoryMock;
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly FineService _service;

        public FineServiceTests()
        {
            _fineRepositoryMock = new Mock<IFineRepository>();
            _loanRepositoryMock = new Mock<ILoanRepository>();

            _service = new FineService(
                _fineRepositoryMock.Object,
                _loanRepositoryMock.Object);
        }

        private Loan CreateLoan()
        {
            return new Loan
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                LibraryBookId = Guid.NewGuid(),
                BorrowDate = new DateTime(2026, 9, 10),
                DueDate = new DateTime(2026, 9, 20),
                ReturnDate = new DateTime(2026, 9, 25),
                Status = "Returned",
                IsDeleted = false
            };
        }

        private Fine CreateFine()
        {
            return new Fine
            {
                Id = Guid.NewGuid(),
                LoanId = Guid.NewGuid(),
                Amount = 50,
                FineDate = new DateTime(2026, 9, 25),
                IsPaid = false,
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnFines()
        {
            var fines = new List<Fine>
            {
                CreateFine(),
                CreateFine()
            };

            _fineRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(fines);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnFine_WhenFineExists()
        {
            var fine = CreateFine();

            _fineRepositoryMock
                .Setup(x => x.GetById(fine.Id))
                .Returns(fine);

            var result = _service.GetById(fine.Id);

            Assert.NotNull(result);
            Assert.Equal(fine.Id, result.Id);
            Assert.Equal(fine.LoanId, result.LoanId);
            Assert.Equal(fine.Amount, result.Amount);
            Assert.Equal(fine.IsPaid, result.IsPaid);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenFineDoesNotExist()
        {
            var id = Guid.NewGuid();

            _fineRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Fine?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByLoanId_ShouldReturnFines()
        {
            var loanId = Guid.NewGuid();

            var fine1 = CreateFine();
            fine1.LoanId = loanId;

            var fine2 = CreateFine();
            fine2.LoanId = loanId;

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loanId))
                .Returns(new List<Fine>
                {
                    fine1,
                    fine2
                });

            var result = _service.GetByLoanId(loanId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(
                result,
                x => Assert.Equal(loanId, x.LoanId));
        }

        [Fact]
        public void Add_ShouldCreateFineForLoan()
        {
            var loan = CreateLoan();

            var dto = new FineCreateDto
            {
                LoanId = loan.Id
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loan.Id))
                .Returns(new List<Fine>());

            _service.Add(dto);

            _fineRepositoryMock.Verify(
                x => x.Add(It.Is<Fine>(fine =>
                    fine.LoanId == loan.Id &&
                    fine.Amount == 50 &&
                    fine.IsPaid == false &&
                    fine.IsDeleted == false)),
                Times.Once);
        }

        [Fact]
        public void CreateFineForLoan_ShouldCalculateFineBasedOnLateDays()
        {
            var loan = CreateLoan();

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loan.Id))
                .Returns(new List<Fine>());

            _service.CreateFineForLoan(loan.Id);

            // 20 Eylül -> 25 Eylül = 5 gün
            // 5 * 10 TL = 50 TL
            _fineRepositoryMock.Verify(
                x => x.Add(It.Is<Fine>(fine =>
                    fine.LoanId == loan.Id &&
                    fine.Amount == 50 &&
                    fine.IsPaid == false &&
                    fine.IsDeleted == false)),
                Times.Once);
        }

        [Fact]
        public void CreateFineForLoan_ShouldThrowException_WhenLoanDoesNotExist()
        {
            var loanId = Guid.NewGuid();

            _loanRepositoryMock
                .Setup(x => x.GetById(loanId))
                .Returns((Loan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.CreateFineForLoan(loanId));

            _fineRepositoryMock.Verify(
                x => x.Add(It.IsAny<Fine>()),
                Times.Never);
        }

        [Fact]
        public void CreateFineForLoan_ShouldThrowException_WhenFineAlreadyExists()
        {
            var loan = CreateLoan();
            var existingFine = CreateFine();

            existingFine.LoanId = loan.Id;

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loan.Id))
                .Returns(new List<Fine>
                {
                    existingFine
                });

            Assert.Throws<InvalidOperationException>(
                () => _service.CreateFineForLoan(loan.Id));

            _fineRepositoryMock.Verify(
                x => x.Add(It.IsAny<Fine>()),
                Times.Never);
        }

        [Fact]
        public void CreateFineForLoan_ShouldThrowException_WhenBookHasNotBeenReturned()
        {
            var loan = CreateLoan();

            loan.ReturnDate = null;

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loan.Id))
                .Returns(new List<Fine>());

            Assert.Throws<InvalidOperationException>(
                () => _service.CreateFineForLoan(loan.Id));

            _fineRepositoryMock.Verify(
                x => x.Add(It.IsAny<Fine>()),
                Times.Never);
        }

        [Fact]
        public void CreateFineForLoan_ShouldThrowException_WhenBookReturnedOnTime()
        {
            var loan = CreateLoan();

            loan.ReturnDate = loan.DueDate;

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loan.Id))
                .Returns(new List<Fine>());

            Assert.Throws<InvalidOperationException>(
                () => _service.CreateFineForLoan(loan.Id));

            _fineRepositoryMock.Verify(
                x => x.Add(It.IsAny<Fine>()),
                Times.Never);
        }

        [Fact]
        public void CreateFineForLoan_ShouldCalculateCorrectFine_WhenBookIsReturnedLate()
        {
            var loan = CreateLoan();

            loan.DueDate = new DateTime(2026, 9, 20);
            loan.ReturnDate = new DateTime(2026, 9, 23);

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _fineRepositoryMock
                .Setup(x => x.GetByLoanId(loan.Id))
                .Returns(new List<Fine>());

            _service.CreateFineForLoan(loan.Id);

            // 20 Eylül -> 23 Eylül = 3 gün
            // 3 * 10 TL = 30 TL
            _fineRepositoryMock.Verify(
                x => x.Add(It.Is<Fine>(fine =>
                    fine.LoanId == loan.Id &&
                    fine.Amount == 30 &&
                    fine.IsPaid == false &&
                    fine.IsDeleted == false)),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateFine_WhenFineExists()
        {
            var fine = CreateFine();

            var dto = new FineUpdateDto
            {
                Amount = 100,
                IsPaid = true
            };

            _fineRepositoryMock
                .Setup(x => x.GetById(fine.Id))
                .Returns(fine);

            _service.Update(fine.Id, dto);

            Assert.Equal(100, fine.Amount);
            Assert.True(fine.IsPaid);

            _fineRepositoryMock.Verify(
                x => x.Update(fine),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenFineDoesNotExist()
        {
            var id = Guid.NewGuid();

            var dto = new FineUpdateDto
            {
                Amount = 100,
                IsPaid = true
            };

            _fineRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Fine?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));

            _fineRepositoryMock.Verify(
                x => x.Update(It.IsAny<Fine>()),
                Times.Never);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenFineExists()
        {
            var fine = CreateFine();

            _fineRepositoryMock
                .Setup(x => x.GetById(fine.Id))
                .Returns(fine);

            _service.Delete(fine.Id);

            _fineRepositoryMock.Verify(
                x => x.Delete(fine),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenFineDoesNotExist()
        {
            var id = Guid.NewGuid();

            _fineRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Fine?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));

            _fineRepositoryMock.Verify(
                x => x.Delete(It.IsAny<Fine>()),
                Times.Never);
        }
    }
}