using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Orders;
using VNC.Application.Services;

namespace VNC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserContextService _userContextService;
        public OrderController(IOrderService orderService, IUserContextService userContextService)
        {
            _orderService = orderService;
            _userContextService = userContextService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request)
        {
            int? customerId = _userContextService.GetUserId();
            if (!customerId.HasValue)
            {
                return Unauthorized(ApiResponse<object>.Failure("Không thể xác định danh tính tài khoản người dùng."));
            }

            string resultOrderCode = await _orderService.CreateOrderAsync(request, customerId.Value);

            return Ok(ApiResponse<string>.Success(resultOrderCode));
        }
    }
}
