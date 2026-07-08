namespace AlignHR.Models
{
    public class HrWorkflowInboxItemViewModel
    {
        public decimal WorkflowInstanceId { get; set; }
        public decimal RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeName { get; set; }
        public string? StepName { get; set; }
        public string? ApproverType { get; set; }
        public string? Status { get; set; }
        public DateTime StartedDate { get; set; }
    }

    public class HrWorkflowActionViewModel
    {
        public decimal WorkflowInstanceId { get; set; }
        public decimal RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public string? PositionTitle { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeName { get; set; }
        public string? RequisitionType { get; set; }
        public string? Nature { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? PromisedDate { get; set; }
        public decimal? BudgetAmountPerMonth { get; set; }
        public string? Reason { get; set; }
        public string? CurrentStepName { get; set; }
        public string? CurrentApproverType { get; set; }
        public string? CurrentApproverName { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
        public List<HrRequisitionSkillViewModel> Skills { get; set; } = new();
        public List<HrRequisitionOfferingViewModel> Offerings { get; set; } = new();
        public List<HrWorkflowHistoryItemViewModel> History { get; set; } = new();
    }

    public class HrWorkflowHistoryItemViewModel
    {
        public string? StepName { get; set; }
        public string? ApproverType { get; set; }
        public string? ActionByName { get; set; }
        public string? ActionType { get; set; }
        public string? Comments { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
