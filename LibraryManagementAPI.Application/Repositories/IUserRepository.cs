using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(Guid id);
        User? GetByEmail(string email);

        void Add(User user);
        void Update(User user);
        void Delete(User user);
    }
}