namespace VNC.Application.Models.Admin.Customers
{
    public class AdminCustomerDto
    {
        public int CustomerId { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalSpent { get; set; }
    }
}