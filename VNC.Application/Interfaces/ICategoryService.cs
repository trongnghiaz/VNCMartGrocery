using VNC.Application.Models.Categories;

namespace VNC.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request);
        Task<bool> UpdateCategoryAsync(int id, CreateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
