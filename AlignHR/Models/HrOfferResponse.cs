using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOfferResponse
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? OfferFk { get; set; }

        [Required]
        [StringLength(50)]
        public string ResponseType { get; set; } = string.Empty;

        public DateTime ResponseDate { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Comments { get; set; }

        public HrOffer? Offer { get; set; }
    }
}
