using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    [Table("TRN_EML_EMAILS", Schema = "dbo")]
    public class TrnEmail
    {
        [Key]
        [Column("EML_ID", TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal EmlId { get; set; }

        [Column("EML_FROM")]
        public string? EmlFrom { get; set; }

        [Column("EML_TO")]
        public string? EmlTo { get; set; }

        [Column("EML_CC")]
        public string? EmlCc { get; set; }

        [Column("EML_SUBJECT")]
        public string? EmlSubject { get; set; }

        [Column("EML_BODY")]
        public string? EmlBody { get; set; }

        [Column("EML_IS_SENT")]
        public bool? EmlIsSent { get; set; }

        [Column("EML_SENT_ON")]
        public DateTime? EmlSentOn { get; set; }

        [Column("EML_ADD_TAG")]
        [StringLength(50)]
        public string? EmlAddTag { get; set; }

        [Column("EML_ADD_STAMP")]
        public DateTime? EmlAddStamp { get; set; }
    }
}
