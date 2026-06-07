
using Microsoft.Extensions.DependencyInjection;
using VNC.Application.Interfaces;
using VNC.Application.Services;

namespace VNC.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 1. Tự động đăng ký AutoMapper (nếu bạn dùng để map Entity sang DTO)
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // 2. Đăng ký các nghiệp vụ ứng dụng (Application Services)
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            // 3. Nếu bạn dùng MediatR cho mẫu thiết kế CQRS (Rất phổ biến trong Clean Architecture)
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


            return services;
        }
    }
}
