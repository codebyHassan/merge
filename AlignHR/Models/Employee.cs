using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AlignHR.Models
{
    
    public class Employee :  IValidatableObject
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImage { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";


        // FK with ValueSet 
        public int? DesiginationId { get; set; }

        [ForeignKey("DesiginationId")]
        public Valueset? Designation { get; set; }

        public DateOnly Dateofjoin { get; set; }

        // audit Trails 
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }

        //value sets 
        public int? EmploymentTypeFk { get; set; }
        [ForeignKey("EmploymentTypeFk")]
        public Valueset? EmploymentType { get; set; }

        public int? EmploymentStatusFk { get; set; }
        [ForeignKey("EmploymentStatusFk")]
        public Valueset? EmploymentStatus { get; set; }




        // ForignKey For Departments 

        public int DepartmentFk { get; set; }
        [ForeignKey("DepartmentFk")]
        public Department? Department { get; set; }


        //ForignKey For Location 

        public int LocationFk { get; set; }
        [ForeignKey("LocationFk")]
        public Location? Location { get; set; }

        // Banking Information 
   

        public string? BankHolderName { get; set; }

        public int? BankNameFk { get; set; }
        [ForeignKey("BankNameFk")]
        public Valueset? BankName { get; set; }
        
        public string? AccountNumber { get; set; }




        //TAX STFF 

        [StringLength(50)]
        [Display(Name = "NTN (National Tax Number)")]
        public string NTN { get; set; }

        [Required]
        [Display(Name = "Filer Status")]
        public bool IsFiler { get; set; }

        [Display(Name = "Tax Exempted")]
        public bool TaxExempted { get; set; } = false;

        [Display(Name = "Zakat Deduction")]
        public bool ZakatDeduction { get; set; } = false;

        // Salery Status 

        public bool IsServiceEnded { get; set; } = false;
        
        [Display(Name = "Service End Date")]
        public DateTime? ServiceEndDate { get; set; }
        
        public bool IsSalaryStopped { get; set; } = false;
        public string SalaryStatus { get; set; } = "Active";


        // EOBI 

        [Display(Name = "(Employees' Old-Age Benefits Institution) - EOBI Number")]
        public int? EobiNumber { get; set; }
        public bool isEobi { get; set; }


        //Social Secuirty 

        [Display(Name = "(Employee Social Secuirty)")]
        public int? SocailSecuirty { get; set; }
        public bool isSocailSecuirty { get; set; }

        //OverTime 
        public bool isOverTime { get; set; }
        public int? OvertimePolicyId { get; set; }
        [ForeignKey("OvertimePolicyId")]
        public Overtime? OvertimePolicy { get; set; }


        //pf members 
        public bool isPfMember { get; set; }
        public int? pfmembernumber { get; set; }

        [Display(Name = "PF Fixed Amount")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PFAmount { get; set; }

        public bool IsFixedPFAmount { get; set; } = false;

        //Leave Managemnt stuff 

        [StringLength(50)]
        [Display(Name = "Line Manager Code")]
        public string? LineManagerEmpNo { get; set; }

        [StringLength(50)]
        [Display(Name = "Leave Approver Code")]
        public string? LeaveApproverEmpNo { get; set; }

        [Display(Name = "Can Apply For Others")]
        public bool CanApplyForOthers { get; set; } = false;



        public int? SubDepartmentFk { get; set; }
        [ForeignKey("SubDepartmentFk")]
        public Valueset? SubDepartment { get; set; }

        public int? GradeFk { get; set; }
        [ForeignKey("GradeFk")]
        public Valueset? Grade { get; set; }

        public int? DivisionFk { get; set; }
        [ForeignKey("DivisionFk")]
        public Valueset? Division { get; set; }





        //Navigation Property 
        public ICollection<EmployeeShifts> EmployeeShifts { get; set; } = new List<EmployeeShifts>();
        public ICollection<AttendenceLogs> AttendenceLogs { get; set; } = new List<AttendenceLogs>();
        public ICollection<PayRollGenrate> PayRollGenrate { get; set; } = new List<PayRollGenrate>();
        public ICollection<WithoutPayDays> WithoutPayDays { get; set; } = new List<WithoutPayDays>();
        public ICollection<IncomeTaxDetuction> IncomeTaxDetuction { get; set; } = new List<IncomeTaxDetuction>();
        public ICollection<MasterLoanAdvance> MasterLoanAdvance { get; set; } = new List<MasterLoanAdvance>();
        public ICollection<OverTimeHours> OverTimeHours { get; set; } = new List<OverTimeHours>(); 
        public ICollection<PFMembers> PFMembers { get; set; }= new List<PFMembers>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<LeaveApproval> LeaveApprovals { get; set; } = new List<LeaveApproval>();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Code) && 
                !string.IsNullOrWhiteSpace(LeaveApproverEmpNo) && 
                Code.Equals(LeaveApproverEmpNo, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    "An employee cannot be their own Final Leave Approver.",
                    new[] { nameof(LeaveApproverEmpNo) }
                );
            }
            
            if (!string.IsNullOrWhiteSpace(Code) && 
                !string.IsNullOrWhiteSpace(LineManagerEmpNo) && 
                Code.Equals(LineManagerEmpNo, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    "An employee cannot be their own Line Manager.",
                    new[] { nameof(LineManagerEmpNo) }
                );
            }
        }








    }
}
