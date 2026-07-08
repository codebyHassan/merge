using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AlignHR.Models
{
    public class HrOfferFormViewModel
    {
        public decimal Id { get; set; }
        public decimal ApplicationFk { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }
        public string? OfferNumber { get; set; }

        [Required(ErrorMessage = "Proposed salary is required.")]
        [Range(1, 9_999_999_999, ErrorMessage = "Salary must be greater than 0.")]
        [Display(Name = "Proposed Salary")]
        public decimal ProposedSalary { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Proposed Joining Date")]
        public DateTime? ProposedJoiningDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Offer Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }

        public string? Notes { get; set; }
    }

    public class HrOfferListItemViewModel
    {
        public decimal Id { get; set; }
        public string? OfferNumber { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }
        public decimal ProposedSalary { get; set; }
        public DateTime? ProposedJoiningDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public int VersionCount { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class HrOfferDetailsViewModel
    {
        public decimal Id { get; set; }
        public decimal? ApplicationFk { get; set; }
        public string? OfferNumber { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public decimal ProposedSalary { get; set; }
        public DateTime? ProposedJoiningDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? Notes { get; set; }
        public HrHiringDecisionItemViewModel? HiringDecision { get; set; }
        public List<HrOfferVersionItemViewModel> Versions { get; set; } = new();
        public List<HrOfferApprovalItemViewModel> Approvals { get; set; } = new();
        public List<HrOfferResponseItemViewModel> Responses { get; set; } = new();
        public List<HrOfferDocumentItemViewModel> Documents { get; set; } = new();
    }

    public class HrOfferVersionItemViewModel
    {
        public decimal Id { get; set; }
        public int? VersionNo { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class HrOfferVersionFormViewModel
    {
        public decimal OfferId { get; set; }
        public string? OfferNumber { get; set; }
        public string? CandidateName { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Salary must be greater than 0.")]
        [Display(Name = "Revised Salary")]
        public decimal Salary { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Revised Joining Date")]
        public DateTime? JoiningDate { get; set; }

        [StringLength(1000)]
        [Display(Name = "Reason for Revision")]
        public string? Remarks { get; set; }
    }

    public class HrOfferApprovalItemViewModel
    {
        public decimal Id { get; set; }
        public int? ApprovalLevel { get; set; }
        public string? ApproverName { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public DateTime? ActionDate { get; set; }
    }

    public class HrAddApproverViewModel
    {
        public decimal OfferId { get; set; }

        [Required(ErrorMessage = "Please select an approver.")]
        [Display(Name = "Approver")]
        public decimal? ApproverEmployeeId { get; set; }

        [Range(1, 99)]
        [Display(Name = "Approval Level")]
        public int ApprovalLevel { get; set; } = 1;
    }

    public class HrApprovalActionViewModel
    {
        public decimal ApprovalId { get; set; }
        public decimal OfferId { get; set; }
        public string? OfferNumber { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? JobTitle { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? ProposedJoiningDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? ApprovalLevel { get; set; }
        public int TotalLevels { get; set; }
        public string? Notes { get; set; }
        public DateTime? SubmittedOn { get; set; }

        [Required(ErrorMessage = "Please select an action.")]
        public string Action { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Comments { get; set; }
    }

    public class HrOfferResponseFormViewModel
    {
        public decimal OfferId { get; set; }
        public string? OfferNumber { get; set; }
        public string? CandidateName { get; set; }

        [Required(ErrorMessage = "Response type is required.")]
        [Display(Name = "Response")]
        public string ResponseType { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Response Date")]
        public DateTime ResponseDate { get; set; } = DateTime.Today;

        [StringLength(1000)]
        public string? Comments { get; set; }
    }

    public class HrOfferResponseItemViewModel
    {
        public decimal Id { get; set; }
        public string? ResponseType { get; set; }
        public DateTime ResponseDate { get; set; }
        public string? Comments { get; set; }
    }

    public class HrOfferDocumentItemViewModel
    {
        public decimal Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime? GeneratedDate { get; set; }
    }

    public class HrHiringDecisionFormViewModel
    {
        public decimal ApplicationFk { get; set; }
        public string? CandidateName { get; set; }
        public string? JobTitle { get; set; }

        [Required(ErrorMessage = "Decision is required.")]
        public string Decision { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Remarks { get; set; }
    }

    public class HrHiringDecisionItemViewModel
    {
        public decimal Id { get; set; }
        public string? Decision { get; set; }
        public string? DecisionByName { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? Remarks { get; set; }
    }

    public class HrOfferApprovalInboxItemViewModel
    {
        public decimal ApprovalId { get; set; }
        public decimal OfferId { get; set; }
        public string? OfferNumber { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? JobTitle { get; set; }
        public decimal ProposedSalary { get; set; }
        public DateTime? ProposedJoiningDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int ApprovalLevel { get; set; }
        public int TotalLevels { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? Comments { get; set; }
    }
}
