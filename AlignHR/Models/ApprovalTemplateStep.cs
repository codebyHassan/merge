using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class ApprovalTemplateStep
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public ApprovalTemplate? Template { get; set; }

        public int StepNo { get; set; }

        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        public ApprovalType ApprovalType { get; set; } = ApprovalType.Single;

        public bool IsOptional { get; set; } = false;

        [StringLength(150)]
        public string? StepLabel { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
    }
}
