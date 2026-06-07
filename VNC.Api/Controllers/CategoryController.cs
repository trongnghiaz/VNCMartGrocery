
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Categories;

namespace VNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<CategoryDto>>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<CategoryDto>.Failure("Không tìm thấy danh mục."));
            return Ok(ApiResponse<CategoryDto>.Success(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.CategoryId }, ApiResponse<CategoryDto>.Success(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryRequest request)
        {
            var success = await _categoryService.UpdateCategoryAsync(id, request);
            if (!success) return NotFound(ApiResponse<bool>.Failure("Không tìm thấy danh mục để cập nhật."));
            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success) return NotFound(ApiResponse<bool>.Failure("Không tìm thấy danh mục để xóa."));
            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
