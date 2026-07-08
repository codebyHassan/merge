using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AlignHR.Models
{
    public class TaxSlabMaster
    {
        // ID 
        [Key]
        public int id { get; set; }

        // ---------------- Fiscal Year Linking ----------------

        public int FiscalFK { get; set; }
        [ForeignKey("FiscalFK")]
        [ValidateNever]
        public FiscalYear? FiscalYear { get; set; }


        // ---------------- TAX SLAB ----------------

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal IncomeFrom { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? IncomeTo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FixedTax { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal RatePercent { get; set; }

        public bool IsActive { get; set; } = true;

        // ---------------- Stamps ----------------

        [ValidateNever]
        public int createdby { get; set; }
        [ValidateNever]
        public DateTime createat { get; set; } = DateTime.Now;

        [ValidateNever]
        public int updatedby { get; set; }
        [ValidateNever]
        public DateTime updateat { get; set; }
    }
}