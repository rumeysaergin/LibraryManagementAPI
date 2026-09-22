using LibraryManagementAPI.Application.DTOs.Category;
using LibraryManagementAPI.Application.Repositories;
using LibraryManagementAPI.Application.Services;
using LibraryManagementAPI.Domain.Entities;
using Moq;

namespace LibraryManagementAPI.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repositoryMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(_repositoryMock.Object);
        }

        private Category CreateCategory()
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Kategori",
                IsDeleted = false
            };
        }

        [Fact]
        public void GetAll_ShouldReturnCategories()
        {
            var categories = new List<Category>
            {
                CreateCategory(),
                CreateCategory()
            };

            _repositoryMock
                .Setup(x => x.GetAll())
                .Returns(categories);

            var result = _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnCategory_WhenCategoryExists()
        {
            var category = CreateCategory();

            _repositoryMock
                .Setup(x => x.GetById(category.Id))
                .Returns(category);

            var result = _service.GetById(category.Id);

            Assert.NotNull(result);
            Assert.Equal(category.Id, result!.Id);
            Assert.Equal(category.Name, result.Name);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Category?)null);

            var result = _service.GetById(id);

            Assert.Null(result);
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            var dto = new CategoryCreateDto
            {
                Name = "Yeni Kategori"
            };

            _service.Add(dto);

            _repositoryMock.Verify(
                x => x.Add(It.IsAny<Category>()),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateCategory_WhenCategoryExists()
        {
            var category = CreateCategory();

            _repositoryMock
                .Setup(x => x.GetById(category.Id))
                .Returns(category);

            var dto = new CategoryUpdateDto
            {
                Name = "Güncellenmiş Kategori"
            };

            _service.Update(category.Id, dto);

            Assert.Equal("Güncellenmiş Kategori", category.Name);

            _repositoryMock.Verify(
                x => x.Update(category),
                Times.Once);
        }

        [Fact]
        public void Update_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Category?)null);

            var dto = new CategoryUpdateDto
            {
                Name = "Güncellenmiş Kategori"
            };

            Assert.Throws<KeyNotFoundException>(
                () => _service.Update(id, dto));
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenCategoryExists()
        {
            var category = CreateCategory();

            _repositoryMock
                .Setup(x => x.GetById(category.Id))
                .Returns(category);

            _service.Delete(category.Id);

            _repositoryMock.Verify(
                x => x.Delete(category),
                Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(x => x.GetById(id))
                .Returns((Category?)null);

            Assert.Throws<KeyNotFoundException>(
                () => _service.Delete(id));
        }
    }
}