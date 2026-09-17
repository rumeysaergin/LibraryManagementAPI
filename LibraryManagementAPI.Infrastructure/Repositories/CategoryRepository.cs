using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;
using LibraryManagementAPI.Infrastructure.Data;

namespace LibraryManagementAPI.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryManagementDbContext _context;

        public CategoryRepository(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public List<Category> GetAll()
        {
            return _context.Categories
                .Where(x => !x.IsDeleted)
                .ToList();
        }

        public Category? GetById(Guid id)
        {
            return _context.Categories
                .FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        public void Delete(Category category)
        {
            category.IsDeleted = true;
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
    }
}