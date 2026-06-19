using VNC.Application.Models.Orders;

namespace VNC.Application.Interfaces
{
    public interface IOrderService
    {
        // Hàm xử lý sinh mã đơn hàng tiếp theo trong ngày
        Task<string> GenerateOrderCodeAsync(string storeCode, string branchCode);

        Task<string> CreateOrderAsync(CreateOrderDto dto, int customerId);
    }
}
