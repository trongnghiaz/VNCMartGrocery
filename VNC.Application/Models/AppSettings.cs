

namespace VNC.Application.Models
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; } = new();

    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; }
    }
}
