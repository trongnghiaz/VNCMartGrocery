using VNC.Application.Models;
using VNC.Application.Models.Products;

namespace VNC.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(GetProductsRequest request);
        Task<ProductDetailDto?> GetProductByIdAsync(int id);
        Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request);
        Task<bool> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
        Task<ProductDetailDto?> GetAdminProductByIdAsync(int id);
        Task<PagedResult<ProductDto>> GetAdminProductsAsync(GetProductsRequest request);
    }
}
