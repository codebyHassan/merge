using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOffer
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? JobApplicationFk { get; set; }

        [StringLength(50)]
        public string? OfferNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProposedSalary { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Draft";

        [StringLength(64)]
        public string? ResponseToken { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? Notes { get; set; }

        public DateTime? ResponseDate { get; set; }

        [StringLength(1000)]
        public string? ResponseComments { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CandidateJoiningDate { get; set; }

        public HrJobApplication? JobApplication { get; set; }
        public ICollection<HrOfferVersion> Versions { get; set; } = new List<HrOfferVersion>();
        public ICollection<HrOfferApproval> Approvals { get; set; } = new List<HrOfferApproval>();
        public ICollection<HrOfferDocument> Documents { get; set; } = new List<HrOfferDocument>();
    }
}
