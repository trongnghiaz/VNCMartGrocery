using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNC.Domain.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderCode { get; set; } = null!;

        [Required]
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public short OrderStatus { get; set; } // Chờ xác nhận, Đang xử lý...

        [Required]
        public short PaymentMethod { get; set; } // COD, Chuyển khoản...

        [Required]
        public short PaymentStatus { get; set; } // Chưa thanh toán, Đã thanh toán

        [Required]
        [StringLength(100)]
        public string ReceiverName { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string ReceiverPhone { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOriginalAmount { get; set; }

        public int? VoucherId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPayAmount { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(VoucherId))]
        public virtual Voucher? Voucher { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
