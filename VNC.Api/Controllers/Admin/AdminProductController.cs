using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Products;

namespace VNC.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin")]
    public class AdminProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public AdminProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request)
        {
            var result = await _productService.GetAdminProductsAsync(request);
            return Ok(ApiResponse<PagedResult<ProductDto>>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetAdminProductByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<ProductDetailDto>.Failure("Không tìm thấy sản phẩm."));
            }

            return Ok(ApiResponse<ProductDetailDto>.Success(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var result = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId }, ApiResponse<ProductDetailDto>.Success(result, 201));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            var success = await _productService.UpdateProductAsync(id, request);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy sản phẩm để cập nhật."));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy sản phẩm để xóa."));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}