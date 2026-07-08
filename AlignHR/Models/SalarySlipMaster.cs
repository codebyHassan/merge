using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    [Table("SalarySlipMasters")]
    public class SalarySlipMaster
    {
        [Key]
        public int Id { get; set; }

        // ==========================================
        // 1. RELATIONSHIPS & SNAPSHOTS (History)
        // ==========================================
        [Required]
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }

        [Required]
        public int SalaryPeriodID { get; set; }
        [ForeignKey("SalaryPeriodID")]
        public virtual SalaryPeriod SalaryPeriod { get; set; }

        public string EmployeeNameSnapshot { get; set; }
        public string DepartmentSnapshot { get; set; }
        public string DesignationSnapshot { get; set; }
        public string LocationSnapshot { get; set; }
        public string BankNameSnapshot { get; set; }
        public string AccountNumberSnapshot { get; set; }

        // ==========================================
        // 2. ATTENDANCE & WITHOUT PAY (FK Added)
        // ==========================================
        public int TotalDaysInMonth { get; set; } 
        
        // Link to your WithoutPayDays table
        public int? WithoutPayDaysFK { get; set; }
        [ForeignKey("WithoutPayDaysFK")]
        public virtual WithoutPayDays WithoutPayDaysRecord { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnpaidDaysSnapshot { get; set; } // The value frozen from the FK

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetPaidDays { get; set; } // TotalDaysInMonth - UnpaidDaysSnapshot

        // ==========================================
        // 3. FINANCIALS (Gross Salary FK Added)
        // ==========================================
        
        // Link to your PayRollGenrate (Salary Breakups)
        public int? GrossSalaryFK { get; set; }
        [ForeignKey("GrossSalaryFK")]
        public virtual PayRollGenrate SalaryBreakupRecord { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossSalarySnapshot { get; set; } // The fixed salary frozen here

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAllowances { get; set; } 
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDeductions { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; } 

        // ==========================================
        // 4. ARREARS & ROUNDING
        // ==========================================
        [Column(TypeName = "decimal(18,2)")]
        public decimal ArrearAmount { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal BonusAmount { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal PreviousCarryForward { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdjustedNetSalary { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal PayableAmount { get; set; } // Rounded Multiple of 100

        [Column(TypeName = "decimal(18,2)")]
        public decimal NewCarryForward { get; set; } 

        // ==========================================
        // 5. STATUS & AUDIT
        // ==========================================
        public string Status { get; set; } = "Draft";
        public string? Remarks { get; set; }

        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int? updatedby { get; set; }
        public DateTime? updateat { get; set; }

        // ==========================================
        // 6. DETAIL RELATIONSHIP
        // ==========================================
        public virtual ICollection<SalarySlipDetail> Details { get; set; } = new List<SalarySlipDetail>();
    }
}
