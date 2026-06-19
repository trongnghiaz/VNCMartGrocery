

namespace VNC.Application.Models
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; } = new();
        public VietQRSettings VietQRSettings { get; set; } = new();
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; }
    }
    public class VietQRSettings
    {
        public string BankBin { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Template { get; set; } = "compact2";
        public string QrUrlTemplate { get; set; } = string.Empty; // 🔥 BỔ SUNG THUỘC TÍNH MỚI
    }
}
