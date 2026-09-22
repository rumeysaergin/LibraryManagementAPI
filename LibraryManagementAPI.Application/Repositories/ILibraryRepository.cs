using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface ILibraryRepository
    {
        List<Library> GetAll();
        Library? GetById(Guid id);

        void Add(Library library);
        void Update(Library library);
        void Delete(Library library);
    }
}