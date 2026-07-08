using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AlignHR.Models
{
    public class FiscalYear
    {
        //id 
        [Key]
        public int id { get; set; }

        //title 
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Fiscal Year must be in format YYYY-YY (e.g., 2025-26)")]
        public string title { get; set; }

        //start date 
        [Required]
        [DataType(DataType.Date)]
        public  DateOnly startdate { get; set; }

        //end date 
        [Required]
        [DataType(DataType.Date)]
        public DateOnly enddate { get; set; }


        //stamps
        [ValidateNever]
        public int createdby { get; set; }
        [ValidateNever]
        public DateTime createat { get; set; } = DateTime.Now;
        [ValidateNever]
        public int updatedby { get; set; }
        [ValidateNever]
        public DateTime updateat { get; set; }


        //Navigation property
        [ValidateNever]
        public ICollection<TaxSlabMaster>? TaxSlabMaster { get; set; }
        [ValidateNever]
        public ICollection<TaxSurcharge>? taxSurcharges { get; set; }


    }
}
