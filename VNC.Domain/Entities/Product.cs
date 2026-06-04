

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNC.Domain.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

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

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        public int StockQuantity { get; set; } = 0;

        [Column(TypeName = "decimal(2,1)")]
        public decimal RatingAverage { get; set; } = 5.0m;

        public bool IsVisible { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
