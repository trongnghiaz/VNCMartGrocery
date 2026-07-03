
namespace VNC.Domain.Entities
{
    public class OTPEntry
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string OTPCode { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
    }
}
