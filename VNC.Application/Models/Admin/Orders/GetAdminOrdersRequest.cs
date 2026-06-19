namespace VNC.Application.Models.Admin.Orders
{
    public class GetAdminOrdersRequest
    {
        public string? SearchTerm { get; set; }

        public short? OrderStatusValue { get; set; }

        public short? PaymentStatusValue { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}