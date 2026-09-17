using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryManagementDbContext _context;

        public UserRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public User? GetById(Guid id)
        {
            return _context.Users
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public User? GetByEmail(string email)
        {
            return _context.Users
                .FirstOrDefault(x => x.Email == email && !x.IsDeleted);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(User user)
        {
            user.IsDeleted = true;
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}