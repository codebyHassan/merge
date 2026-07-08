using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrCandidate
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? CurrentLocation { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Gender { get; set; }

        [StringLength(100)]
        public string? HighestDegree { get; set; }

        [StringLength(200)]
        public string? FieldOfStudy { get; set; }

        [StringLength(200)]
        public string? University { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalExperienceYears { get; set; }

        public bool? IsCurrentlyEmployed { get; set; }

        [StringLength(200)]
        public string? CurrentEmployer { get; set; }

        [StringLength(200)]
        public string? CurrentDesignation { get; set; }

        [StringLength(50)]
        public string? NoticePeriod { get; set; }

        [StringLength(500)]
        public string? ResumeFileName { get; set; }

        [StringLength(500)]
        public string? LinkedInProfile { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        public ICollection<HrCandidateDocument> Documents { get; set; } = new List<HrCandidateDocument>();
        public ICollection<HrJobApplication> Applications { get; set; } = new List<HrJobApplication>();
    }
}
