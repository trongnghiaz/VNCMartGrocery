using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNC.Domain.Entities
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        [StringLength(250)]
        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();
    }
}
