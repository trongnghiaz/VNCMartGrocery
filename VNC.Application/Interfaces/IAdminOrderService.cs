using VNC.Application.Models;
using VNC.Application.Models.Admin.Orders;

namespace VNC.Application.Interfaces
{
    public interface IAdminOrderService
    {
        Task<PagedResult<AdminOrderDto>> GetOrdersAsync(GetAdminOrdersRequest request);

        Task<AdminOrderDetailDto?> GetOrderByIdAsync(int orderId);

        Task<bool> UpdateOrderStatusAsync(int orderId, short orderStatusValue);

        Task<bool> UpdatePaymentStatusAsync(int orderId, short paymentStatusValue);
    }
}