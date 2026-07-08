using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrAssignmentFormViewModel
    {
        public decimal Id { get; set; }

        [Display(Name = "Requisition")]
        public decimal RequisitionFk { get; set; }

        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }

        [Required(ErrorMessage = "Please select a recruiter.")]
        [Display(Name = "Recruiter")]
        public decimal? RecruiterEmployeeFK { get; set; }

        [Display(Name = "Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime? AssignedDate { get; set; } = DateTime.Today;

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class HrAssignmentDetailsViewModel
    {
        public decimal Id { get; set; }
        public decimal RequisitionFk { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? RecruiterName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class HrMyAssignmentViewModel
    {
        public decimal AssignmentId { get; set; }
        public decimal RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? RequisitionType { get; set; }
        public string? Nature { get; set; }
        public decimal? BudgetAmountPerMonth { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string? AssignedByName { get; set; }
        public string? Notes { get; set; }
        public string? RequisitionStatus { get; set; }
        public List<SkillItem> Skills { get; set; } = new();
        public List<OfferingItem> Offerings { get; set; } = new();
    }

    public class SkillItem
    {
        public string SkillName { get; set; } = string.Empty;
        public decimal? YearsExperience { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class OfferingItem
    {
        public string OfferingName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
