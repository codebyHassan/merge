using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlignHR.Models
{
    public enum LoanStatus
    {
        Pending,
        Approved,
        Rejected,
        Closed
    }
    public enum LoanInstallmentStatus
    {
        Pending,
        Deducted,
        Skipped
    }
    public enum TransactionType
    {
        Advance,
        Loan,
        Salary
    }

    public class MasterLoanAdvance
    {

        // id 
        [Key]
        public int id { get; set; }

     

        // Forign Key Employee 
        public int EmployeeID { get; set; }
        [ForeignKey("EmployeeID")]
        public Employee? Employee { get; set; }


        //(Advance PAISA & Loan Paisa )
        public TransactionType TransactionType { get; set; }

        public DateTime RequestDate { get; set; }

        //Amounts 

         [Column(TypeName = "decimal(18,2)")]
         public decimal TotalAmount { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal ApprovedAmount { get; set; }

        public string? Reason { get; set; }
        public LoanStatus Status { get; set; }
        public int TenureMonths { get; set; }


         [Column(TypeName = "decimal(18,2)")]
         public decimal MonthlyInstallment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InterestRate { get; set; }

        // Loan Detuction Boolean field  

        public bool LoanDetuction { get; set; }


 


        //Stamps 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

        // Navigation Property
        public ICollection<MasterLoanAdvanceDetail> Details { get; set; } = new List<MasterLoanAdvanceDetail>();
    }
}
