using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class DmsDefinationFile
    {
        [Key]
        public int Id { get; set; }

        public string? DefinitionName { get; set; }

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public DocumentCategory? Category { get; set; }

        public int DocumentTypeID { get; set; }
        [ForeignKey("DocumentTypeID")]
        public DocumentType? DocumentType { get; set; }

        public string? label { get; set; }
        public string? Percentage { get; set; }

        public int? ApprovalTemplateId { get; set; }
        [ForeignKey("ApprovalTemplateId")]
        public ApprovalTemplate? ApprovalTemplate { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
