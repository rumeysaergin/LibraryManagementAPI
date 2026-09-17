using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class UserMembershipRepository : IUserMembershipRepository
    {
        private readonly LibraryManagementDbContext _context;

        public UserMembershipRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<UserMembership> GetAll()
        {
            return _context.UserMemberships
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public UserMembership? GetById(Guid id)
        {
            return _context.UserMemberships
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public List<UserMembership> GetByUserId(Guid userId)
        {
            return _context.UserMemberships
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToList();
        }

        public void Add(UserMembership userMembership)
        {
            _context.UserMemberships.Add(userMembership);
            _context.SaveChanges();
        }

        public void Update(UserMembership userMembership)
        {
            _context.UserMemberships.Update(userMembership);
            _context.SaveChanges();
        }

        public void Delete(UserMembership userMembership)
        {
            userMembership.IsDeleted = true;
            _context.UserMemberships.Update(userMembership);
            _context.SaveChanges();
        }
    }
}