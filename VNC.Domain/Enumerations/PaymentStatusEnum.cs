
namespace VNC.Domain.Enumerations
{
    public class PaymentStatusEnum : Enumeration
    {
        public static readonly PaymentStatusEnum Unpaid = new(1, "Chưa thanh toán");
        public static readonly PaymentStatusEnum Paid = new(2, "Đã thanh toán");
        protected PaymentStatusEnum() { }
        public PaymentStatusEnum(short value, string name) : base(value, name)
        {
        }
        public static IEnumerable<PaymentStatusEnum> List() => new[] { Unpaid, Paid };
        public static PaymentStatusEnum FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (state == null)
            {
                throw new ArgumentException($"Possible values for PaymentStatusEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
        public static PaymentStatusEnum FromValue(short value)
        {
            var state = List().SingleOrDefault(s => s.Value == value);
            if (state == null)
            {
                throw new ArgumentException($"Possible values for PaymentStatusEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
    }
}
