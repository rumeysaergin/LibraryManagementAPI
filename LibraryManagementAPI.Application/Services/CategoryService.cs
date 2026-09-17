using LibraryManagementAPI.Application.DTOs.Category;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Domain.Entities;

namespace LibraryManagementAPI.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<CategoryResponseDto> GetAll()
        {
            return _categoryRepository.GetAll()
                .Select(x => new CategoryResponseDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }

        public CategoryResponseDto? GetById(Guid id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public void Add(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsDeleted = false
            };

            _categoryRepository.Add(category);
        }

        public void Update(Guid id, CategoryUpdateDto dto)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            category.Name = dto.Name;

            _categoryRepository.Update(category);
        }

        public void Delete(Guid id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            _categoryRepository.Delete(category);
        }
    }
}