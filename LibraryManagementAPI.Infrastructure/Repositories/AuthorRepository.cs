using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryManagementDbContext _context;

        public AuthorRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<Author> GetAll()
        {
            return _context.Authors
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public Author? GetById(Guid id)
        {
            return _context.Authors
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public void Add(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        public void Update(Author author)
        {
            _context.Authors.Update(author);
            _context.SaveChanges();
        }

        public void Delete(Author author)
        {
            author.IsDeleted = true;
            _context.Authors.Update(author);
            _context.SaveChanges();
        }
    }
}