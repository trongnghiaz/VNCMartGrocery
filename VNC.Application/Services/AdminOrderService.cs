using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Admin.Orders;
using VNC.Domain.Enumerations;

namespace VNC.Application.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IAppDbContext _context;

        public AdminOrderService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AdminOrderDto>> GetOrdersAsync(GetAdminOrdersRequest request)
        {
            request.PageNumber = Math.Max(1, request.PageNumber);
            request.PageSize = Math.Clamp(request.PageSize, 1, 100);

            var query = _context.Orders
                .Include(o => o.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();

                query = query.Where(o =>
                    o.OrderCode.ToLower().Contains(searchTerm)
                    || o.ReceiverName.ToLower().Contains(searchTerm)
                    || o.ReceiverPhone.ToLower().Contains(searchTerm)
                    || (o.Customer.FullName != null && o.Customer.FullName.ToLower().Contains(searchTerm)));
            }

            if (request.OrderStatusValue.HasValue)
            {
                var status = OrderStatusEnum.FromValue(request.OrderStatusValue.Value);
                query = query.Where(o => o.OrderStatus == status);
            }

            if (request.PaymentStatusValue.HasValue)
            {
                var status = PaymentStatusEnum.FromValue(request.PaymentStatusValue.Value);
                query = query.Where(o => o.PaymentStatus == status);
            }

            if (request.FromDate.HasValue)
            {
                var fromDate = request.FromDate.Value.Date;
                query = query.Where(o => o.OrderDate >= fromDate);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(o => o.OrderDate < toDate);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = orders.Select(o => new AdminOrderDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.FullName,
                OrderDate = o.OrderDate,
                OrderStatus = o.OrderStatus.Name,
                PaymentMethod = o.PaymentMethod.Name,
                PaymentStatus = o.PaymentStatus.Name,
                TotalOriginalAmount = o.TotalOriginalAmount,
                DiscountAmount = o.DiscountAmount,
                ShippingFee = o.ShippingFee,
                TotalPayAmount = o.TotalPayAmount
            }).ToList();

            return new PagedResult<AdminOrderDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<AdminOrderDetailDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            return new AdminOrderDetailDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.FullName,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus.Name,
                PaymentMethod = order.PaymentMethod.Name,
                PaymentStatus = order.PaymentStatus.Name,
                TotalOriginalAmount = order.TotalOriginalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalPayAmount = order.TotalPayAmount,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ShippingAddress = order.ShippingAddress,
                Note = order.Note,
                Items = order.OrderItems.Select(oi => new AdminOrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Amount = oi.Amount
                }).ToList()
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, short orderStatusValue)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return false;
            }

            order.OrderStatus = OrderStatusEnum.FromValue(orderStatusValue);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePaymentStatusAsync(int orderId, short paymentStatusValue)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return false;
            }

            order.PaymentStatus = PaymentStatusEnum.FromValue(paymentStatusValue);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}