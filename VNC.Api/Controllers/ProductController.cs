using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Products;

namespace VNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request)
        {
            var result = await _productService.GetProductsAsync(request);
            return Ok(ApiResponse<PagedResult<ProductDto>>.Success(result));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<ProductDetailDto>.Failure("Không tìm thấy sản phẩm."));

            return Ok(ApiResponse<ProductDetailDto>.Success(result));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var result = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId }, ApiResponse<ProductDetailDto>.Success(result, 21));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            var success = await _productService.UpdateProductAsync(id, request);
            if (!success)
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy sản phẩm để cập nhật."));

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy sản phẩm để xóa."));

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
