
using System.ComponentModel.DataAnnotations;

namespace VNC.Application.Models.Orders
{
    public class CreateOrderDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string ReceiverName { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string ReceiverPhone { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = null!; // COD, Chuyển khoản...

        public string? Note { get; set; }

        [Required]
        public List<CartItemDto> Items { get; set; } = new();
    }

    public class CartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
