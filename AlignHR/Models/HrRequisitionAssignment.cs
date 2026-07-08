using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrRequisitionAssignment
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? RequisitionFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? AssignedByEmployeeId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? RecruiterEmployeeFK { get; set; }

        public DateTime? AssignedDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
