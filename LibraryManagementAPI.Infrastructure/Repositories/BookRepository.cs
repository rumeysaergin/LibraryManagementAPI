using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryManagementDbContext _context;

        public BookRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<Book> GetAll()
        {
            return _context.Books
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public Book? GetById(Guid id)
        {
            return _context.Books
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<Book> GetByAuthorId(Guid authorId)
        {
            return _context.Books
                .Where(x => x.AuthorId == authorId && !x.IsDeleted)
                .ToList();
        }

        public List<Book> GetByCategoryId(Guid categoryId)
        {
            return _context.Books
                .Where(x => x.CategoryId == categoryId && !x.IsDeleted)
                .ToList();
        }

        public void Add(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }

        public void Delete(Book book)
        {
            book.IsDeleted = true;
            _context.Books.Update(book);
            _context.SaveChanges();
        }
    }
}