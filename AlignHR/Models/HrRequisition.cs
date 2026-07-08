using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrRequisition
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [StringLength(30)]
        public string? RequisitionNo { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? EmployeeFK { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? DepartmentFK { get; set; }

        [Column(TypeName = "date")]
        public DateTime? InitialDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PromisedDate { get; set; }

        [StringLength(50)]
        public string? RequisitionType { get; set; }

        [StringLength(50)]
        public string? Nature { get; set; }

        [StringLength(200)]
        public string? PositionTitle { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BudgetAmountPerMonth { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? ReplacementEmployeeId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? TransferFromDepartmentId { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Draft";

        [Column(TypeName = "numeric(18,0)")]
        public decimal? WorkflowInstanceFK { get; set; }

        public ICollection<HrRequisitionSkill> Skills { get; set; } = new List<HrRequisitionSkill>();
        public ICollection<HrRequisitionOffering> Offerings { get; set; } = new List<HrRequisitionOffering>();

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
