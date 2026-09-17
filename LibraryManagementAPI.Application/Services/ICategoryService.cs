using LibraryManagementAPI.Application.DTOs.Category;

namespace LibraryManagementAPI.Application.Services
{
    public interface ICategoryService
    {
        List<CategoryResponseDto> GetAll();
        CategoryResponseDto? GetById(Guid id);

        void Add(CategoryCreateDto dto);
        void Update(Guid id, CategoryUpdateDto dto);
        void Delete(Guid id);
    }
}