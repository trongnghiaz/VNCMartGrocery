using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Admin;

namespace VNC.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] int lowStockThreshold = 10)
        {
            var result = await _dashboardService.GetSummaryAsync(lowStockThreshold);
            return Ok(ApiResponse<DashboardSummaryDto>.Success(result));
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _dashboardService.GetRevenueChartAsync(fromDate, toDate);
            return Ok(ApiResponse<List<RevenueChartDto>>.Success(result));
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int top = 5)
        {
            var result = await _dashboardService.GetTopProductsAsync(top);
            return Ok(ApiResponse<List<TopProductDto>>.Success(result));
        }

        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders([FromQuery] int take = 10)
        {
            var result = await _dashboardService.GetRecentOrdersAsync(take);
            return Ok(ApiResponse<List<RecentOrderDto>>.Success(result));
        }
    }
}