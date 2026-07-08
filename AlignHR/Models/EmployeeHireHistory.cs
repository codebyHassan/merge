using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class EmployeeHireHistory
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [StringLength(20)]
        public string HireType { get; set; } = "New";  // New, Replacement, Transfer

        [Column(TypeName = "numeric(18,0)")]
        public decimal? CandidateFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? OfferId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? RequisitionFk { get; set; }

        public int? ReplacedEmployeeId { get; set; }
        [ForeignKey("ReplacedEmployeeId")]
        public Employee? ReplacedEmployee { get; set; }

        public int? FromDepartmentId { get; set; }
        [ForeignKey("FromDepartmentId")]
        public Department? FromDepartment { get; set; }

        [Column(TypeName = "date")]
        public DateOnly EffectiveDate { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
