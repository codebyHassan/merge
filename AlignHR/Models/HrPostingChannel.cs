using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrPostingChannel
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ChannelName { get; set; } = string.Empty;

        public bool IsInternal { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<HrJobPostingChannel> JobPostingChannels { get; set; } = new List<HrJobPostingChannel>();
    }
}
