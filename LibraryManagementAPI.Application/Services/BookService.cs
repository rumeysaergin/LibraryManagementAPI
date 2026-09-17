using LibraryManagementAPI.Application.DTOs.Book;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public List<BookResponseDto> GetAll()
        {
            return _bookRepository.GetAll()
                .Select(x => new BookResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ISBN = x.ISBN,
                    StockQuantity = x.StockQuantity,
                    AuthorId = x.AuthorId,
                    CategoryId = x.CategoryId
                })
                .ToList();
        }

        public BookResponseDto? GetById(Guid id)
        {
            var book = _bookRepository.GetById(id);

            if (book == null)
                return null;

            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                StockQuantity = book.StockQuantity,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId
            };
        }

        public List<BookResponseDto> GetByAuthorId(Guid authorId)
        {
            return _bookRepository.GetByAuthorId(authorId)
                .Select(x => new BookResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ISBN = x.ISBN,
                    StockQuantity = x.StockQuantity,
                    AuthorId = x.AuthorId,
                    CategoryId = x.CategoryId
                })
                .ToList();
        }

        public List<BookResponseDto> GetByCategoryId(Guid categoryId)
        {
            return _bookRepository.GetByCategoryId(categoryId)
                .Select(x => new BookResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ISBN = x.ISBN,
                    StockQuantity = x.StockQuantity,
                    AuthorId = x.AuthorId,
                    CategoryId = x.CategoryId
                })
                .ToList();
        }

        public void Add(BookCreateDto dto)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ISBN = dto.ISBN,
                StockQuantity = dto.StockQuantity,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                IsDeleted = false
            };

            _bookRepository.Add(book);
        }

        public void Update(Guid id, BookUpdateDto dto)
        {
            var book = _bookRepository.GetById(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            book.Title = dto.Title;
            book.ISBN = dto.ISBN;
            book.StockQuantity = dto.StockQuantity;
            book.AuthorId = dto.AuthorId;
            book.CategoryId = dto.CategoryId;

            _bookRepository.Update(book);
        }

        public void Delete(Guid id)
        {
            var book = _bookRepository.GetById(id);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            _bookRepository.Delete(book);
        }
    }
}