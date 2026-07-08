using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrHiringDecision
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? JobApplicationFk { get; set; }

        [Required]
        [StringLength(50)]
        public string Decision { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,0)")]
        public decimal? DecisionBy { get; set; }

        public DateTime? DecisionDate { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }

        public HrJobApplication? JobApplication { get; set; }
    }
}
