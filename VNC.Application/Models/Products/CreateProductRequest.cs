using System.ComponentModel.DataAnnotations;

namespace VNC.Application.Models.Products
{
    public class CreateProductRequest
    {
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = null!;

        [Required]
        [StringLength(250)]
        public string Slug { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? OriginalPrice { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;
    }
}
