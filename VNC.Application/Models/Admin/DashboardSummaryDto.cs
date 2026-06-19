

namespace VNC.Application.Models.Admin
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }

        public int TotalOrders { get; set; }

        public int TodayOrders { get; set; }

        public int PendingOrders { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }
    }
}
