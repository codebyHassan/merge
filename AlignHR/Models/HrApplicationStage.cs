using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrApplicationStage
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StageName { get; set; } = string.Empty;

        public int? StageOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
