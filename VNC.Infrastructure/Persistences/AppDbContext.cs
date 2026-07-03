
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VNC.Application.Interfaces;
using VNC.Domain.Entities;
using VNC.Domain.Enumerations;

namespace VNC.Infrastructure.Persistences
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo các DbSet để EF Core hiểu và map bảng
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Staff> Staffs { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<Voucher> Vouchers { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<OrderSequence> OrderSequences { get; set; } = null!;
        public DbSet<OTPEntry> OTPEntries { get; set; } = null!;
        public DatabaseFacade Database => base.Database; // Triển khai Database từ IAppDbContext
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Khóa chính phức hợp cho bảng OrderSequences
            modelBuilder.Entity<OrderSequence>()
                .HasKey(os => new { os.StoreCode, os.BranchCode, os.OrderDate });

            // Đảm bảo kiểu DATE trong SQL tương thích tốt với DateTime (chỉ lấy phần ngày) trong C#
            modelBuilder.Entity<OrderSequence>()
                .Property(os => os.OrderDate)
                .HasAnnotation("Relational:ColumnType", "date");
            modelBuilder.Entity<Order>(builder =>
            {
                builder.Property(o => o.PaymentStatus)
                    // Hướng dẫn EF Core cách chuyển đổi (Conversion)
                    .HasConversion(
                        status => status.Value, // Khi LƯU: Lấy thuộc tính Value (int) để lưu vào DB
                        value => PaymentStatusEnum.FromValue(value) // Khi ĐỌC: Dùng hàm FromValue để dựng lại Object
                    )
                    .IsRequired();
                builder.Property(o => o.OrderStatus)
                    .HasConversion(
                        status => status.Value,
                        value => OrderStatusEnum.FromValue(value)
                    )
                    .IsRequired();
                builder.Property(o => o.PaymentMethod)
                    .HasConversion(
                        status => status.Value,
                        value => PaymentMethodEnum.FromValue(value)
                    )
                    .IsRequired();
            });
        }
    }
}
