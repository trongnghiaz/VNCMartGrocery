
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNC.Domain.Entities
{
    [Table("CustomerAddresses")]
    public class CustomerAddress
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string ReceiverName { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string ReceiverPhone { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string SpecificAddress { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Wards { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string District { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Province { get; set; } = null!;

        public bool IsDefault { get; set; } = false;

        // Navigation property
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;
    }
}
