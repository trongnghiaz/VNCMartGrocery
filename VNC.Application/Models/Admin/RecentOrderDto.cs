
namespace VNC.Application.Models.Admin
{
    public class RecentOrderDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = null!;

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public decimal TotalPayAmount { get; set; }
    }
}
