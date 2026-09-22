using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IUserLibraryRepository
    {
        List<UserLibrary> GetAll();
        UserLibrary? GetById(Guid id);
        List<UserLibrary> GetByUserId(Guid userId);
        List<UserLibrary> GetByLibraryId(Guid libraryId);

        void Add(UserLibrary userLibrary);
        void Update(UserLibrary userLibrary);
        void Delete(UserLibrary userLibrary);
    }
}