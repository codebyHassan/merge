using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
 

    [Table("SalaryPeriods")]
    public class SalaryPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SalaryPeriodID { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Salary Period must be in format YYYY-MM (e.g., 2025-04)")]
        public string PeriodName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = false;

        public bool IsProcessed { get; set; } = false;

        public bool IsPostedToGL { get; set; } = false;

        [Display(Name = "Amount")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? EOBIAmount { get; set; }

        [Display(Name = "PF Percentage")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PFPercentage { get; set; }




       
    
        // in Employee Fixed Amount 



        //Stamps
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }


        // Navigation Property 
        public ICollection<WithoutPayDays> WithoutPayDays { get; set; } = new List<WithoutPayDays>();
        public ICollection<IncomeTaxDetuction> IncomeTaxDetuction { get; set; } = new List<IncomeTaxDetuction>();
        public ICollection<OverTimeHours> OverTimeHours { get; set; } = new List<OverTimeHours>();
        public ICollection<PFMembers> PFMembers { get; set; } = new List<PFMembers>();

        
    }
}
