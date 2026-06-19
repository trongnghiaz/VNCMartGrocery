
namespace VNC.Application.Models.Admin
{
    public class TopProductDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? ThumbnailUrl { get; set; }

        public int TotalQuantitySold { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
