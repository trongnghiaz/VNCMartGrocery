
using VNC.Application.Models.Carts;

namespace VNC.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int customerId);
        Task<CartDto> AddToCartAsync(int customerId, AddToCartRequest request);
        Task<CartDto> UpdateQuantityAsync(int customerId, int productId, int quantity);
        Task<CartDto> RemoveFromCartAsync(int customerId, int productId);
    }
}
