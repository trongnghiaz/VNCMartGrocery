using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Auth;
using VNC.Domain.Entities;

namespace VNC.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppDbContext _context;
        private readonly AppSettings _setting;

        public AuthService(IAppDbContext context, IOptions<AppSettings> options)
        {
            _context = context;
            _setting = options.Value;
        }

        // 1. ĐĂNG KÝ TÀI KHOẢN KHÁCH HÀNG
        public async Task<bool> RegisterCustomerAsync(RegisterRequest request)
        {
            var isPhoneNumberExist = await _context.Customers
                .AnyAsync(c => c.PhoneNumber == request.PhoneNumber);

            if (isPhoneNumberExist)
            {
                throw new Exception("Số điện thoại này đã được sử dụng trên hệ thống.");
            }

            string securePasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newCustomer = new Customer
            {
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = securePasswordHash,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
            return true;
        }

        // 2. ĐĂNG NHẬP KHÁCH HÀNG
        public async Task<AuthResultDto?> LoginCustomerAsync(LoginRequest request)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == request.Account && c.IsActive);

            if (customer == null || string.IsNullOrEmpty(customer.PasswordHash))
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            // Khởi tạo DTO kết quả
            var result = new AuthResultDto
            {
                Id = customer.CustomerId,
                Account = customer.PhoneNumber,
                FullName = customer.FullName ?? "Khách hàng",
                Role = "Customer",
                IsStaff = false
            };

            // Sinh chuỗi Token và gán vào DTO
            result.Token = GenerateJwtToken(result);
            return result;
        }

        // 3. ĐĂNG NHẬP NHÂN VIÊN
        public async Task<AuthResultDto?> LoginStaffAsync(LoginRequest request)
        {
            var staff = await _context.Staffs
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.Email == request.Account && s.IsActive);

            if (staff == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, staff.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }
            if (staff.Role == null || string.IsNullOrEmpty(staff.Role.RoleName)) //[cite: 3, 4]
            {
                throw new UnauthorizedAccessException("Tài khoản nhân viên của bạn hiện chưa được cấp quyền truy cập hệ thống. Vui lòng liên hệ Admin.");
            }
            var result = new AuthResultDto
            {
                Id = staff.StaffId,
                Account = staff.Email,
                FullName = staff.FullName,
                Role = staff.Role?.RoleName ?? "Staff",
                IsStaff = true
            };

            // Sinh chuỗi Token và gán vào DTO
            result.Token = GenerateJwtToken(result);
            return result;
        }

        // HÀM BỔ TRỢ: TỰ ĐỘNG KHỞI TẠO VÀ KÝ SỐ ĐỐI TƯỢNG JWT TOKEN
        private string GenerateJwtToken(AuthResultDto user)
        {
            var secretKey = _setting.JwtSettings.SecretKey
                ?? throw new InvalidOperationException("Chưa cấu hình SecretKey cho hệ thống.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Thiết lập danh sách thông tin nhận diện (Claims) lồng vào trong Token
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("IsStaff", user.IsStaff.ToString().ToLower())
        };

            var token = new JwtSecurityToken(
                issuer: _setting.JwtSettings.Issuer,
                audience: _setting.JwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_setting.JwtSettings.ExpiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
