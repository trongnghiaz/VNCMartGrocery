

namespace VNC.Application.Models.Products
{
    public class UpdateProductRequest : CreateProductRequest
    {
        public bool IsVisible { get; set; } = true;
    }
}
