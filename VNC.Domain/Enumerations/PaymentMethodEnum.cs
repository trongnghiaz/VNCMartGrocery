
namespace VNC.Domain.Enumerations
{
    public class PaymentMethodEnum : Enumeration
    {
        public static readonly PaymentMethodEnum COD = new(1, "COD");
        public static readonly PaymentMethodEnum BankTransfer = new(2, "Chuyển khoản");
        protected PaymentMethodEnum() { }
        public PaymentMethodEnum(short value, string name) : base(value, name)
        {
        }
        public static IEnumerable<PaymentMethodEnum> List() => new[] { COD, BankTransfer };
        public static PaymentMethodEnum FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (state == null)
            {
                throw new ArgumentException($"Possible values for PaymentMethodEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
        public static PaymentMethodEnum FromValue(short value)
        {
            var state = List().SingleOrDefault(s => s.Value == value);
            if (state == null)
            {
                throw new ArgumentException($"Possible values for PaymentMethodEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
    
    }
}
