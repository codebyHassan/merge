using AlignHR.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    /// <summary>
    /// Implementation of the centralized payroll lock service.
    /// Encapsulates all logic for checking whether payroll records are immutable.
    /// </summary>
    public class PayrollLockService : IPayrollLockService
    {
        private readonly ApplicationDbContext _context;

        public PayrollLockService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<bool> IsRecordLocked(int employeeId, int periodId)
        {
            var period = await _context.SalaryPeriod.FindAsync(periodId);
            if (period != null && period.IsPostedToGL) return true;

            return await _context.SalarySlipMasters
                .AnyAsync(s => s.EmployeeID == employeeId && s.SalaryPeriodID == periodId);
        }

        /// <inheritdoc />
        public async Task<bool> IsPeriodLocked(int periodId)
        {
            var period = await _context.SalaryPeriod.FindAsync(periodId);
            return period != null && period.IsPostedToGL;
        }

        /// <inheritdoc />
        public SelectList GetUnlockedPeriods(string valueField = "SalaryPeriodID", string textField = "PeriodName", object selectedValue = null)
        {
            var periods = _context.SalaryPeriod
                .Where(s => !s.IsPostedToGL)
                .OrderByDescending(s => s.SalaryPeriodID)
                .ToList();

            return new SelectList(periods, valueField, textField, selectedValue);
        }
    }
}
