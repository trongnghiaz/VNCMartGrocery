using VNC.Application.Models;
using VNC.Application.Models.Admin.Customers;

namespace VNC.Application.Interfaces
{
    public interface IAdminCustomerService
    {
        Task<PagedResult<AdminCustomerDto>> GetCustomersAsync(GetAdminCustomersRequest request);

        Task<AdminCustomerDetailDto?> GetCustomerByIdAsync(int customerId);

        Task<bool> LockCustomerAsync(int customerId);

        Task<bool> UnlockCustomerAsync(int customerId);
    }
}