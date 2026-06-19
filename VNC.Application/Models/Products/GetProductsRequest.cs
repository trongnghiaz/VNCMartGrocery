
using System.ComponentModel.DataAnnotations;

namespace VNC.Application.Models.Products
{
    public class GetProductsRequest
    {
        public string? SearchTerm { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
