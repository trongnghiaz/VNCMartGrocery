
namespace VNC.Application.Models.Products
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal Price { get; set; } // Giá bán hiện tại
        public decimal? OriginalPrice { get; set; } // Giá gốc trước khi giảm (nếu có)
        public string? ThumbnailUrl { get; set; }
        public int StockQuantity { get; set; }
        public decimal RatingAverage { get; set; }
        public bool IsAvailable => StockQuantity > 0; // Thuộc tính tự tính toán
        public string CategoryName { get; set; } = string.Empty;
    }
}
