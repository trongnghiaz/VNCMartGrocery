namespace VNC.Application.Models.Orders
{
    public class QrPaymentResultDto
    {
        public string QrCodeUrl { get; set; } = string.Empty;   
        public string QrDataString { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}