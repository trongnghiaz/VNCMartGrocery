using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models.Admin;
using VNC.Domain.Enumerations;

namespace VNC.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAppDbContext _context;

        public AdminDashboardService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int lowStockThreshold = 10)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var totalRevenue = await _context.Orders
                .Where(o => o.OrderStatus != OrderStatusEnum.Cancelled)
                .SumAsync(o => o.TotalPayAmount);

            var totalOrders = await _context.Orders.CountAsync();

            var todayOrders = await _context.Orders
                .CountAsync(o => o.OrderDate >= today && o.OrderDate < tomorrow);

            var pendingOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == OrderStatusEnum.Pending);

            var totalCustomers = await _context.Customers.CountAsync();

            var totalProducts = await _context.Products.CountAsync();

            var lowStockProducts = await _context.Products
                .CountAsync(p => p.StockQuantity <= lowStockThreshold);

            return new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TodayOrders = todayOrders,
                PendingOrders = pendingOrders,
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                LowStockProducts = lowStockProducts
            };
        }

        public async Task<List<RevenueChartDto>> GetRevenueChartAsync(DateTime? fromDate, DateTime? toDate)
        {
            var from = fromDate?.Date ?? DateTime.Today.AddDays(-30);
            var to = toDate?.Date.AddDays(1) ?? DateTime.Today.AddDays(1);

            return await _context.Orders
                .Where(o => o.OrderDate >= from
                         && o.OrderDate < to
                         && o.OrderStatus != OrderStatusEnum.Cancelled)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new RevenueChartDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPayAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<TopProductDto>> GetTopProductsAsync(int top = 5)
        {
            top = Math.Clamp(top, 1, 50);

            return await _context.OrderItems.AsNoTracking().Include(oi => oi.Order)
                .Where(oi => oi.Order.OrderStatus != OrderStatusEnum.Cancelled)
                .GroupBy(oi => new
                {
                    oi.ProductId,
                    oi.ProductName
                })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(top)
                .ToListAsync();
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int take = 10)
        {
            take = Math.Clamp(take, 1, 50);

            var orders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(take)
                .ToListAsync();

            return orders.Select(o => new RecentOrderDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.FullName,
                OrderDate = o.OrderDate,
                OrderStatus = o.OrderStatus.Name,
                PaymentStatus = o.PaymentStatus.Name,
                TotalPayAmount = o.TotalPayAmount
            }).ToList();
        }
    }
}