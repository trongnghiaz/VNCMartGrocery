namespace VNC.Application.Models.Admin.Orders
{
    public class AdminOrderDetailDto : AdminOrderDto
    {
        public string ReceiverName { get; set; } = null!;

        public string ReceiverPhone { get; set; } = null!;

        public string ShippingAddress { get; set; } = null!;

        public string? Note { get; set; }

        public List<AdminOrderItemDto> Items { get; set; } = new();
    }

    public class AdminOrderItemDto
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }
    }
}