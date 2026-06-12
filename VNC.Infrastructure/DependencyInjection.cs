using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VNC.Application.Interfaces;
using VNC.Infrastructure.Persistences;

namespace VNC.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Cấu hình DbContext kết nối với SQL Server lấy ConnectionString từ appsettings.json
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    //b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                    sqlOptions =>
                    {
                        // Bật tính năng tự động kết nối lại khi gặp lỗi tạm thời
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,                  // Số lần thử lại tối đa (Mặc định là 6)
                            maxRetryDelay: TimeSpan.FromSeconds(30), // Thời gian chờ tối đa giữa các lần thử
                            errorNumbersToAdd: null            // Các mã lỗi SQL cụ thể muốn bắt thêm
                        );
                    }
                )
            );
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            // 2. Đăng ký các Repositories hoặc Infrastructure Services của bạn ở đây 
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Missing JWT Secret Key");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            return services;
        }
    }
}
