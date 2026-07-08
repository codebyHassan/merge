using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrJobPostingChannel
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal JobPostingFK { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal PostingChannelFK { get; set; }

        public DateTime? PublishedDate { get; set; }

        [StringLength(500)]
        public string? ExternalReference { get; set; }

        public HrJobPosting? JobPosting { get; set; }
        public HrPostingChannel? PostingChannel { get; set; }
    }
}
