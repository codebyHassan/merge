using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class DocumentCategory
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

        public ICollection<DocumentType> DocumentTypes { get; set; } = new List<DocumentType>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
