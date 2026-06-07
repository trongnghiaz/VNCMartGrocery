using System.ComponentModel.DataAnnotations;
namespace VNC.Application.Models.Categories
{
    public class CreateCategoryRequest
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Slug { get; set; } = null!;

        public int DisplayOrder { get; set; } = 0;
        public bool IsVisible { get; set; } = true;
        public string? Description { get; set; }
    }
}
