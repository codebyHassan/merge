using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    [Table("Executions")]
    public class Execution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Period { get; set; }

        public bool IsExecuted { get; set; } = false;

        public DateTime? ExecutedAt { get; set; }

        public int? ExecutedBy { get; set; }

        public bool IncomeTaxExecuted { get; set; }
        public bool EOBIExecuted { get; set; }
        public bool LoanDeductionExecuted { get; set; }
        public bool PFExecuted { get; set; }
        public bool BonusFetched { get; set; }
        public bool SalaryAdjustmentFetched { get; set; }
    }
}
