using LibraryManagementAPI.Application.DTOs.Loan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class LoanServiceTests
    {
        private readonly Mock<ILoanRepository> _loanRepositoryMock;
        private readonly Mock<ILibraryBookRepository> _libraryBookRepositoryMock;
        private readonly Mock<IFineService> _fineServiceMock;
        private readonly LoanService _service;

        public LoanServiceTests()
        {
            _loanRepositoryMock = new Mock<ILoanRepository>();
            _libraryBookRepositoryMock = new Mock<ILibraryBookRepository>();
            _fineServiceMock = new Mock<IFineService>();

            _service = new LoanService(
                _loanRepositoryMock.Object,
                _libraryBookRepositoryMock.Object,
                _fineServiceMock.Object);
        }

        private Loan CreateLoan()
        {
            return new Loan
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                LibraryBookId = Guid.NewGuid(),
                BorrowDate = new DateTime(2026, 9, 15),
                DueDate = new DateTime(2026, 9, 20),
                ReturnDate = null,
                Status = "Borrowed",
                IsDeleted = false
            };
        }

        private LibraryBook CreateLibraryBook()
        {
            return new LibraryBook
            {
                Id = Guid.NewGuid(),
                LibraryId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                Quantity = 5,
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnLoans()
        {
            var loans = new List<Loan>
            {
                CreateLoan(),
                CreateLoan()
            };

            _loanRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(loans);

            var result = _service.GetAll();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnLoan_WhenLoanExists()
        {
            var loan = CreateLoan();

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            var result = _service.GetById(loan.Id);

            Assert.NotNull(result);
            Assert.Equal(loan.Id, result.Id);
            Assert.Equal(loan.UserId, result.UserId);
            Assert.Equal(loan.LibraryBookId, result.LibraryBookId);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenLoanDoesNotExist()
        {
            var id = Guid.NewGuid();

            _loanRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Loan?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByUserId_ShouldReturnLoans()
        {
            var userId = Guid.NewGuid();

            var loans = new List<Loan>
            {
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LibraryBookId = Guid.NewGuid(),
                    BorrowDate = new DateTime(2026, 9, 15),
                    DueDate = new DateTime(2026, 9, 20),
                    Status = "Borrowed",
                    IsDeleted = false
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LibraryBookId = Guid.NewGuid(),
                    BorrowDate = new DateTime(2026, 9, 16),
                    DueDate = new DateTime(2026, 9, 21),
                    Status = "Borrowed",
                    IsDeleted = false
                }
            };

            _loanRepositoryMock
                .Setup(x => x.GetByUserId(userId))
                .Returns(loans);

            var result = _service.GetByUserId(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(userId, x.UserId));
        }

        [Fact]
        public void GetByLibraryBookId_ShouldReturnLoans()
        {
            var libraryBookId = Guid.NewGuid();

            var loans = new List<Loan>
            {
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    LibraryBookId = libraryBookId,
                    BorrowDate = new DateTime(2026, 9, 15),
                    DueDate = new DateTime(2026, 9, 20),
                    Status = "Borrowed",
                    IsDeleted = false
                }
            };

            _loanRepositoryMock
                .Setup(x => x.GetByLibraryBookId(libraryBookId))
                .Returns(loans);

            var result = _service.GetByLibraryBookId(libraryBookId);

            Assert.Single(result);
            Assert.Equal(libraryBookId, result[0].LibraryBookId);
        }

        [Fact]
        public void Add_ShouldDecreaseLibraryBookQuantity_AndAddLoan()
        {
            var libraryBook = CreateLibraryBook();

            var dto = new LoanCreateDto
            {
                UserId = Guid.NewGuid(),
                LibraryBookId = libraryBook.Id,
                BorrowDate = new DateTime(2026, 9, 15),
                DueDate = new DateTime(2026, 9, 20)
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(dto.LibraryBookId))
                .Returns(libraryBook);

            _service.Add(dto);

            Assert.Equal(4, libraryBook.Quantity);

            _libraryBookRepositoryMock.Verify(
                x => x.Update(libraryBook),
                Times.Once);

            _loanRepositoryMock.Verify(
                x => x.Add(It.Is<Loan>(loan =>
                    loan.UserId == dto.UserId &&
                    loan.LibraryBookId == dto.LibraryBookId &&
                    loan.BorrowDate == dto.BorrowDate &&
                    loan.DueDate == dto.DueDate &&
                    loan.Status == "Borrowed" &&
                    loan.IsDeleted == false)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenLibraryBookDoesNotExist()
        {
            var libraryBookId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = Guid.NewGuid(),
                LibraryBookId = libraryBookId,
                BorrowDate = new DateTime(2026, 9, 15),
                DueDate = new DateTime(2026, 9, 20)
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(libraryBookId))
                .Returns((LibraryBook?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);

            _loanRepositoryMock.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenLibraryBookIsOutOfStock()
        {
            var libraryBook = CreateLibraryBook();
            libraryBook.Quantity = 0;

            var dto = new LoanCreateDto
            {
                UserId = Guid.NewGuid(),
                LibraryBookId = libraryBook.Id,
                BorrowDate = new DateTime(2026, 9, 15),
                DueDate = new DateTime(2026, 9, 20)
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(libraryBook.Id))
                .Returns(libraryBook);

            Assert.Throws<InvalidOperationException>(
                () => _service.Add(dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);

            _loanRepositoryMock.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Never);
        }

        [Fact]
        public void Update_ShouldUpdateLoan_WhenLoanExists()
        {
            var loan = CreateLoan();

            var dto = new LoanUpdateDto
            {
                DueDate = new DateTime(2026, 9, 25),
                ReturnDate = null,
                Status = "Extended"
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _service.Update(loan.Id, dto);

            Assert.Equal(dto.DueDate, loan.DueDate);
            Assert.Equal(dto.ReturnDate, loan.ReturnDate);
            Assert.Equal(dto.Status, loan.Status);

            _loanRepositoryMock.Verify(
                x => x.Update(loan),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenLoanDoesNotExist()
        {
            var id = Guid.NewGuid();

            var dto = new LoanUpdateDto
            {
                DueDate = new DateTime(2026, 9, 25),
                ReturnDate = null,
                Status = "Extended"
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Loan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));

            _loanRepositoryMock.Verify(
                x => x.Update(It.IsAny<Loan>()),
                Times.Never);
        }

        [Fact]
        public void Update_ShouldIncreaseLibraryBookQuantity_WhenBookIsReturnedForTheFirstTime()
        {
            var loan = CreateLoan();
            var libraryBook = CreateLibraryBook();

            libraryBook.Id = loan.LibraryBookId;
            libraryBook.Quantity = 4;

            var dto = new LoanUpdateDto
            {
                DueDate = loan.DueDate,
                ReturnDate = new DateTime(2026, 9, 20),
                Status = "Returned"
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(loan.LibraryBookId))
                .Returns(libraryBook);

            _service.Update(loan.Id, dto);

            Assert.Equal(5, libraryBook.Quantity);

            _libraryBookRepositoryMock.Verify(
                x => x.Update(libraryBook),
                Times.Once);

            _loanRepositoryMock.Verify(
                x => x.Update(loan),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldNotIncreaseLibraryBookQuantity_WhenLoanWasAlreadyReturned()
        {
            var loan = CreateLoan();

            loan.ReturnDate = new DateTime(2026, 9, 20);
            loan.Status = "Returned";

            var dto = new LoanUpdateDto
            {
                DueDate = loan.DueDate,
                ReturnDate = new DateTime(2026, 9, 21),
                Status = "Returned"
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _service.Update(loan.Id, dto);

            _libraryBookRepositoryMock.Verify(
                x => x.GetById(It.IsAny<Guid>()),
                Times.Never);

            _libraryBookRepositoryMock.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);

            _loanRepositoryMock.Verify(
                x => x.Update(loan),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldCreateFine_WhenReturnDateIsAfterDueDate()
        {
            var loan = CreateLoan();
            var libraryBook = CreateLibraryBook();

            libraryBook.Id = loan.LibraryBookId;
            libraryBook.Quantity = 4;

            var dto = new LoanUpdateDto
            {
                DueDate = loan.DueDate,
                ReturnDate = new DateTime(2026, 9, 24),
                Status = "Returned"
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(loan.LibraryBookId))
                .Returns(libraryBook);

            _service.Update(loan.Id, dto);

            _fineServiceMock.Verify(
                x => x.CreateFineForLoan(loan.Id),
                Times.Once);

            Assert.Equal(5, libraryBook.Quantity);

            _libraryBookRepositoryMock.Verify(
                x => x.Update(libraryBook),
                Times.Once);

            _loanRepositoryMock.Verify(
                x => x.Update(loan),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldNotCreateFine_WhenReturnDateIsNotAfterDueDate()
        {
            var loan = CreateLoan();
            var libraryBook = CreateLibraryBook();

            libraryBook.Id = loan.LibraryBookId;
            libraryBook.Quantity = 4;

            var dto = new LoanUpdateDto
            {
                DueDate = loan.DueDate,
                ReturnDate = new DateTime(2026, 9, 20),
                Status = "Returned"
            };

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(loan.LibraryBookId))
                .Returns(libraryBook);

            _service.Update(loan.Id, dto);

            _fineServiceMock.Verify(
                x => x.CreateFineForLoan(It.IsAny<Guid>()),
                Times.Never);

            Assert.Equal(5, libraryBook.Quantity);

            _libraryBookRepositoryMock.Verify(
                x => x.Update(libraryBook),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenLoanExists()
        {
            var loan = CreateLoan();

            _loanRepositoryMock
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _service.Delete(loan.Id);

            _loanRepositoryMock.Verify(
                x => x.Delete(loan),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenLoanDoesNotExist()
        {
            var id = Guid.NewGuid();

            _loanRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Loan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));

            _loanRepositoryMock.Verify(
                x => x.Delete(It.IsAny<Loan>()),
                Times.Never);
        }
    }
}