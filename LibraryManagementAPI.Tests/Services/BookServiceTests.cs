using LibraryManagementAPI.Application.DTOs.Book;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _repositoryMock;
        private readonly BookService _service;

        public BookServiceTests()
        {
            _repositoryMock = new Mock<IBookRepository>();
            _service = new BookService(_repositoryMock.Object);
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
        public void GetAll_ShouldReturnBooks()
        {
            var books = new List<Book>
            {
                CreateBook(),
                CreateBook()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(books);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnBook_WhenBookExists()
        {
            var book = CreateBook();

            _repositoryMock
                .Setup(x => x.GetById(book.Id))
                .Returns(book);

            var result = _service.GetById(book.Id);

            Assert.NotNull(result);
            Assert.Equal(book.Id, result!.Id);
            Assert.Equal(book.Title, result.Title);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Book?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void GetByAuthorId_ShouldReturnBooks()
        {
            var authorId = Guid.NewGuid();

            var book1 = CreateBook();
            book1.AuthorId = authorId;

            var book2 = CreateBook();
            book2.AuthorId = authorId;

            _repositoryMock
                .Setup(x => x.GetByAuthorId(authorId))
                .Returns(new List<Book> { book1, book2 });

            var result = _service.GetByAuthorId(authorId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(authorId, x.AuthorId));
        }

        [Fact]
        public void GetByCategoryId_ShouldReturnBooks()
        {
            var categoryId = Guid.NewGuid();

            var book1 = CreateBook();
            book1.CategoryId = categoryId;

            var book2 = CreateBook();
            book2.CategoryId = categoryId;

            _repositoryMock
                .Setup(x => x.GetByCategoryId(categoryId))
                .Returns(new List<Book> { book1, book2 });

            var result = _service.GetByCategoryId(categoryId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.Equal(categoryId, x.CategoryId));
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new BookCreateDto
            {
                Title = "Yeni Kitap",
                ISBN = "9876543210",
                AuthorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid()
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<Book>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateBook_WhenBookExists()
        {
            var book = CreateBook();

            _repositoryMock
                .Setup(x => x.GetById(book.Id))
                .Returns(book);

            var dto = new BookUpdateDto
            {
                Title = "Güncellenmiş Kitap",
                ISBN = "1234567890",
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId
            };

            _service.Update(book.Id, dto);

            Assert.Equal("Güncellenmiş Kitap", book.Title);
            Assert.Equal(dto.ISBN, book.ISBN);
            Assert.Equal(dto.AuthorId, book.AuthorId);
            Assert.Equal(dto.CategoryId, book.CategoryId);

            _repositoryMock.Verify(
                x => x.Update(book),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenBookDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Book?)null);

            var dto = new BookUpdateDto
            {
                Title = "Güncellenmiş Kitap",
                ISBN = "1234567890",
                AuthorId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid()
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenBookExists()
        {
            var book = CreateBook();

            _repositoryMock
                .Setup(x => x.GetById(book.Id))
                .Returns(book);

            _service.Delete(book.Id);

            _repositoryMock.Verify(
                x => x.Delete(book),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenBookDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Book?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}