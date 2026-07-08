namespace AlignHR.Models.Enums
{
    /// <summary>
    /// Type-safe workflow stage enum for leave approval routing.
    /// Replaces magic strings with compile-time validated stages.
    /// </summary>
    public enum WorkflowStage
    {
        /// <summary>Request is with the employee's direct line manager.</summary>
        LineManager = 1,

        /// <summary>Request has escalated beyond line manager in the hierarchy.</summary>
        HigherManagement = 2,

        /// <summary>Request is with the designated final approver (HR).</summary>
        HR = 3,

        /// <summary>Request has been fully approved.</summary>
        FinalApproved = 4,

        /// <summary>Request has been rejected at some level.</summary>
        Rejected = 5,

        /// <summary>Request is with a configured external approver (Dept/Grade/Division).</summary>
        ExternalApprover = 6
    }
}
