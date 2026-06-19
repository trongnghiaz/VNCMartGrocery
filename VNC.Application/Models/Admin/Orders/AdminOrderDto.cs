namespace VNC.Application.Models.Admin.Orders
{
    public class AdminOrderDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = null!;

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public decimal TotalOriginalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal TotalPayAmount { get; set; }
    }
}