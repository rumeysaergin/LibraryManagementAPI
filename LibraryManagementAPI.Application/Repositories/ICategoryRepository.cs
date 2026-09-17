using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category? GetById(Guid id);

        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);
    }
}