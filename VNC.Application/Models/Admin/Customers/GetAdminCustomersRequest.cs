namespace VNC.Application.Models.Admin.Customers
{
    public class GetAdminCustomersRequest
    {
        public string? SearchTerm { get; set; }

        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}