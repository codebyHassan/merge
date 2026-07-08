using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrJobPosting
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal? RequisitionFK { get; set; }

        [StringLength(30)]
        public string? JobCode { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }

        [StringLength(1000)]
        public string? JobDescription { get; set; }

        [StringLength(50)]
        public string? EmploymentType { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Required]
        [StringLength(30)]
        public string PostingStatus { get; set; } = "Draft";

        [Required]
        [Column(TypeName = "date")]
        public DateTime OpenDate { get; set; } = DateTime.Today;

        [Column(TypeName = "date")]
        public DateTime? CloseDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public ICollection<HrJobPostingChannel> PostingChannels { get; set; } = new List<HrJobPostingChannel>();

    }
}
