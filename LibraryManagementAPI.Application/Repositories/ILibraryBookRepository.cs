using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface ILibraryBookRepository
    {
        List<LibraryBook> GetAll();

        LibraryBook? GetById(Guid id);

        List<LibraryBook> GetByLibraryId(Guid libraryId);

        List<LibraryBook> GetByBookId(Guid bookId);

        void Add(LibraryBook libraryBook);

        void Update(LibraryBook libraryBook);

        void Delete(LibraryBook libraryBook);
    }
}