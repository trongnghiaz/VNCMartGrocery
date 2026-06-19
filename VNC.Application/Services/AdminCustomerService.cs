using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models;
using VNC.Application.Models.Admin.Customers;
using VNC.Domain.Enumerations;

namespace VNC.Application.Services
{
    public class AdminCustomerService : IAdminCustomerService
    {
        private readonly IAppDbContext _context;

        public AdminCustomerService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AdminCustomerDto>> GetCustomersAsync(GetAdminCustomersRequest request)
        {
            request.PageNumber = Math.Max(1, request.PageNumber);
            request.PageSize = Math.Clamp(request.PageSize, 1, 100);

            var query = _context.Customers
                .Include(c => c.Orders)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();

                query = query.Where(c =>
                    c.PhoneNumber.ToLower().Contains(searchTerm)
                    || (c.FullName != null && c.FullName.ToLower().Contains(searchTerm))
                    || (c.Email != null && c.Email.ToLower().Contains(searchTerm)));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync();

            var customers = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = customers.Select(c => new AdminCustomerDto
            {
                CustomerId = c.CustomerId,
                PhoneNumber = c.PhoneNumber,
                FullName = c.FullName,
                Email = c.Email,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                TotalOrders = c.Orders.Count,
                TotalSpent = c.Orders
                    .Where(o => o.OrderStatus != OrderStatusEnum.Cancelled)
                    .Sum(o => o.TotalPayAmount)
            }).ToList();

            return new PagedResult<AdminCustomerDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<AdminCustomerDetailDto?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Addresses)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return null;
            }

            return new AdminCustomerDetailDto
            {
                CustomerId = customer.CustomerId,
                PhoneNumber = customer.PhoneNumber,
                FullName = customer.FullName,
                Email = customer.Email,
                Gender = customer.Gender,
                DateOfBirth = customer.DateOfBirth,
                ZaloId = customer.ZaloId,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                TotalOrders = customer.Orders.Count,
                TotalSpent = customer.Orders
                    .Where(o => o.OrderStatus != OrderStatusEnum.Cancelled)
                    .Sum(o => o.TotalPayAmount),
                Addresses = customer.Addresses.Select(a => new AdminCustomerAddressDto
                {
                    CustomerAddressId = a.AddressId,
                    ReceiverName = a.ReceiverName,
                    ReceiverPhone = a.ReceiverPhone,
                    FullAddress = a.SpecificAddress,
                    IsDefault = a.IsDefault
                }).ToList()
            };
        }

        public async Task<bool> LockCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return false;
            }

            customer.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlockCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return false;
            }

            customer.IsActive = true;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}