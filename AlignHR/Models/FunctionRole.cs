using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class FunctionRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FunctionId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("FunctionId")]
        public virtual Function? Function { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
    }
}
