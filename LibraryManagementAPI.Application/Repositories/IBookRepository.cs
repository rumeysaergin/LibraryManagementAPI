using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface IBookRepository
    {
        List<Book> GetAll();
        Book? GetById(Guid id);
        List<Book> GetByAuthorId(Guid authorId);
        List<Book> GetByCategoryId(Guid categoryId);

        void Add(Book book);
        void Update(Book book);
        void Delete(Book book);
    }
}