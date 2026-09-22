using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class UserLibraryRepository : IUserLibraryRepository
    {
        private readonly LibraryManagementDbContext _context;

        public UserLibraryRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<UserLibrary> GetAll()
        {
            return _context.UserLibraries
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public UserLibrary? GetById(Guid id)
        {
            return _context.UserLibraries
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<UserLibrary> GetByUserId(Guid userId)
        {
            return _context.UserLibraries
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToList();
        }

        public List<UserLibrary> GetByLibraryId(Guid libraryId)
        {
            return _context.UserLibraries
                .Where(x => x.LibraryId == libraryId && !x.IsDeleted)
                .ToList();
        }

        public void Add(UserLibrary userLibrary)
        {
            _context.UserLibraries.Add(userLibrary);
            _context.SaveChanges();
        }

        public void Update(UserLibrary userLibrary)
        {
            _context.UserLibraries.Update(userLibrary);
            _context.SaveChanges();
        }

        public void Delete(UserLibrary userLibrary)
        {
            userLibrary.IsDeleted = true;
            _context.UserLibraries.Update(userLibrary);
            _context.SaveChanges();
        }
    }
}