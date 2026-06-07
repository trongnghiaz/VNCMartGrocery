using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            

            return services;
        }
    }
}
