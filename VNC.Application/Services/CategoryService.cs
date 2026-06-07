using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models.Categories;
using VNC.Domain.Entities;

namespace VNC.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IAppDbContext _context;

        public CategoryService(IAppDbContext context)
        {
            _context = context;
        }

        // 1. READ ALL (Sắp xếp theo thứ tự hiển thị tăng dần)
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.DisplayOrder) // Sử dụng trường DisplayOrder để sắp xếp
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Slug = c.Slug,
                    DisplayOrder = c.DisplayOrder,
                    IsVisible = c.IsVisible,
                    Description = c.Description
                })
                .ToListAsync();
        }

        // 2. READ DETAIL
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Slug = category.Slug,
                DisplayOrder = category.DisplayOrder,
                IsVisible = category.IsVisible,
                Description = category.Description
            };
        }

        // 3. CREATE
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                CategoryName = request.CategoryName,
                Slug = request.Slug,
                DisplayOrder = request.DisplayOrder,
                IsVisible = request.IsVisible,
                Description = request.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return (await GetCategoryByIdAsync(category.CategoryId))!;
        }

        // 4. UPDATE
        public async Task<bool> UpdateCategoryAsync(int id, CreateCategoryRequest request)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return false;

            category.CategoryName = request.CategoryName;
            category.Slug = request.Slug;
            category.DisplayOrder = request.DisplayOrder;
            category.IsVisible = request.IsVisible;
            category.Description = request.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        // 5. DELETE (Xóa mềm bằng cách đổi IsVisible = false, hoặc Xóa cứng)
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return false;

            // Khuyên dùng Xóa Mềm để tránh lỗi khóa ngoại liên kết với bảng Product cũ
            category.IsVisible = false;

            // Nếu muốn Xóa Cứng hẳn khỏi DB, bỏ chú thích dòng dưới:
            // _context.Categories.Remove(category);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
