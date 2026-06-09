using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Carts;

namespace VNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(int customerId)
        {
            var result = await _cartService.GetCartAsync(customerId);
            return Ok(ApiResponse<CartDto>.Success(result));
        }

        [HttpPost("{customerId}/add")]
        public async Task<IActionResult> AddItem(int customerId, [FromBody] AddToCartRequest request)
        {
            var result = await _cartService.AddToCartAsync(customerId, request);
            return Ok(ApiResponse<CartDto>.Success(result));
        }

        [HttpPut("{customerId}/update-quantity")]
        public async Task<IActionResult> UpdateQuantity(int customerId, [FromQuery] int productId, [FromQuery] int quantity)
        {
            var result = await _cartService.UpdateQuantityAsync(customerId, productId, quantity);
            return Ok(ApiResponse<CartDto>.Success(result));
        }

        [HttpDelete("{customerId}/remove-item/{productId}")]
        public async Task<IActionResult> RemoveItem(int customerId, int productId)
        {
            var result = await _cartService.RemoveFromCartAsync(customerId, productId);
            return Ok(ApiResponse<CartDto>.Success(result));
        }
    }
}
