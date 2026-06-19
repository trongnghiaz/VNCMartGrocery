
using VNC.Application.Models.Admin;

namespace VNC.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int lowStockThreshold = 10);

        Task<List<RevenueChartDto>> GetRevenueChartAsync(DateTime? fromDate, DateTime? toDate);

        Task<List<TopProductDto>> GetTopProductsAsync(int top = 5);

        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int take = 10);
    }
}
