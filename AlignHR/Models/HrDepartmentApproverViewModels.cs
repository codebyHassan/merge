using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrDepartmentApproverListItemViewModel
    {
        public decimal Id { get; set; }
        public string? DepartmentName { get; set; }
        public string? ApproverType { get; set; }
        public string? EmployeeName { get; set; }
        public bool IsActive { get; set; }
    }

    public class HrDepartmentApproverFormViewModel
    {
        public decimal Id { get; set; }

        [Required]
        [Display(Name = "Department")]
        public decimal? DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Approver Type")]
        public string? ApproverType { get; set; }

        [Required]
        [Display(Name = "Approver")]
        public decimal? EmployeeId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
