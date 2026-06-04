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
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                )
            );
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            // 2. Đăng ký các Repositories hoặc Infrastructure Services của bạn ở đây 
            

            return services;
        }
    }
}
