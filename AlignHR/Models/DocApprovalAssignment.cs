using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class DocApprovalAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        /// <summary>Matches DmsDefinationFile.DefinitionName</summary>
        [Required]
        [StringLength(150)]
        public string DmsDefinitionName { get; set; } = string.Empty;

        [Required]
        public int ApprovalTemplateId { get; set; }

        [ForeignKey("ApprovalTemplateId")]
        public ApprovalTemplate? ApprovalTemplate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

    }
}
