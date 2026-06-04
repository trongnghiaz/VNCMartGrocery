using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using VNC.Domain.Entities;

namespace VNC.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Role> Roles { get; set; }
        DbSet<Staff> Staffs { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<CustomerAddress> CustomerAddresses { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductImage> ProductImages { get; set; }
        DbSet<Voucher> Vouchers { get; set; }
        DbSet<Cart> Carts { get; set; }
        DbSet<CartItem> CartItems { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderItem> OrderItems { get; set; }
        DbSet<OrderSequence> OrderSequences { get; set; }
        DatabaseFacade Database { get; } // Giúp tầng Application gọi được .Database.BeginTransactionAsync()
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
