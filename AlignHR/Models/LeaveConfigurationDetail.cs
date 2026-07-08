using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    /// <summary>
    /// Child table storing the mapping between a grouping value (Department/Grade/Division)
    /// and a selected external approver employee.
    /// </summary>
    public class LeaveConfigurationDetail
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal LeaveConfigurationId { get; set; }

        /// <summary>
        /// The ValueSet Id (or Department Id) representing the grouping value.
        /// </summary>
        [Display(Name = "Group Value")]
        public int ReferenceId { get; set; }

        /// <summary>
        /// Display name of the group value (e.g. "Sales", "G1", "North").
        /// Stored for performance — avoids joins on read.
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Group Name")]
        public string? ReferenceName { get; set; }

        /// <summary>
        /// Employee code of the selected external approver.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Approver Code")]
        public string ApproverEmpNo { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the approver (cached for grid display).
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Approver Name")]
        public string? ApproverName { get; set; }

        // ===== Navigation =====

        [ForeignKey("LeaveConfigurationId")]
        public LeaveConfiguration? LeaveConfiguration { get; set; }
    }
}
