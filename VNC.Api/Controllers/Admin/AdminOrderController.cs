using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Admin.Orders;

namespace VNC.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;

        public AdminOrderController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] GetAdminOrdersRequest request)
        {
            var result = await _adminOrderService.GetOrdersAsync(request);
            return Ok(ApiResponse<PagedResult<AdminOrderDto>>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _adminOrderService.GetOrderByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<AdminOrderDetailDto>.Failure("Không tìm thấy đơn hàng."));
            }

            return Ok(ApiResponse<AdminOrderDetailDto>.Success(result));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] short statusValue)
        {
            var success = await _adminOrderService.UpdateOrderStatusAsync(id, statusValue);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy đơn hàng để cập nhật trạng thái."));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPut("{id}/payment-status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromQuery] short paymentStatusValue)
        {
            var success = await _adminOrderService.UpdatePaymentStatusAsync(id, paymentStatusValue);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy đơn hàng để cập nhật trạng thái thanh toán."));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}