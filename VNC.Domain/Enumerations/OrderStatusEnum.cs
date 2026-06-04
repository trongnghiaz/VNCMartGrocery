

namespace VNC.Domain.Enumerations
{
    public class OrderStatusEnum : Enumeration
    {
        public static readonly OrderStatusEnum Pending = new(1, "Chờ xác nhận");
        public static readonly OrderStatusEnum Processing = new(2, "Đang xử lý");
        public static readonly OrderStatusEnum Shipped = new(3, "Đã giao hàng");
        public static readonly OrderStatusEnum Delivered = new(4, "Đã nhận hàng");
        public static readonly OrderStatusEnum Cancelled = new(5, "Đã hủy");
        protected OrderStatusEnum() { }
        public OrderStatusEnum(int value, string name) : base(value, name)
        {
        }
        public static IEnumerable<OrderStatusEnum> List() => new[] { Pending, Processing, Shipped, Delivered, Cancelled };
        public static OrderStatusEnum FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (state == null)
            {
                throw new ArgumentException($"Possible values for OrderStatusEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
        public static OrderStatusEnum FromValue(int value)
        {
            var state = List().SingleOrDefault(s => s.Value == value);
            if (state == null)
            {
                throw new ArgumentException($"Possible values for OrderStatusEnum: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
    }
}
