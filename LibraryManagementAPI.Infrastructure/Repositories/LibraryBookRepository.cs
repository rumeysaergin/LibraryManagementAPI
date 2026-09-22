using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class LibraryBookRepository : ILibraryBookRepository
    {
        private readonly LibraryManagementDbContext _context;

        public LibraryBookRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<LibraryBook> GetAll()
        {
            return _context.LibraryBooks
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public LibraryBook? GetById(Guid id)
        {
            return _context.LibraryBooks
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<LibraryBook> GetByLibraryId(Guid libraryId)
        {
            return _context.LibraryBooks
                .Where(x => x.LibraryId == libraryId && !x.IsDeleted)
                .ToList();
        }

        public List<LibraryBook> GetByBookId(Guid bookId)
        {
            return _context.LibraryBooks
                .Where(x => x.BookId == bookId && !x.IsDeleted)
                .ToList();
        }

        public void Add(LibraryBook libraryBook)
        {
            _context.LibraryBooks.Add(libraryBook);
            _context.SaveChanges();
        }

        public void Update(LibraryBook libraryBook)
        {
            _context.LibraryBooks.Update(libraryBook);
            _context.SaveChanges();
        }

        public void Delete(LibraryBook libraryBook)
        {
            libraryBook.IsDeleted = true;

            _context.LibraryBooks.Update(libraryBook);
            _context.SaveChanges();
        }
    }
}