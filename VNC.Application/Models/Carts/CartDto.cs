
namespace VNC.Application.Models.Carts
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<CartItemDto> Items { get; set; } = new();

        // Thuộc tính tự tính toán: Tổng số lượng sản phẩm có trong giỏ
        public int TotalQuantity => Items.Sum(item => item.Quantity);

        // Thuộc tính tự tính toán: Tổng tiền tạm tính của cả giỏ hàng (Chưa tính ship/mã giảm giá)
        public decimal SubTotal => Items.Sum(item => item.TotalPrice);
    }
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }

        // Đổ dữ liệu từ bảng Product sang để hiển thị tên và ảnh
        public string ProductName { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }

        // Giá bán hiện tại của sản phẩm
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Thuộc tính tự tính toán (Read-only property): Thành tiền của dòng này
        public decimal TotalPrice => Price * Quantity;
    }
}
