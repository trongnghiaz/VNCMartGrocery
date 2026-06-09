
using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models.Carts;
using VNC.Domain.Entities;

namespace VNC.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IAppDbContext _context;

        public CartService(IAppDbContext context)
        {
            _context = context;
        }

        // 1. LẤY THÔNG TIN GIỎ HÀNG (Nếu chưa có giỏ thì tự động tạo mới giỏ trống)
        public async Task<CartDto> GetCartAsync(int customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product) // Eager loading lấy thông tin sản phẩm để tính giá
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            // Khéo léo: Nếu người dùng chưa từng có giỏ hàng, tạo ngay cho họ một giỏ trống trong DB
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId, UpdatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return MapToDto(cart);
        }

        // 2. THÊM SẢN PHẨM VÀO GIỎ HÀNG
        public async Task<CartDto> AddToCartAsync(int customerId, AddToCartRequest request)
        {
            // Lấy giỏ hàng hiện tại (hoặc tạo mới nếu chưa có)
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId, UpdatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem sản phẩm này đã nằm trong giỏ hàng từ trước chưa
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Nếu đã có, chỉ cần cộng dồn số lượng mới vào số lượng cũ
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                // Nếu chưa có, tạo một CartItem mới liên kết vào giỏ
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                _context.CartItems.Add(newItem);
            }

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return await GetCartAsync(customerId);
        }

        // 3. CẬP NHẬT SỐ LƯỢNG MÓN HÀNG (Admin hoặc User gõ trực tiếp số lượng ở ô Input)
        public async Task<CartDto> UpdateQuantityAsync(int customerId, int productId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null) throw new Exception("Không tìm thấy giỏ hàng.");

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    // Nếu số lượng đưa về 0 hoặc nhỏ hơn, tiến hành xóa hẳn khỏi giỏ
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return await GetCartAsync(customerId);
        }

        // 4. XÓA MÓN HÀNG KHỎI GIỎ (Bấm nút Thùng rác)
        public async Task<CartDto> RemoveFromCartAsync(int customerId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null) throw new Exception("Không tìm thấy giỏ hàng.");

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return await GetCartAsync(customerId);
        }

        // Hàm Helper dùng chung để biến đổi cấu trúc Entity sang DTO an toàn
        private CartDto MapToDto(Cart cart)
        {
            return new CartDto
            {
                CartId = cart.CartId,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.ProductName ?? "Sản phẩm không xác định",
                    ThumbnailUrl = ci.Product?.ThumbnailUrl,
                    Price = ci.Product?.Price ?? 0,
                    Quantity = ci.Quantity
                }).ToList()
            };
        }
    }
}
