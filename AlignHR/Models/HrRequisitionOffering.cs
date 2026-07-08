using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrRequisitionOffering
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal RequisitionId { get; set; }

        [Required]
        [StringLength(200)]
        public string OfferingName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public HrRequisition? Requisition { get; set; }
    }
}
