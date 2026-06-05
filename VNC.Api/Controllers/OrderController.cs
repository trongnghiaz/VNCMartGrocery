using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models.Orders;

namespace VNC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request)
        {
            try
            {
                // Gọi xuống Application Layer để xử lý nghiệp vụ
                string resultOrderCode = await _orderService.CreateOrderAsync(request);

                // Trả về kết quả 200 OK kèm mã book thành công
                return Ok(new { Success = true, Message = "Đặt hàng thành công!", OrderCode = resultOrderCode });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 400 nếu có bất kỳ sự cố nào (Hết hàng, sai ID...)
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
