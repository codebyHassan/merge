using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class HrRecruitmentUnitListItemViewModel
    {
        public decimal Id { get; set; }
        public string? BusinessDepartmentName { get; set; }
        public string? RecruitmentDepartmentName { get; set; }
        public int RecruiterCount { get; set; }
    }

    public class HrRecruitmentUnitFormViewModel
    {
        public decimal Id { get; set; }

        [Required(ErrorMessage = "Business department is required.")]
        [Display(Name = "Business Department")]
        public int? BusinessDepartmentId { get; set; }

        [Required(ErrorMessage = "Recruitment department is required.")]
        [Display(Name = "Recruitment Department")]
        public int? RecruitmentDepartmentId { get; set; }
    }

    public class HrRecruiterInboxItemViewModel
    {
        public decimal RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsAssigned { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
