using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public DocumentCategory? Category { get; set; }

        [Required]
        [StringLength(200)]
        public string TypeName { get; set; } = string.Empty;

        public bool RequiresApproval { get; set; } = false;

        public bool HasExpiry { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
