using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Auth;

namespace VNC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterRequest request)
        {
            try
            {
                var success = await _authService.RegisterCustomerAsync(request);
                return Ok(ApiResponse<bool>.Success(success));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.Failure(ex.Message));
            }
        }

        [HttpPost("customer/login")]
        public async Task<IActionResult> CustomerLogin([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginCustomerAsync(request);
            if (result == null)
            {
                return Unauthorized(ApiResponse<AuthResultDto>.Failure("Số điện thoại hoặc mật khẩu không chính xác."));
            }
            return Ok(ApiResponse<AuthResultDto>.Success(result));
        }

        [HttpPost("staff/login")]
        public async Task<IActionResult> StaffLogin([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginStaffAsync(request);
            if (result == null)
            {
                return Unauthorized(ApiResponse<AuthResultDto>.Failure("Email hoặc mật khẩu không chính xác."));
            }
            return Ok(ApiResponse<AuthResultDto>.Success(result));
        }
    }
}
