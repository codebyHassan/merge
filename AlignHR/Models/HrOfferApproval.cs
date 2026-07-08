using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOfferApproval
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? OfferFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? ApproverEmployeeFk { get; set; }

        public int? ApprovalLevel { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Pending";

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime? ActionDate { get; set; }

        public HrOffer? Offer { get; set; }
    }

}
