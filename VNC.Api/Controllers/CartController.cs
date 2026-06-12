using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Carts;

namespace VNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserContextService _userContextService;

        public CartController(ICartService cartService, IUserContextService userContextService)
        {
            _cartService = cartService;
            _userContextService = userContextService;
        }

        [HttpPost("add")] 
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            int? customerId = _userContextService.GetUserId();

            if (customerId == null)
            {
                return Unauthorized(ApiResponse<object>.Failure("Không thể xác định danh tính khách hàng."));
            }

            var result = await _cartService.AddToCartAsync(customerId.Value, request);
            return Ok(ApiResponse<CartDto>.Success(result));
        }

        [HttpGet] 
        public async Task<IActionResult> GetCart()
        {
            int? customerId = _userContextService.GetUserId();
            if (customerId == null)
            {
                return Unauthorized(ApiResponse<object>.Failure("Không thể xác định danh tính khách hàng."));
            }

            var result = await _cartService.GetCartAsync(customerId.Value);
            return Ok(ApiResponse<CartDto>.Success(result));
        }
    }
}
