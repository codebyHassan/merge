using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOfferVersion
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? OfferFk { get; set; }

        public int? VersionNo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Salary { get; set; }

        [Column(TypeName = "date")]
        public DateTime? JoiningDate { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public HrOffer? Offer { get; set; }
    }
}
