using System.Data;
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public class LeaveAccountingService : ILeaveAccountingService
    {
        private readonly ApplicationDbContext _context;

        public LeaveAccountingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetCurrentBalanceAsync(int employeeFk, decimal leaveTypeFk, DateTime? asOfDate = null)
        {
            var query = _context.LeaveTransactions
                .Where(t => t.EmployeeFk == employeeFk && t.LeaveTypeFk == leaveTypeFk);

            if (asOfDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= asOfDate.Value);
            }

            var sumCredit = await query.SumAsync(t => t.Credit);
            var sumDebit = await query.SumAsync(t => t.Debit);

            return sumDebit - sumCredit;
        }

        public async Task InsertTransactionAsync(LeaveTransaction txn)
        {
            if (_context.Database.CurrentTransaction != null)
            {
                await _InsertTransactionInternalAsync(txn);
            }
            else
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                try
                {
                    await _InsertTransactionInternalAsync(txn);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task _InsertTransactionInternalAsync(LeaveTransaction txn)
        {
            var sumCredit = await _context.LeaveTransactions
                .Where(t => t.EmployeeFk == txn.EmployeeFk && t.LeaveTypeFk == txn.LeaveTypeFk && t.TransactionDate <= txn.TransactionDate)
                .SumAsync(t => t.Credit);

            var sumDebit = await _context.LeaveTransactions
                .Where(t => t.EmployeeFk == txn.EmployeeFk && t.LeaveTypeFk == txn.LeaveTypeFk && t.TransactionDate <= txn.TransactionDate)
                .SumAsync(t => t.Debit);

            txn.Balance = (sumDebit + txn.Debit) - (sumCredit + txn.Credit);

            _context.LeaveTransactions.Add(txn);
            await _context.SaveChangesAsync();
        }

        public async Task ReverseTransactionAsync(decimal transactionId, string narration, int createdBy)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var originalTxn = await _context.LeaveTransactions.FindAsync(transactionId);
                
                if (originalTxn == null)
                    throw new InvalidOperationException("Transaction not found.");

                var reversalTxn = new LeaveTransaction
                {
                    EmployeeFk = originalTxn.EmployeeFk,
                    LeaveTypeFk = originalTxn.LeaveTypeFk,
                    LeaveRequestFk = originalTxn.LeaveRequestFk,
                    TransactionDate = DateTime.Now,
                    TransactionType = originalTxn.TransactionType,
                    Year = originalTxn.Year,
                    Credit = originalTxn.Debit, // Usage (Subtraction)
                    Debit = originalTxn.Credit, // Allocation (Addition)
                    Narration = narration,
                    ReferenceNo = $"REV-{originalTxn.Id}",
                    TransactionSource = "SystemReversal",
                    ReversalOfTransactionId = originalTxn.Id,
                    CreatedBy = createdBy
                };

                var sumCredit = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == reversalTxn.EmployeeFk && t.LeaveTypeFk == reversalTxn.LeaveTypeFk && t.TransactionDate <= reversalTxn.TransactionDate)
                    .SumAsync(t => t.Credit);
                    
                var sumDebit = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == reversalTxn.EmployeeFk && t.LeaveTypeFk == reversalTxn.LeaveTypeFk && t.TransactionDate <= reversalTxn.TransactionDate)
                    .SumAsync(t => t.Debit);

                reversalTxn.Balance = (sumDebit + reversalTxn.Debit) - (sumCredit + reversalTxn.Credit);

                _context.LeaveTransactions.Add(reversalTxn);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task GenerateYearOpeningAsync(int employeeFk, decimal leaveTypeFk, int year, decimal openingBalance, int createdBy)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var existingOpening = await _context.LeaveOpenings
                    .FirstOrDefaultAsync(o => o.EmployeeFk == employeeFk && o.LeaveTypeFk == leaveTypeFk && o.Year == year);

                if (existingOpening != null)
                    throw new InvalidOperationException("Opening balance for this year already exists.");

                var opening = new LeaveOpening
                {
                    EmployeeFk = employeeFk,
                    LeaveTypeFk = leaveTypeFk,
                    Year = year,
                    OpeningBalance = openingBalance,
                    Narration = $"Opening balance for {year}",
                    CreatedBy = createdBy
                };

                _context.LeaveOpenings.Add(opening);
                await _context.SaveChangesAsync();

                var txn = new LeaveTransaction
                {
                    EmployeeFk = employeeFk,
                    LeaveTypeFk = leaveTypeFk,
                    TransactionDate = new DateTime(year, 1, 1),
                    TransactionType = LeaveTransactionType.Opening,
                    Year = year,
                    Debit = openingBalance, // Allocation
                    Credit = 0,
                    Narration = $"Opening balance for {year}",
                    TransactionSource = "YearEndProcess",
                    CreatedBy = createdBy
                };

                var sumCredit = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == txn.EmployeeFk && t.LeaveTypeFk == txn.LeaveTypeFk && t.TransactionDate <= txn.TransactionDate)
                    .SumAsync(t => t.Credit);
                    
                var sumDebit = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == txn.EmployeeFk && t.LeaveTypeFk == txn.LeaveTypeFk && t.TransactionDate <= txn.TransactionDate)
                    .SumAsync(t => t.Debit);

                txn.Balance = (sumDebit + txn.Debit) - (sumCredit + txn.Credit);

                _context.LeaveTransactions.Add(txn);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task LapseLeavesAsync(int employeeFk, decimal leaveTypeFk, int year, int createdBy)
        {
            var currentBalance = await GetCurrentBalanceAsync(employeeFk, leaveTypeFk);

            if (currentBalance <= 0) return;

            var txn = new LeaveTransaction
            {
                EmployeeFk = employeeFk,
                LeaveTypeFk = leaveTypeFk,
                TransactionDate = new DateTime(year, 12, 31),
                TransactionType = LeaveTransactionType.Lapse,
                Year = year,
                Debit = 0,
                Credit = currentBalance, // Reduction
                Narration = $"Year-end leave lapse for {year}",
                TransactionSource = "YearEndProcess",
                CreatedBy = createdBy
            };

            await InsertTransactionAsync(txn);
        }

        public async Task LapseExcessLeavesAsync(int employeeFk, decimal leaveTypeFk, int year, int createdBy, decimal limit)
        {
            // IMPORTANT: Only look at balance up to the end of the year being lapsed
            var asOfDate = new DateTime(year, 12, 31, 23, 59, 59);
            var currentBalance = await GetCurrentBalanceAsync(employeeFk, leaveTypeFk, asOfDate);

            if (currentBalance <= limit) return; // Nothing to lapse

            decimal lapseAmount = currentBalance - limit;

            var txn = new LeaveTransaction
            {
                EmployeeFk = employeeFk,
                LeaveTypeFk = leaveTypeFk,
                TransactionDate = new DateTime(year, 12, 31),
                TransactionType = LeaveTransactionType.Lapse,
                Year = year,
                Debit = 0,
                Credit = lapseAmount, // Reduction
                Narration = $"Year-end leave lapse for excess over CF limit in {year}",
                TransactionSource = "YearEndProcess",
                CreatedBy = createdBy
            };

            await InsertTransactionAsync(txn);
        }

        public async Task CarryForwardLeavesAsync(int employeeFk, decimal leaveTypeFk, int fromYear, int toYear, int createdBy, decimal carryForwardLimit, decimal annualQuota, string customNarration = null, string transactionSource = "YearEndProcess")
        {
            await CleanupOldYearForTransferAsync(employeeFk, leaveTypeFk, fromYear, toYear, createdBy, carryForwardLimit);
            await CreateNewYearOpeningForTransferAsync(employeeFk, leaveTypeFk, fromYear, toYear, createdBy, carryForwardLimit, annualQuota, customNarration, transactionSource);
        }

        private async Task CleanupOldYearForTransferAsync(int employeeFk, decimal leaveTypeFk, int fromYear, int toYear, int createdBy, decimal carryForwardLimit)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Cleanup existing year-end entries for the OLD year only
                var existingOldYearEntries = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == employeeFk && t.LeaveTypeFk == leaveTypeFk && 
                                t.Year == fromYear && 
                                (t.TransactionType == LeaveTransactionType.Lapse || t.TransactionType == LeaveTransactionType.CarryForward) &&
                                t.TransactionSource == "YearEndProcess")
                    .ToListAsync();

                if (existingOldYearEntries.Any())
                {
                    _context.LeaveTransactions.RemoveRange(existingOldYearEntries);
                    await _context.SaveChangesAsync();
                }

                var asOfDate = new DateTime(fromYear, 12, 31, 23, 59, 59);
                var currentBalance = await GetCurrentBalanceAsync(employeeFk, leaveTypeFk, asOfDate);

                decimal carryForwardAmount = Math.Min(Math.Max(0, currentBalance), carryForwardLimit);
                decimal lapseAmount = Math.Max(0, currentBalance - carryForwardAmount);

                if (lapseAmount > 0)
                {
                    await InsertTransactionAsync(new LeaveTransaction
                    {
                        EmployeeFk = employeeFk,
                        LeaveTypeFk = leaveTypeFk,
                        TransactionDate = new DateTime(fromYear, 12, 31, 23, 59, 59),
                        TransactionType = LeaveTransactionType.Lapse,
                        Year = fromYear,
                        Debit = 0,
                        Credit = lapseAmount,
                        Narration = $"Year-end leave lapse (Balance {currentBalance}, Limit {carryForwardLimit})",
                        TransactionSource = "YearEndProcess",
                        CreatedBy = createdBy
                    });
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task CreateNewYearOpeningForTransferAsync(int employeeFk, decimal leaveTypeFk, int fromYear, int toYear, int createdBy, decimal carryForwardLimit, decimal annualQuota, string customNarration = null, string transactionSource = "YearEndProcess")
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Cleanup any existing opening entry for the NEW year (any source)
                var knownSources = new[] { "YearEndProcess", "CompanyWideTransfer", "IndividualTransfer" };
                var existingNewYearEntry = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == employeeFk && t.LeaveTypeFk == leaveTypeFk &&
                                t.Year == toYear && t.TransactionType == LeaveTransactionType.Opening &&
                                knownSources.Contains(t.TransactionSource))
                    .ToListAsync();

                if (existingNewYearEntry.Any())
                {
                    _context.LeaveTransactions.RemoveRange(existingNewYearEntry);
                    await _context.SaveChangesAsync();
                }

                // Determine Carry Forward amount from the OLD year balance as of end of year
                var asOfDate = new DateTime(fromYear, 12, 31, 23, 59, 59);
                
                // Because we no longer zero out the balance in the previous step,
                // the currentBalance is exactly the carry forward amount.
                var currentBalance = await GetCurrentBalanceAsync(employeeFk, leaveTypeFk, asOfDate);
                
                decimal carryForwardAmount = Math.Max(0, currentBalance);
                decimal totalNewBalance = carryForwardAmount + annualQuota;

                if (annualQuota > 0)
                {
                    await InsertTransactionAsync(new LeaveTransaction
                    {
                        EmployeeFk = employeeFk,
                        LeaveTypeFk = leaveTypeFk,
                        TransactionDate = new DateTime(toYear, 1, 1, 0, 0, 1),
                        TransactionType = LeaveTransactionType.Opening,
                        Year = toYear,
                        Debit = annualQuota,
                        Credit = 0,
                        Narration = !string.IsNullOrEmpty(customNarration) ? customNarration : $"Opening Balance (Annual Allocation: {annualQuota})",
                        TransactionSource = transactionSource,
                        CreatedBy = createdBy
                    });
                }

                if (totalNewBalance > 0)
                {
                    var existingOpening = await _context.LeaveOpenings
                        .FirstOrDefaultAsync(o => o.EmployeeFk == employeeFk && o.LeaveTypeFk == leaveTypeFk && o.Year == toYear);

                    if (existingOpening == null)
                    {
                        _context.LeaveOpenings.Add(new LeaveOpening
                        {
                            EmployeeFk = employeeFk,
                            LeaveTypeFk = leaveTypeFk,
                            Year = toYear,
                            OpeningBalance = totalNewBalance,
                            Narration = $"Consolidated Year Opening for {toYear}",
                            CreatedBy = createdBy
                        });
                    }
                    else
                    {
                        existingOpening.OpeningBalance = totalNewBalance;
                        existingOpening.UpdatedAt = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                await RebuildBalancesForSpecificEmployeeAsync(employeeFk, leaveTypeFk);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task RebuildBalancesForSpecificEmployeeAsync(int employeeFk, decimal leaveTypeFk)
        {
            var txns = await _context.LeaveTransactions
                .Where(t => t.EmployeeFk == employeeFk && t.LeaveTypeFk == leaveTypeFk)
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.Id)
                .ToListAsync();

            decimal runningBalance = 0;
            foreach (var txn in txns)
            {
                runningBalance += txn.Debit - txn.Credit;
                txn.Balance = runningBalance;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ProcessCompanyWideLapseAsync(int year, int createdBy, bool lapseAll = false)
        {
            var employeesToLapse = (await _context.EmployeeLeavePolicies
                .Where(ep => ep.IsActive && !ep.Employee.IsServiceEnded)
                .Join(_context.LeavePolicyRules.Where(r => r.IsActive),
                    ep => ep.LeavePolicyId,
                    r => r.LeavePolicyId,
                    (ep, r) => new { ep.EmployeeId, r.LeaveTypeId, r.CarryForwardLimit })
                .ToListAsync())
                .DistinctBy(x => (x.EmployeeId, x.LeaveTypeId))
                .ToList();

            foreach (var elt in employeesToLapse)
            {
                decimal limit = lapseAll ? 0 : elt.CarryForwardLimit;
                await LapseExcessLeavesAsync(elt.EmployeeId, elt.LeaveTypeId, year, createdBy, limit);
            }
        }

        public async Task ProcessCompanyWideCarryForwardAsync(int fromYear, int toYear, int createdBy)
        {
            var employeesToTransfer = (await _context.EmployeeLeavePolicies
                .Where(ep => ep.IsActive && !ep.Employee.IsServiceEnded)
                .Join(_context.LeavePolicyRules.Where(r => r.IsActive),
                    ep => ep.LeavePolicyId,
                    r => r.LeavePolicyId,
                    (ep, r) => new { ep.EmployeeId, r.LeaveTypeId, r.CarryForwardLimit, r.AnnualQuota })
                .ToListAsync())
                .DistinctBy(x => (x.EmployeeId, x.LeaveTypeId))
                .ToList();

            // Pass 1: LAPSE and TRANSFER OUT for all employees first
            foreach (var elt in employeesToTransfer)
            {
                await CleanupOldYearForTransferAsync(elt.EmployeeId, elt.LeaveTypeId, fromYear, toYear, createdBy, elt.CarryForwardLimit);
            }

            // Pass 2: OPENING BALANCE and NEW ALLOCATION for all employees second (full quota, no proration)
            foreach (var elt in employeesToTransfer)
            {
                await CreateNewYearOpeningForTransferAsync(elt.EmployeeId, elt.LeaveTypeId, fromYear, toYear, createdBy, elt.CarryForwardLimit, elt.AnnualQuota, transactionSource: "CompanyWideTransfer");
            }
        }

        public async Task<List<LeaveTransaction>> GetLedgerAsync(int employeeFk, decimal leaveTypeFk, int year)
        {
            return await _context.LeaveTransactions
                .Where(t => t.EmployeeFk == employeeFk && t.LeaveTypeFk == leaveTypeFk && t.Year == year)
                .OrderBy(t => t.TransactionDate)
                .ThenBy(t => t.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task RebuildBalancesAsync()
        {
            var employeesLeaveTypes = await _context.LeaveTransactions
                .Select(t => new { t.EmployeeFk, t.LeaveTypeFk })
                .Distinct()
                .ToListAsync();

            foreach (var elt in employeesLeaveTypes)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                try
                {
                    var txns = await _context.LeaveTransactions
                        .Where(t => t.EmployeeFk == elt.EmployeeFk && t.LeaveTypeFk == elt.LeaveTypeFk)
                        .OrderBy(t => t.TransactionDate)
                        .ThenBy(t => t.Id)
                        .ToListAsync();

                    decimal runningBalance = 0;
                    foreach (var txn in txns)
                    {
                        runningBalance += txn.Debit - txn.Credit;
                        txn.Balance = runningBalance;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task CompensateLeavesAsync(int employeeId, decimal leaveTypeId, decimal days, string narration, int year, int createdBy)
        {
            var txn = new LeaveTransaction
            {
                EmployeeFk        = employeeId,
                LeaveTypeFk       = leaveTypeId,
                TransactionDate   = DateTime.Now,
                TransactionType   = LeaveTransactionType.ManualAdjustment,
                Year              = year,
                Debit             = days,
                Credit            = 0,
                Narration         = narration,
                ReferenceNo       = $"COMP-{employeeId}-{DateTime.Now:yyyyMMddHHmmss}",
                TransactionSource = "LeaveCompensation",
                CreatedBy         = createdBy
            };

            await InsertTransactionAsync(txn);
        }

        public async Task<(bool isValid, string message, bool isWarning)> ValidateLeavePolicyRulesAsync(int employeeId, decimal leaveTypeId, DateTime fromDate, DateTime toDate, decimal daysCount, bool isHalfDay)
        {
            // 1. Get Employee's Policy
            var policy = await _context.EmployeeLeavePolicies
                .Include(p => p.LeavePolicy)
                .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.IsActive);

            if (policy == null)
                return (false, "No active leave policy found for this employee. Please contact HR.", false); 

            // 2. Get Rule for this Leave Type
            var rule = await _context.LeavePolicyRules
                .FirstOrDefaultAsync(r => r.LeavePolicyId == policy.LeavePolicyId && r.LeaveTypeId == leaveTypeId && r.IsActive);

            if (rule == null)
                return (false, "No leave policy rule found for the selected leave type. Please contact HR.", false);

            // 3. Basic Field Checks
            if (isHalfDay && !rule.AllowHalfDay)
                return (false, "Half-day leaves are not allowed for this leave type.", false);

            if (rule.MaxConsecutiveDays > 0 && daysCount > rule.MaxConsecutiveDays)
                return (false, $"This leave request exceeds the maximum consecutive days limit of {rule.MaxConsecutiveDays}.", false);

            if (rule.MinNoticeDays > 0)
            {
                var noticeDays = (fromDate.Date - DateTime.Today).TotalDays;
                if (noticeDays < (double)rule.MinNoticeDays)
                    return (false, $"This leave type requires at least {rule.MinNoticeDays} days of notice.", false);
            }

            // 4. Probation Check
            if (!rule.AllowDuringProbation)
            {
                var employee = await _context.emp.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employeeId);
                if (employee != null && employee.Dateofjoin != default(DateOnly))
                {
                    var joiningDate = employee.Dateofjoin.ToDateTime(TimeOnly.MinValue);
                    // Standard 90-day probation check
                    if ((DateTime.Today - joiningDate).TotalDays < 90)
                    {
                         return (false, "This leave type is not allowed during the probation period (first 90 days).", false);
                    }
                }
            }

            // 5. Monthly Accrual Check (Strict Limit as requested)
            if (rule.MonthlyAccrual > 0)
            {
                int currentMonth = fromDate.Month;
                int currentYear = fromDate.Year;

                // Find opening balance (Carry Forward)
                var openingEntry = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == employeeId && t.LeaveTypeFk == leaveTypeId && t.Year == currentYear && t.TransactionType == LeaveTransactionType.Opening)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                
                decimal carryForward = openingEntry?.Debit ?? 0;
                
                // Max allowed usage up to this month
                decimal accruedToDate = (rule.MonthlyAccrual * currentMonth) + carryForward;
                
                // Total already availed this year (summing all 'Availed' entries)
                var totalAvailed = await _context.LeaveTransactions
                    .Where(t => t.EmployeeFk == employeeId && t.LeaveTypeFk == leaveTypeId && t.Year == currentYear && t.TransactionType == LeaveTransactionType.Availed)
                    .SumAsync(t => t.Credit);

                if (totalAvailed + daysCount > accruedToDate)
                {
                    return (false, $"Monthly Accrual Limit Exceeded. You have accrued {accruedToDate} days up to {fromDate:MMMM}, but this request would bring your total usage to {totalAvailed + daysCount} days.", false);
                }
            }

            // 6. Balance Check
            var currentBalance = await GetCurrentBalanceAsync(employeeId, leaveTypeId);
            var projectedBalance = currentBalance - daysCount;

            // 7. Final Balance Check (Negative Balance Rule)
            if (projectedBalance < 0)
            {
                if (!rule.AllowNegativeBalance)
                {
                    return (false, $"Insufficient leave balance. Available: {currentBalance}, Requested: {daysCount}.", false);
                }
                else
                {
                    // Allow but warn as requested: "Alert him when he apply"
                    return (true, $"Warning: You are exceeding your available balance. This request will result in a negative balance of {projectedBalance}.", true);
                }
            }

            return (true, "", false);
        }
    }
}
