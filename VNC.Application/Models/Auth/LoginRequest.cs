
using System.ComponentModel.DataAnnotations;

namespace VNC.Application.Models.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Tài khoản đăng nhập không được để trống.")]
        public string Account { get; set; } = null!; // Có thể là PhoneNumber đối với Customer hoặc Email đối với Staff

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; } = null!;
    }
}
