
using Microsoft.EntityFrameworkCore;
using VNC.Application.Interfaces;
using VNC.Application.Models.Orders;
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
        public async Task<string> CreateOrderAsync(CreateOrderDto dto, int customerId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                string orderCode = await GenerateOrderCodeInternalAsync("VNC", "CS1");

                decimal totalOriginalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in dto.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                        throw new Exception($"Sản phẩm với ID {item.ProductId} không tồn tại.");

                    if (product.StockQuantity < item.Quantity)
                        throw new Exception($"Sản phẩm {product.ProductName} không đủ tồn kho.");

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

                var order = new Order
                {
                    OrderCode = orderCode,
                    CustomerId = customerId,
                    OrderStatus = OrderStatusEnum.Processing,
                    PaymentMethod = PaymentMethodEnum.FromName(dto.PaymentMethod),
                    PaymentStatus = PaymentStatusEnum.Unpaid,
                    ReceiverName = dto.ReceiverName,
                    ReceiverPhone = dto.ReceiverPhone,
                    ShippingAddress = dto.ShippingAddress,
                    TotalOriginalAmount = totalOriginalAmount,
                    TotalPayAmount = totalOriginalAmount,
                    Note = dto.Note,
                    OrderItems = orderItems
                };

                // 4. Lưu vào Database
                _context.Orders.Add(order);

                // 5. 🔥 BỔ SUNG: DỌN SẠCH GIỎ HÀNG CỦA KHÁCH HÀNG
                
                var productIdsInOrder = dto.Items.Select(i => i.ProductId).ToList();
                var cartItemsToRemove = await _context.CartItems
                    .Where(ci => ci.Cart.CustomerId == customerId && productIdsInOrder.Contains(ci.ProductId))
                    .ToListAsync();

                if (cartItemsToRemove.Any())
                {
                    _context.CartItems.RemoveRange(cartItemsToRemove);
                }

                // 6. Lưu tất cả thay đổi xuống Database (Trừ kho, Thêm đơn, Xóa giỏ)
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return orderCode;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task<string> GenerateOrderCodeInternalAsync(string storeCode, string branchCode)
        {
            var today = DateTime.Today;
            int nextValue = 1;

            var sequence = await _context.OrderSequences
                .FirstOrDefaultAsync(os => os.StoreCode == storeCode
                                        && os.BranchCode == branchCode
                                        && os.OrderDate == today);

            if (sequence == null)
            {
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
                sequence.CurrentValue += 1;
                nextValue = sequence.CurrentValue;
            }

            string sequenceStr = nextValue.ToString("D4");
            string dateStr = today.ToString("yyyyMMdd");

            return $"{storeCode}-{branchCode}-{dateStr}-{sequenceStr}";
        }
    }
}
