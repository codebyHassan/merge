using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public enum PayrollComponentType
    {
        Earning,
        Deduction
    }

    [Table("SalarySlipDetails")]
    public class SalarySlipDetail
    {
        [Key]
        public int Id { get; set; }

        // ==========================================
        // 1. RELATIONSHIPS
        // ==========================================
        [Required]
        public int SalarySlipMasterID { get; set; }
        [ForeignKey("SalarySlipMasterID")]
        public virtual SalarySlipMaster SalarySlipMaster { get; set; }

        // ==========================================
        // 2. COMPONENT DATA
        // ==========================================
        [Required]
        public string ComponentName { get; set; } // e.g. "Basic Salary", "House Rent", "Income Tax"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PayrollComponentType Type { get; set; } // Earning or Deduction

        // ==========================================
        // 3. REFERENCES (Audit Trail)
        // ==========================================
        
        // Link back to the rule definition used to calculate this
        public int? PayRollDefinationFK { get; set; }
        [ForeignKey("PayRollDefinationFK")]
        public virtual PayRollDefinationFile PayRollDefinationFile { get; set; }

        //Audit Trails 

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int? updatedby { get; set; }
        public DateTime? updateat { get; set; }

    }
}
