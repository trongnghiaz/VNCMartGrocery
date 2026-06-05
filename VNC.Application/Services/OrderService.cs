
using Microsoft.EntityFrameworkCore;
using VNC.Application.Dtos;
using VNC.Application.Interfaces;
using VNC.Domain.Entities;
using VNC.Domain.Enumerations;

namespace VNC.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAppDbContext _context;

        public OrderService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateOrderCodeAsync(string storeCode, string branchCode)
        {
            var today = DateTime.Today; // Lấy ngày hiện tại (chỉ giữ phần ngày, giờ là 00:00:00)
            int nextValue = 1;

            // Sử dụng Database Transaction để đảm bảo an toàn dữ liệu khi có nhiều request đồng thời
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Tìm dòng số thứ tự của cửa hàng + chi nhánh trong ngày hôm nay
                var sequence = await _context.OrderSequences
                    .FirstOrDefaultAsync(os => os.StoreCode == storeCode
                                            && os.BranchCode == branchCode
                                            && os.OrderDate == today);

                if (sequence == null)
                {
                    // Nếu chưa có (đơn hàng đầu tiên của ngày mới), tiến hành khởi tạo dòng mới với giá trị khởi điểm = 1
                    sequence = new OrderSequence
                    {
                        StoreCode = storeCode,
                        BranchCode = branchCode,
                        OrderDate = today,
                        CurrentValue = 1
                    };
                    _context.OrderSequences.Add(sequence);
                }
                else
                {
                    // Nếu đã có, tăng số thứ tự lên 1
                    sequence.CurrentValue += 1;
                    nextValue = sequence.CurrentValue;
                }

                // Lưu thay đổi xuống Database
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            // Định dạng chuỗi số thứ tự thành 4 chữ số (ví dụ: 1 -> "0001", 12 -> "0012")
            string sequenceStr = nextValue.ToString("D4");

            // Định dạng chuỗi ngày tháng YYYYMMDD (ví dụ: 20260604)
            string dateStr = today.ToString("yyyyMMdd");

            // Trả về mã hoàn chỉnh theo đúng cấu trúc: VNC-CS1-20260604-0001
            return $"{storeCode}-{branchCode}-{dateStr}-{sequenceStr}";
        }
        public async Task<string> CreateOrderAsync(CreateOrderDto dto)
        {
            // 1. Sinh mã đơn hàng tự động (Giả định Store: VNC, Chi nhánh: CS1)
            string orderCode = await GenerateOrderCodeAsync("VNC", "CS1");

            decimal totalOriginalAmount = 0;
            var orderItems = new List<OrderItem>();

            // 2. Duyệt qua danh sách sản phẩm để tính tiền dựa trên giá trị thực tế trong DB (tránh client sửa giá)
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Sản phẩm với ID {item.ProductId} không tồn tại.");

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Sản phẩm {product.ProductName} không đủ tồn kho.");

                // Trừ kho sản phẩm
                product.StockQuantity -= item.Quantity;

                var amount = product.Price * item.Quantity;
                totalOriginalAmount += amount;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Amount = amount
                });
            }

            // 3. Khởi tạo thực thể Đơn hàng (Order Entity)
            var order = new Order
            {
                OrderCode = orderCode,
                CustomerId = dto.CustomerId,
                OrderStatus = OrderStatusEnum.Processing,
                PaymentMethod = PaymentMethodEnum.COD,
                PaymentStatus = PaymentStatusEnum.Unpaid,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                ShippingAddress = dto.ShippingAddress,
                TotalOriginalAmount = totalOriginalAmount,
                TotalPayAmount = totalOriginalAmount, // Tạm thời chưa tính toán giảm giá/phí ship
                Note = dto.Note,
                OrderItems = orderItems
            };

            // 4. Lưu vào Database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return orderCode; // Trả về mã đơn hàng thành công cho API
        }
    }
}
