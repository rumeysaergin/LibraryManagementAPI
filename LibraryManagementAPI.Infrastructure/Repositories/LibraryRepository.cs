using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LibraryManagementDbContext _context;

        public LibraryRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<Library> GetAll()
        {
            return _context.Libraries
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public Library? GetById(Guid id)
        {
            return _context.Libraries
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public void Add(Library library)
        {
            _context.Libraries.Add(library);
            _context.SaveChanges();
        }

        public void Update(Library library)
        {
            _context.Libraries.Update(library);
            _context.SaveChanges();
        }

        public void Delete(Library library)
        {
            library.IsDeleted = true;
            _context.Libraries.Update(library);
            _context.SaveChanges();
        }
    }
}