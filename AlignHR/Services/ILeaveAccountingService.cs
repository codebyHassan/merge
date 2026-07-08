using AlignHR.Models;

namespace AlignHR.Services
{
    public interface ILeaveAccountingService
    {
        Task<decimal> GetCurrentBalanceAsync(int employeeFk, decimal leaveTypeFk, DateTime? asOfDate = null);
        Task InsertTransactionAsync(LeaveTransaction txn);
        Task ReverseTransactionAsync(decimal transactionId, string narration, int createdBy);
        Task GenerateYearOpeningAsync(int employeeFk, decimal leaveTypeFk, int year, decimal openingBalance, int createdBy);
        Task LapseLeavesAsync(int employeeFk, decimal leaveTypeFk, int year, int createdBy);
        Task LapseExcessLeavesAsync(int employeeFk, decimal leaveTypeFk, int year, int createdBy, decimal limit);
        Task CarryForwardLeavesAsync(int employeeFk, decimal leaveTypeFk, int fromYear, int toYear, int createdBy, decimal carryForwardLimit, decimal annualQuota, string customNarration = null, string transactionSource = "YearEndProcess");
        Task ProcessCompanyWideLapseAsync(int year, int createdBy, bool lapseAll = false);
        Task ProcessCompanyWideCarryForwardAsync(int fromYear, int toYear, int createdBy);
        Task<List<LeaveTransaction>> GetLedgerAsync(int employeeFk, decimal leaveTypeFk, int year);
        Task RebuildBalancesAsync();
        
        /// <summary>
        /// Validates a leave request against the employee's assigned policy rules.
        /// Returns (isValid, message, isWarning).
        /// </summary>
        Task<(bool isValid, string message, bool isWarning)> ValidateLeavePolicyRulesAsync(int employeeId, decimal leaveTypeId, DateTime fromDate, DateTime toDate, decimal daysCount, bool isHalfDay);

        /// <summary>
        /// Credits leave days directly to an employee's balance as a manual compensation (ManualAdjustment transaction).
        /// Reuses the existing LeaveTransaction ledger — no new tables or columns.
        /// </summary>
        Task CompensateLeavesAsync(int employeeId, decimal leaveTypeId, decimal days, string narration, int year, int createdBy);
    }
}
