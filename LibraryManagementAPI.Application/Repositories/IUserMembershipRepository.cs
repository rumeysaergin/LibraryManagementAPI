using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IUserMembershipRepository
    {
        List<UserMembership> GetAll();
        UserMembership? GetById(Guid id);
        List<UserMembership> GetByUserId(Guid userId);

        void Add(UserMembership userMembership);
        void Update(UserMembership userMembership);
        void Delete(UserMembership userMembership);
    }
}