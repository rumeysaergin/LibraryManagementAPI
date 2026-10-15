
using LibraryManagementAPI.Application.DTOs.Loan;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class LoanServiceTest
    {
        private readonly Mock<ILoanRepository> _loanRepository;
        private readonly Mock<ILibraryBookRepository> _libraryBookRepository;
        private readonly Mock<IUserMembershipRepository> _userMembershipRepository;
        private readonly Mock<IMembershipPlanRepository> _membershipPlanRepository;
        private readonly Mock<IMembershipPaymentRepository> _membershipPaymentRepository;
        private readonly Mock<IUserLibraryRepository> _userLibraryRepository;
        private readonly Mock<IFineService> _fineService;

        private readonly LoanService _loanService;

        public LoanServiceTest()
        {
            _loanRepository = new Mock<ILoanRepository>();
            _libraryBookRepository = new Mock<ILibraryBookRepository>();
            _userMembershipRepository = new Mock<IUserMembershipRepository>();
            _membershipPlanRepository = new Mock<IMembershipPlanRepository>();
            _membershipPaymentRepository = new Mock<IMembershipPaymentRepository>();
            _userLibraryRepository = new Mock<IUserLibraryRepository>();
            _fineService = new Mock<IFineService>();

            _loanService = new LoanService(
                _loanRepository.Object,
                _libraryBookRepository.Object,
                _userMembershipRepository.Object,
                _membershipPlanRepository.Object,
                _membershipPaymentRepository.Object,
                _userLibraryRepository.Object,
                _fineService.Object);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenLibraryBookNotFound()
        {
            var dto = new LoanCreateDto
            {
                UserId = Guid.NewGuid(),
                LibraryBookId = Guid.NewGuid()
            };

            _userMembershipRepository
                .Setup(x => x.GetByUserId(dto.UserId))
                .Returns(new List<UserMembership>());

            Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));
        }

        [Fact]
        public void Add_ShouldThrowException_WhenBookIsOutOfStock()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 0,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, Guid.NewGuid());

            SetupMembershipAndPayment(userId, membership, isPaid: true);
            SetupUserLibraryMembership(userId, libraryId);

            _libraryBookRepository
                .Setup(x => x.GetById(libraryBookId))
                .Returns(libraryBook);

            Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));
        }

        [Fact]
        public void Add_ShouldThrowException_WhenUserHasNoActiveMembership()
        {
            var userId = Guid.NewGuid();
            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = Guid.NewGuid()
            };

            _userMembershipRepository
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserMembership>());

            Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));
        }

        [Fact]
        public void Add_ShouldThrowException_WhenMembershipPlanNotFound()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var membershipPlanId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 5,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, membershipPlanId);

            SetupMembershipAndPayment(userId, membership, isPaid: true);
            SetupUserLibraryMembership(userId, libraryId);

            _libraryBookRepository
                .Setup(x => x.GetById(libraryBookId))
                .Returns(libraryBook);

            _membershipPlanRepository
                .Setup(x => x.GetById(membershipPlanId))
                .Returns((MembershipPlan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _loanService.Add(dto));
        }

        [Fact]
        public void Add_ShouldThrowException_WhenPaymentIsNotCompleted()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var membershipPlanId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var membership = CreateActiveMembership(
                userId, membershipPlanId);

            SetupMembershipAndPayment(userId, membership, isPaid: false);

            var exception = Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));

            Assert.Contains(
                "payment",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            _loanRepository.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Never);

            _libraryBookRepository.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenUserIsNotMemberOfLibrary()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 5,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, Guid.NewGuid());

            SetupMembershipAndPayment(userId, membership, isPaid: true);

            _libraryBookRepository
                .Setup(x => x.GetById(libraryBookId))
                .Returns(libraryBook);

            _userLibraryRepository
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserLibrary>());

            var exception = Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));

            Assert.Contains(
                "not a member",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            _loanRepository.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Never);

            _libraryBookRepository.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenBorrowLimitIsReached()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var membershipPlanId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 5,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, membershipPlanId);

            var membershipPlan = new MembershipPlan
            {
                Id = membershipPlanId,
                MaxBorrowLimit = 3,
                DurationInDays = 15,
                IsDeleted = false
            };

            var activeLoans = new List<Loan>
            {
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                }
            };

            SetupBorrowDependencies(
                userId, libraryBook, membership,
                membershipPlan, activeLoans);

            var exception = Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));

            Assert.Contains(
                "borrowing limit",
                exception.Message,
                StringComparison.OrdinalIgnoreCase);

            _loanRepository.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Never);

            _libraryBookRepository.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldCreateLoanAndCalculateDates_WhenBorrowLimitIsNotReached()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var membershipPlanId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 5,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, membershipPlanId);

            var membershipPlan = new MembershipPlan
            {
                Id = membershipPlanId,
                MaxBorrowLimit = 3,
                DurationInDays = 15,
                IsDeleted = false
            };

            var activeLoans = new List<Loan>
            {
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                }
            };

            SetupBorrowDependencies(
                userId, libraryBook, membership,
                membershipPlan, activeLoans);

            var before = DateTime.Now;

            _loanService.Add(dto);

            var after = DateTime.Now;

            _loanRepository.Verify(
                x => x.Add(It.Is<Loan>(loan =>
                    loan.UserId == userId &&
                    loan.LibraryBookId == libraryBookId &&
                    loan.Status == "Borrowed" &&
                    loan.ReturnDate == null &&
                    !loan.IsDeleted &&
                    loan.BorrowDate >= before &&
                    loan.BorrowDate <= after &&
                    loan.DueDate == loan.BorrowDate.AddDays(15))),
                Times.Once);

            Assert.Equal(4, libraryBook.Quantity);

            _libraryBookRepository.Verify(
                x => x.Update(It.Is<LibraryBook>(book =>
                    book.Id == libraryBookId &&
                    book.Quantity == 4)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldNotCountReturnedLoansInBorrowLimit()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var membershipPlanId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 5,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, membershipPlanId);

            var membershipPlan = new MembershipPlan
            {
                Id = membershipPlanId,
                MaxBorrowLimit = 3,
                DurationInDays = 15,
                IsDeleted = false
            };

            var loans = new List<Loan>
            {
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = null,
                    Status = "Borrowed",
                    IsDeleted = false
                },
                new Loan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ReturnDate = DateTime.Now.AddDays(-2),
                    Status = "Returned",
                    IsDeleted = false
                }
            };

            SetupBorrowDependencies(
                userId, libraryBook, membership,
                membershipPlan, loans);

            _loanService.Add(dto);

            _loanRepository.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Once);

            _libraryBookRepository.Verify(
                x => x.Update(It.Is<LibraryBook>(book =>
                    book.Quantity == 4)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenMembershipDurationIsInvalid()
        {
            var userId = Guid.NewGuid();
            var libraryBookId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var membershipPlanId = Guid.NewGuid();

            var dto = new LoanCreateDto
            {
                UserId = userId,
                LibraryBookId = libraryBookId
            };

            var libraryBook = new LibraryBook
            {
                Id = libraryBookId,
                LibraryId = libraryId,
                Quantity = 5,
                IsDeleted = false
            };

            var membership = CreateActiveMembership(
                userId, membershipPlanId);

            var membershipPlan = new MembershipPlan
            {
                Id = membershipPlanId,
                MaxBorrowLimit = 3,
                DurationInDays = 0,
                IsDeleted = false
            };

            SetupBorrowDependencies(
                userId,
                libraryBook,
                membership,
                membershipPlan,
                new List<Loan>());

            Assert.Throws<InvalidOperationException>(
                () => _loanService.Add(dto));

            _loanRepository.Verify(
                x => x.Add(It.IsAny<Loan>()),
                Times.Never);
        }

        [Fact]
        public void ReturnLoan_ShouldThrowException_WhenLoanNotFound()
        {
            var loanId = Guid.NewGuid();

            _loanRepository
                .Setup(x => x.GetById(loanId))
                .Returns((Loan?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _loanService.ReturnLoan(loanId));
        }

        [Fact]
        public void ReturnLoan_ShouldIncreaseStock_WhenLoanIsReturnedOnTime()
        {
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                LibraryBookId = Guid.NewGuid(),
                BorrowDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(10),
                ReturnDate = null,
                Status = "Borrowed",
                IsDeleted = false
            };

            var libraryBook = new LibraryBook
            {
                Id = loan.LibraryBookId,
                Quantity = 2,
                IsDeleted = false
            };

            _loanRepository
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _libraryBookRepository
                .Setup(x => x.GetById(loan.LibraryBookId))
                .Returns(libraryBook);

            _loanService.ReturnLoan(loan.Id);

            Assert.Equal("Returned", loan.Status);
            Assert.NotNull(loan.ReturnDate);
            Assert.Equal(3, libraryBook.Quantity);

            _loanRepository.Verify(
                x => x.Update(It.Is<Loan>(x =>
                    x.Id == loan.Id &&
                    x.ReturnDate.HasValue &&
                    x.Status == "Returned")),
                Times.Once);

            _libraryBookRepository.Verify(
                x => x.Update(It.Is<LibraryBook>(x =>
                    x.Id == libraryBook.Id &&
                    x.Quantity == 3)),
                Times.Once);

            _fineService.Verify(
                x => x.CreateFineForLoan(It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public void ReturnLoan_ShouldCreateFine_WhenLoanIsOverdue()
        {
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                LibraryBookId = Guid.NewGuid(),
                BorrowDate = DateTime.Now.AddDays(-20),
                DueDate = DateTime.Now.AddDays(-5),
                ReturnDate = null,
                Status = "Borrowed",
                IsDeleted = false
            };

            var libraryBook = new LibraryBook
            {
                Id = loan.LibraryBookId,
                Quantity = 2,
                IsDeleted = false
            };

            _loanRepository
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            _libraryBookRepository
                .Setup(x => x.GetById(loan.LibraryBookId))
                .Returns(libraryBook);

            _loanService.ReturnLoan(loan.Id);

            _fineService.Verify(
                x => x.CreateFineForLoan(loan.Id),
                Times.Once);

            Assert.Equal(3, libraryBook.Quantity);
            Assert.Equal("Returned", loan.Status);
        }

        [Fact]
        public void ReturnLoan_ShouldThrowException_WhenLoanAlreadyReturned()
        {
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                LibraryBookId = Guid.NewGuid(),
                BorrowDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(5),
                ReturnDate = DateTime.Now.AddDays(-1),
                Status = "Returned",
                IsDeleted = false
            };

            _loanRepository
                .Setup(x => x.GetById(loan.Id))
                .Returns(loan);

            Assert.Throws<InvalidOperationException>(
                () => _loanService.ReturnLoan(loan.Id));

            _libraryBookRepository.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        private static UserMembership CreateActiveMembership(
            Guid userId,
            Guid membershipPlanId)
        {
            return new UserMembership
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MembershipPlanId = membershipPlanId,
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(20),
                IsActive = true,
                IsDeleted = false
            };
        }

        private void SetupMembershipAndPayment(
            Guid userId,
            UserMembership membership,
            bool isPaid)
        {
            _userMembershipRepository
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserMembership> { membership });

            _membershipPaymentRepository
                .Setup(x => x.GetByUserMembershipId(membership.Id))
                .Returns(new List<MembershipPayment>
                {
                    new MembershipPayment
                    {
                        Id = Guid.NewGuid(),
                        UserMembershipId = membership.Id,
                        Amount = 100,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Test",
                        IsPaid = isPaid,
                        IsDeleted = false
                    }
                });
        }

        private void SetupUserLibraryMembership(
            Guid userId,
            Guid libraryId)
        {
            _userLibraryRepository
                .Setup(x => x.GetByUserId(userId))
                .Returns(new List<UserLibrary>
                {
                    new UserLibrary
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        LibraryId = libraryId,
                        IsDeleted = false
                    }
                });
        }

        private void SetupBorrowDependencies(
            Guid userId,
            LibraryBook libraryBook,
            UserMembership membership,
            MembershipPlan membershipPlan,
            List<Loan> loans)
        {
            _libraryBookRepository
                .Setup(x => x.GetById(libraryBook.Id))
                .Returns(libraryBook);

            SetupMembershipAndPayment(
                userId, membership, isPaid: true);

            SetupUserLibraryMembership(
                userId, libraryBook.LibraryId);

            _membershipPlanRepository
                .Setup(x => x.GetById(membershipPlan.Id))
                .Returns(membershipPlan);

            _loanRepository
                .Setup(x => x.GetByUserId(userId))
                .Returns(loans);
        }
    }
}