
using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Products;
using VNC.Domain.Entities;

namespace VNC.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IAppDbContext _context;

        public ProductService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductDto>> GetProductsAsync(GetProductsRequest request)
        {
            request.PageNumber = Math.Max(1, request.PageNumber);
            request.PageSize = Math.Clamp(request.PageSize, 1, 100);
            // Lọc các sản phẩm được phép hiển thị (IsVisible == true)
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsVisible)
                .AsQueryable();

            // Tìm kiếm theo Tên hoặc Mã sản phẩm
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(searchTerm)
                                      || p.ProductCode.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Slug = p.Slug,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    ThumbnailUrl = p.ThumbnailUrl,
                    StockQuantity = p.StockQuantity,
                    RatingAverage = p.RatingAverage,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chưa phân loại"
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id && p.IsVisible);

            if (product == null) return null;

            return new ProductDetailDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Slug = product.Slug,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Description = product.Description,
                ThumbnailUrl = product.ThumbnailUrl,
                StockQuantity = product.StockQuantity,
                RatingAverage = product.RatingAverage,
                CategoryName = product.Category?.CategoryName ?? "Chưa phân loại"
            };
        }

        // 3. CREATE (Thêm sản phẩm mới)
        public async Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                Slug = request.Slug,
                CategoryId = request.CategoryId,
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Description = request.Description,
                ThumbnailUrl = request.ThumbnailUrl,
                StockQuantity = request.StockQuantity,
                IsVisible = true,
                CreatedAt = DateTime.Now // Hoặc để SaveChanges tự sinh nếu làm Audit Logs
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return (await GetProductByIdAsync(product.ProductId))!;
        }

        // 4. UPDATE (Sửa sản phẩm)
        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return false;

            product.ProductCode = request.ProductCode;
            product.ProductName = request.ProductName;
            product.Slug = request.Slug;
            product.CategoryId = request.CategoryId;
            product.Price = request.Price;
            product.OriginalPrice = request.OriginalPrice;
            product.Description = request.Description;
            product.ThumbnailUrl = request.ThumbnailUrl;
            product.StockQuantity = request.StockQuantity;
            product.IsVisible = request.IsVisible;

            await _context.SaveChangesAsync();
            return true;
        }

        // 5. DELETE (Xóa sản phẩm)
        // Khuyên dùng: Sử dụng giải pháp Soft Delete (ẩn đi) thay vì xóa hẳn khỏi DB để tránh lỗi toàn vẹn dữ liệu đơn hàng cũ
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return false;

            product.IsVisible = false; // Soft delete

            // Nếu muốn Hard Delete (xóa hẳn), dùng lệnh dưới:
            // _context.Products.Remove(product);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<ProductDto>> GetAdminProductsAsync(GetProductsRequest request)
        {
            request.PageNumber = Math.Max(1, request.PageNumber);
            request.PageSize = Math.Clamp(request.PageSize, 1, 100);
            // Lọc các sản phẩm được phép hiển thị (IsVisible == true)
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Tìm kiếm theo Tên hoặc Mã sản phẩm
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(searchTerm)
                                      || p.ProductCode.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Slug = p.Slug,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    ThumbnailUrl = p.ThumbnailUrl,
                    StockQuantity = p.StockQuantity,
                    RatingAverage = p.RatingAverage,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chưa phân loại"
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
        public async Task<ProductDetailDto?> GetAdminProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return null;

            return new ProductDetailDto
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Slug = product.Slug,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Description = product.Description,
                ThumbnailUrl = product.ThumbnailUrl,
                StockQuantity = product.StockQuantity,
                RatingAverage = product.RatingAverage,
                CategoryName = product.Category?.CategoryName ?? "Chưa phân loại"
            };
        }
    }
}
