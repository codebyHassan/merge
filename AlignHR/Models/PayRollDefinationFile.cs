using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class PayRollDefinationFile
    {
        [Key]
        public int Id { get; set; }

        public string? DefinitionName { get; set; }

        public int PayRollComFK { get; set; }
        [ForeignKey("PayRollComFK")]
        public Valueset? PayRollCom { get; set; }

        public string? label { get; set; }
        public string? Field { get; set; }
        public string? Percentage { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }


        //Navigate property 
        public ICollection<PayRollGenrate> PayRollGenrate { get; set; } = new List<PayRollGenrate>();
    }
}
