
namespace VNC.Domain.Enumerations
{
    public class VoucherDiscountTypeEnum : Enumeration
    {
        public static readonly VoucherDiscountTypeEnum Percentage = new(1, "Phần trăm");
        public static readonly VoucherDiscountTypeEnum Amount = new(2, "Số tiền");
        public static readonly VoucherDiscountTypeEnum FreeShipping = new(3, "Miễn phí vận chuyển");
        public static readonly VoucherDiscountTypeEnum GiveProduct = new(4, "Mua tặng thêm");
        protected VoucherDiscountTypeEnum() { }
        public VoucherDiscountTypeEnum(int value, string name) : base(value, name)
        {
        }
        public static IEnumerable<VoucherDiscountTypeEnum> List() => new[] { Percentage, Amount, FreeShipping, GiveProduct };
        public static VoucherDiscountTypeEnum FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (state == null)
            {
                throw new ArgumentException($"Possible values for VoucherDiscountTypeEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
        public static VoucherDiscountTypeEnum FromValue(int value)
        {
            var state = List().SingleOrDefault(s => s.Value == value);
            if (state == null)
            {
                throw new ArgumentException($"Possible values for VoucherDiscountTypeEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
    }
}
