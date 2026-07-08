using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class Document
    {
        [Key]
        public long DocumentID { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentNo { get; set; } = string.Empty;

        [Required]
        public int EmployeeID { get; set; }

        [ForeignKey("EmployeeID")]
        public Employee? Employee { get; set; }

        [Required]
        public int DocumentTypeID { get; set; }

        [ForeignKey("DocumentTypeID")]
        public DocumentType? DocumentType { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public DocumentCategory? Category { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [StringLength(500)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(20)]
        public string? FileExtension { get; set; }

        public long FileSize { get; set; }

        [StringLength(100)]
        public string? MimeType { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Required]
        public int UploadedBy { get; set; }

        [ForeignKey("UploadedBy")]
        public User? UploadedByUser { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsConfidential { get; set; } = false;

        [StringLength(1000)]
        public string? Remarks { get; set; }
    }
}
