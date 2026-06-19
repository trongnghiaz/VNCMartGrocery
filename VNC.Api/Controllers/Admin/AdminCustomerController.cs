using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Admin.Customers;

namespace VNC.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/customers")]
    [Authorize(Roles = "Admin")]
    public class AdminCustomerController : ControllerBase
    {
        private readonly IAdminCustomerService _adminCustomerService;

        public AdminCustomerController(IAdminCustomerService adminCustomerService)
        {
            _adminCustomerService = adminCustomerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] GetAdminCustomersRequest request)
        {
            var result = await _adminCustomerService.GetCustomersAsync(request);
            return Ok(ApiResponse<PagedResult<AdminCustomerDto>>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _adminCustomerService.GetCustomerByIdAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<AdminCustomerDetailDto>.Failure("Không tìm thấy khách hàng."));
            }

            return Ok(ApiResponse<AdminCustomerDetailDto>.Success(result));
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockCustomer(int id)
        {
            var success = await _adminCustomerService.LockCustomerAsync(id);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy khách hàng để khóa."));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPut("{id}/unlock")]
        public async Task<IActionResult> UnlockCustomer(int id)
        {
            var success = await _adminCustomerService.UnlockCustomerAsync(id);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.Failure("Không tìm thấy khách hàng để mở khóa."));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}