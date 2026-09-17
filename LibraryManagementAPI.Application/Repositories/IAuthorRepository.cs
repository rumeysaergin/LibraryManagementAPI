using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IAuthorRepository
    {
        List<Author> GetAll();
        Author? GetById(Guid id);

        void Add(Author author);
        void Update(Author author);
        void Delete(Author author);
    }
}