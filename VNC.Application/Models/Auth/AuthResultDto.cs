
namespace VNC.Application.Models.Auth
{
    public class AuthResultDto
    {
        public int Id { get; set; }
        public string Account { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Customer" hoặc tên Role cụ thể của Staff
        public bool IsStaff { get; set; } // Để Frontend biết nên chuyển hướng vào trang Admin hay Client
        public string Token { get; set; } = string.Empty;
    }
}
