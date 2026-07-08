using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrCandidateDocument
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? CandidateFk { get; set; }

        [StringLength(50)]
        public string? DocumentType { get; set; }

        [StringLength(500)]
        public string? FileName { get; set; }

        [StringLength(1000)]
        public string? FilePath { get; set; }

        public DateTime? UploadedDate { get; set; }

        public HrCandidate? Candidate { get; set; }
    }
}
