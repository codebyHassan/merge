using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrOfferDocument
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? OfferFk { get; set; }

        [StringLength(500)]
        public string? FileName { get; set; }

        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime? GeneratedDate { get; set; }

        public HrOffer? Offer { get; set; }
    }
}
