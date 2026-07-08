using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class LeaveBalance
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveTypeId { get; set; }
        
        // Optional: Year-based tracking
        public int Year { get; set; } 

        // ===== Balances =====
        [Column(TypeName = "decimal(5,2)")]
        public decimal Allocated { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Used { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Pending { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CarriedForward { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Available { get; set; } = 0;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey("LeaveTypeId")]
        public LeaveType LeaveType { get; set; }

        // NOTE: Assuming you already have Employee table
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
