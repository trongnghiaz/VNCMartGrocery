
using System.ComponentModel.DataAnnotations;

namespace VNC.Application.Models.Carts
{
    public class AddToCartRequest
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng thêm vào giỏ hàng phải lớn hơn hoặc bằng 1.")]
        public int Quantity { get; set; }
    }
}
