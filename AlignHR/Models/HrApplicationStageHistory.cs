using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrApplicationStageHistory
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? JobApplicationFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? FromStageId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? ToStageFk { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? ChangedBy { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime? ChangedDate { get; set; }

        public HrJobApplication? JobApplication { get; set; }
        public HrApplicationStage? FromStage { get; set; }
        public HrApplicationStage? ToStage { get; set; }
    }
}
