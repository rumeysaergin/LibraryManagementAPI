using LibraryManagementAPI.Application.DTOs.LibraryBook;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class LibraryBookService : ILibraryBookService
    {
        private readonly ILibraryBookRepository _libraryBookRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IBookRepository _bookRepository;

        public LibraryBookService(
            ILibraryBookRepository libraryBookRepository,
            ILibraryRepository libraryRepository,
            IBookRepository bookRepository)
        {
            _libraryBookRepository = libraryBookRepository;
            _libraryRepository = libraryRepository;
            _bookRepository = bookRepository;
        }

        public List<LibraryBookResponseDto> GetAll()
        {
            return _libraryBookRepository.GetAll()
                .Select(x => new LibraryBookResponseDto
                {
                    Id = x.Id,
                    LibraryId = x.LibraryId,
                    BookId = x.BookId,
                    Quantity = x.Quantity
                })
                .ToList();
        }

        public LibraryBookResponseDto? GetById(Guid id)
        {
            var libraryBook = _libraryBookRepository.GetById(id);

            if (libraryBook == null)
                return null;

            return new LibraryBookResponseDto
            {
                Id = libraryBook.Id,
                LibraryId = libraryBook.LibraryId,
                BookId = libraryBook.BookId,
                Quantity = libraryBook.Quantity
            };
        }

        public List<LibraryBookResponseDto> GetByLibraryId(Guid libraryId)
        {
            return _libraryBookRepository.GetByLibraryId(libraryId)
                .Select(x => new LibraryBookResponseDto
                {
                    Id = x.Id,
                    LibraryId = x.LibraryId,
                    BookId = x.BookId,
                    Quantity = x.Quantity
                })
                .ToList();
        }

        public List<LibraryBookResponseDto> GetByBookId(Guid bookId)
        {
            return _libraryBookRepository.GetByBookId(bookId)
                .Select(x => new LibraryBookResponseDto
                {
                    Id = x.Id,
                    LibraryId = x.LibraryId,
                    BookId = x.BookId,
                    Quantity = x.Quantity
                })
                .ToList();
        }

        public void Add(LibraryBookCreateDto dto)
        {
            var library = _libraryRepository.GetById(dto.LibraryId);

            if (library == null)
                throw new KeyNotFoundException("Library not found.");

            var book = _bookRepository.GetById(dto.BookId);

            if (book == null)
                throw new KeyNotFoundException("Book not found.");

            if (dto.Quantity <= 0)
                throw new InvalidOperationException(
                    "Quantity must be greater than zero.");

            var existingLibraryBook = _libraryBookRepository
                .GetByLibraryId(dto.LibraryId)
                .FirstOrDefault(x => x.BookId == dto.BookId);

            if (existingLibraryBook != null)
                throw new InvalidOperationException(
                    "This book already exists in the library.");

            var libraryBook = new LibraryBook
            {
                Id = Guid.NewGuid(),
                LibraryId = dto.LibraryId,
                BookId = dto.BookId,
                Quantity = dto.Quantity,
                IsDeleted = false
            };

            _libraryBookRepository.Add(libraryBook);
        }

        public void Update(Guid id, LibraryBookUpdateDto dto)
        {
            var libraryBook = _libraryBookRepository.GetById(id);

            if (libraryBook == null)
                throw new KeyNotFoundException("Library book record not found.");

            if (dto.Quantity <= 0)
                throw new InvalidOperationException(
                    "Quantity must be greater than zero.");

            libraryBook.Quantity = dto.Quantity;

            _libraryBookRepository.Update(libraryBook);
        }

        public void Delete(Guid id)
        {
            var libraryBook = _libraryBookRepository.GetById(id);

            if (libraryBook == null)
                throw new KeyNotFoundException("Library book record not found.");

            _libraryBookRepository.Delete(libraryBook);
        }
    }
}