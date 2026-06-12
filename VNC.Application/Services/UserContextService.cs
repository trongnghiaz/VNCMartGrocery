
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VNC.Application.Interfaces;

namespace VNC.Application.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Hàm tự động bóc tách ID người dùng từ Token
        public int? GetUserId()
        {
            // Lấy ô dữ liệu ClaimTypes.NameIdentifier mà ta đã ký số lúc Đăng nhập
            var nameIdentifier = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(nameIdentifier, out int userId))
            {
                return userId;
            }

            return null;
        }

        // Hàm lấy quyền hiện tại (ví dụ: "Customer" hoặc "Admin")
        public string? GetUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
