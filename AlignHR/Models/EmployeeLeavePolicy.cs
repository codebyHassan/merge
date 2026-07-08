using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class EmployeeLeavePolicy
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public decimal LeavePolicyId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // ===== Navigation =====
        [ForeignKey("LeavePolicyId")]
        public LeavePolicy? LeavePolicy { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
