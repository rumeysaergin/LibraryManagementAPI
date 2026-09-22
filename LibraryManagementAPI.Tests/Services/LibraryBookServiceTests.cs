using LibraryManagementAPI.Application.DTOs.LibraryBook;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class LibraryBookServiceTests
    {
        private readonly Mock<ILibraryBookRepository> _libraryBookRepositoryMock;
        private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly LibraryBookService _service;

        public LibraryBookServiceTests()
        {
            _libraryBookRepositoryMock = new Mock<ILibraryBookRepository>();
            _libraryRepositoryMock = new Mock<ILibraryRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();

            _service = new LibraryBookService(
                _libraryBookRepositoryMock.Object,
                _libraryRepositoryMock.Object,
                _bookRepositoryMock.Object);
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

        private Library CreateLibrary()
        {
            return new Library
            {
                Id = Guid.NewGuid(),
                Name = "Bursa Merkez Kütüphanesi",
                Address = "Bursa",
                IsDeleted = false
            };
        }

        private Book CreateBook()
        {
            return new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Kitabı",
                ISBN = "1234567890",
                AuthorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnLibraryBooks()
        {
            var libraryBooks = new List<LibraryBook>
            {
                CreateLibraryBook(),
                CreateLibraryBook()
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(libraryBooks);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnLibraryBook_WhenExists()
        {
            var libraryBook = CreateLibraryBook();

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(libraryBook.Id))
                .Returns(libraryBook);

            var result = _service.GetById(libraryBook.Id);

            Assert.NotNull(result);
            Assert.Equal(libraryBook.Id, result.Id);
            Assert.Equal(libraryBook.LibraryId, result.LibraryId);
            Assert.Equal(libraryBook.BookId, result.BookId);
            Assert.Equal(libraryBook.Quantity, result.Quantity);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenDoesNotExist()
        {
            var id = Guid.NewGuid();

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((LibraryBook?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByLibraryId_ShouldReturnLibraryBooks()
        {
            var libraryId = Guid.NewGuid();

            var libraryBook1 = CreateLibraryBook();
            libraryBook1.LibraryId = libraryId;

            var libraryBook2 = CreateLibraryBook();
            libraryBook2.LibraryId = libraryId;

            _libraryBookRepositoryMock
                .Setup(x => x.GetByLibraryId(libraryId))
                .Returns(new List<LibraryBook>
                {
                    libraryBook1,
                    libraryBook2
                });

            var result = _service.GetByLibraryId(libraryId);

            Assert.Equal(2, result.Count);
            Assert.All(
                result,
                x => Assert.Equal(libraryId, x.LibraryId));
        }

        [Fact]
        public void GetByBookId_ShouldReturnLibraryBooks()
        {
            var bookId = Guid.NewGuid();

            var libraryBook1 = CreateLibraryBook();
            libraryBook1.BookId = bookId;

            var libraryBook2 = CreateLibraryBook();
            libraryBook2.BookId = bookId;

            _libraryBookRepositoryMock
                .Setup(x => x.GetByBookId(bookId))
                .Returns(new List<LibraryBook>
                {
                    libraryBook1,
                    libraryBook2
                });

            var result = _service.GetByBookId(bookId);

            Assert.Equal(2, result.Count);
            Assert.All(
                result,
                x => Assert.Equal(bookId, x.BookId));
        }

        [Fact]
        public void Add_ShouldAddLibraryBook_WhenDataIsValid()
        {
            var library = CreateLibrary();
            var book = CreateBook();

            var dto = new LibraryBookCreateDto
            {
                LibraryId = library.Id,
                BookId = book.Id,
                Quantity = 5
            };

            _libraryRepositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            _bookRepositoryMock
                .Setup(x => x.GetById(book.Id))
                .Returns(book);

            _libraryBookRepositoryMock
                .Setup(x => x.GetByLibraryId(library.Id))
                .Returns(new List<LibraryBook>());

            _service.Add(dto);

            _libraryBookRepositoryMock.Verify(
                x => x.Add(It.Is<LibraryBook>(libraryBook =>
                    libraryBook.LibraryId == dto.LibraryId &&
                    libraryBook.BookId == dto.BookId &&
                    libraryBook.Quantity == dto.Quantity &&
                    libraryBook.IsDeleted == false)),
                Times.Once);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenLibraryDoesNotExist()
        {
            var dto = new LibraryBookCreateDto
            {
                LibraryId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                Quantity = 5
            };

            _libraryRepositoryMock
                .Setup(x => x.GetById(dto.LibraryId))
                .Returns((Library?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Add(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenBookDoesNotExist()
        {
            var library = CreateLibrary();

            var dto = new LibraryBookCreateDto
            {
                LibraryId = library.Id,
                BookId = Guid.NewGuid(),
                Quantity = 5
            };

            _libraryRepositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            _bookRepositoryMock
                .Setup(x => x.GetById(dto.BookId))
                .Returns((Book?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Add(dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Add(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            var library = CreateLibrary();
            var book = CreateBook();

            _libraryRepositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            _bookRepositoryMock
                .Setup(x => x.GetById(book.Id))
                .Returns(book);

            var dto = new LibraryBookCreateDto
            {
                LibraryId = library.Id,
                BookId = book.Id,
                Quantity = 0
            };

            Assert.Throws<InvalidOperationException>(
                () => _service.Add(dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Add(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Add_ShouldThrowException_WhenBookAlreadyExistsInLibrary()
        {
            var library = CreateLibrary();
            var book = CreateBook();

            var existingLibraryBook = CreateLibraryBook();
            existingLibraryBook.LibraryId = library.Id;
            existingLibraryBook.BookId = book.Id;

            var dto = new LibraryBookCreateDto
            {
                LibraryId = library.Id,
                BookId = book.Id,
                Quantity = 5
            };

            _libraryRepositoryMock
                .Setup(x => x.GetById(library.Id))
                .Returns(library);

            _bookRepositoryMock
                .Setup(x => x.GetById(book.Id))
                .Returns(book);

            _libraryBookRepositoryMock
                .Setup(x => x.GetByLibraryId(library.Id))
                .Returns(new List<LibraryBook>
                {
                    existingLibraryBook
                });

            Assert.Throws<InvalidOperationException>(
                () => _service.Add(dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Add(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Update_ShouldUpdateQuantity_WhenLibraryBookExists()
        {
            var libraryBook = CreateLibraryBook();

            var dto = new LibraryBookUpdateDto
            {
                Quantity = 10
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(libraryBook.Id))
                .Returns(libraryBook);

            _service.Update(libraryBook.Id, dto);

            Assert.Equal(10, libraryBook.Quantity);

            _libraryBookRepositoryMock.Verify(
                x => x.Update(libraryBook),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenLibraryBookDoesNotExist()
        {
            var id = Guid.NewGuid();

            var dto = new LibraryBookUpdateDto
            {
                Quantity = 10
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((LibraryBook?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            var libraryBook = CreateLibraryBook();

            var dto = new LibraryBookUpdateDto
            {
                Quantity = 0
            };

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(libraryBook.Id))
                .Returns(libraryBook);

            Assert.Throws<InvalidOperationException>(
                () => _service.Update(libraryBook.Id, dto));

            _libraryBookRepositoryMock.Verify(
                x => x.Update(It.IsAny<LibraryBook>()),
                Times.Never);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenLibraryBookExists()
        {
            var libraryBook = CreateLibraryBook();

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(libraryBook.Id))
                .Returns(libraryBook);

            _service.Delete(libraryBook.Id);

            _libraryBookRepositoryMock.Verify(
                x => x.Delete(libraryBook),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenLibraryBookDoesNotExist()
        {
            var id = Guid.NewGuid();

            _libraryBookRepositoryMock
                .Setup(x => x.GetById(id))
                .Returns((LibraryBook?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));

            _libraryBookRepositoryMock.Verify(
                x => x.Delete(It.IsAny<LibraryBook>()),
                Times.Never);
        }
    }
}