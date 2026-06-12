
using System.ComponentModel.DataAnnotations;

namespace VNC.Application.Models.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được quá 15 ký tự.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        [StringLength(100)]
        public string? Email { get; set; }
    }
}
