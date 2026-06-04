
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNC.Domain.Entities
{
    [Table("OrderSequences")]
    public class OrderSequence
    {
        [Required]
        [StringLength(20)]
        public string StoreCode { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string BranchCode { get; set; } = null!;

        [Required]
        public DateTime OrderDate { get; set; }

        public int CurrentValue { get; set; } = 0;
    }
}
