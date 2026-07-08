using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrDepartmentApprover
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string ApproverType { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,0)")]
        public decimal EmployeeId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
