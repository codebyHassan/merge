using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlignHR.Services
{
    /// <summary>
    /// Centralized service for enforcing payroll period locks.
    /// When a SalaryPeriod has IsPostedToGL = true, all associated 
    /// payroll records must be immutable (no Create, Edit, or Delete).
    /// </summary>
    public interface IPayrollLockService
    {
        /// <summary>
        /// Checks whether a payroll record is locked for the given employee and period.
        /// Returns true if the period is posted to GL or if a salary slip already exists.
        /// </summary>
        Task<bool> IsRecordLocked(int employeeId, int periodId);

        /// <summary>
        /// Checks whether a salary period is posted to GL (locked) by period ID only.
        /// Useful for BulkUpload/BulkAdd operations that don't have an employee context yet.
        /// </summary>
        Task<bool> IsPeriodLocked(int periodId);

        /// <summary>
        /// Returns a SelectList of salary periods excluding those that are posted to GL.
        /// Orders by SalaryPeriodID descending (newest first).
        /// </summary>
        SelectList GetUnlockedPeriods(string valueField = "SalaryPeriodID", string textField = "PeriodName", object selectedValue = null);
    }
}
