

using VNC.Application.Models.Auth;

namespace VNC.Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterCustomerAsync(RegisterRequest request);
        Task<AuthResultDto?> LoginCustomerAsync(LoginRequest request);
        Task<AuthResultDto?> LoginStaffAsync(LoginRequest request);
        Task<AuthResultDto?> GetOTPAsync(LoginRequest request);
    }
}
