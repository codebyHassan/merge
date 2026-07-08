using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlignHR.Models;
using AlignHR.Data;
using AlignHR.Security;

namespace AlignHR.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var yearStart = new DateOnly(now.Year, 1, 1);
            var session = PermissionHelper.GetUserSession(HttpContext);
            var myEmpId = session?.EmployeeId;

            var vm = new DashboardViewModel
            {
                GreetingName = FirstName(session?.FullName) ?? "there",
                Role = session?.Roles?.FirstOrDefault(),
                HasEmployee = myEmpId.HasValue
            };

            // ── Org KPIs (universal) ──
            try
            {
                vm.TotalEmployees = _context.emp.Count();
                vm.ActiveEmployees = _context.emp.Count(e => e.EmploymentStatus != null && e.EmploymentStatus.Name == "Active");
                vm.NewHiresThisMonth = _context.emp.Count(e => e.createat.Month == now.Month && e.createat.Year == now.Year);
                vm.TotalDepartments = _context.department.Count();
            }
            catch { /* leave zeros */ }

            // ── Department distribution (universal) ──
            try
            {
                var deptStats = _context.department
                    .Select(d => new { d.Name, Count = d.Employees.Count() })
                    .Where(d => d.Count > 0)
                    .OrderByDescending(d => d.Count)
                    .Take(8)
                    .ToList();
                vm.DeptLabels = deptStats.Select(d => d.Name).ToList();
                vm.DeptData = deptStats.Select(d => d.Count).ToList();
            }
            catch { }

            // ── Hire-type mix (smart) ──
            try
            {
                var hires = _context.EmployeeHireHistories
                    .Where(h => h.EffectiveDate >= yearStart)
                    .Select(h => h.HireType)
                    .ToList();
                if (hires.Count > 0)
                {
                    vm.HasHireHistory = true;
                    vm.HireNew = hires.Count(h => h == "New");
                    vm.HireReplacement = hires.Count(h => h == "Replacement");
                    vm.HireTransfer = hires.Count(h => h == "Transfer");
                }
            }
            catch { }

            // ── Celebrations (smart) ──
            try
            {
                var joiners = _context.emp
                    .Where(e => e.Dateofjoin.Month == now.Month && e.Dateofjoin.Year > 1900)
                    .Select(e => new { e.FirstName, e.LastName, e.ProfileImage, e.Dateofjoin, Dept = e.Department != null ? e.Department.Name : null })
                    .Take(24)
                    .ToList();

                foreach (var j in joiners)
                {
                    var years = now.Year - j.Dateofjoin.Year;
                    var item = new CelebrationItem
                    {
                        Name = $"{j.FirstName} {j.LastName}".Trim(),
                        ProfileImage = j.ProfileImage,
                        Department = j.Dept,
                        Years = years,
                        Initials = Initials(j.FirstName, j.LastName)
                    };
                    if (years <= 0) vm.NewJoiners.Add(item);
                    else vm.Anniversaries.Add(item);
                }
                vm.Anniversaries = vm.Anniversaries.OrderByDescending(a => a.Years).Take(5).ToList();
                vm.NewJoiners = vm.NewJoiners.Take(5).ToList();
            }
            catch { }

            // ── Unified approvals inbox (smart) ──
            try
            {
                var approvals = new List<ApprovalItem>();

                if (myEmpId.HasValue)
                {
                    var leaveInbox = _context.LeaveRequests
                        .Include(r => r.Employee)
                        .Include(r => r.LeaveType)
                        .Where(r => r.CurrentApproverId == myEmpId && r.ApprovedAt == null && r.RejectedAt == null)
                        .OrderBy(r => r.AppliedAt)
                        .Take(10)
                        .ToList();

                    foreach (var r in leaveInbox)
                    {
                        var applied = r.AppliedAt ?? r.CreatedAt ?? now;
                        approvals.Add(new ApprovalItem
                        {
                            Kind = "Leave",
                            Icon = "bi-airplane",
                            Title = r.Employee != null ? $"{r.Employee.FirstName} {r.Employee.LastName}".Trim() : "Leave request",
                            Subtitle = $"{r.LeaveType?.Name ?? "Leave"} · {r.DaysCount:0.#} day(s)",
                            AgeDays = (int)(now - applied).TotalDays,
                            Controller = "LeaveRequests",
                            Action = "Index"
                        });
                    }

                    var myEmpDec = (decimal)myEmpId.Value;
                    var wfSteps = _context.HrWorkflowInstanceSteps
                        .Include(s => s.WorkflowInstance)
                        .Where(s => s.EmployeeId == myEmpDec && s.Status == "Pending")
                        .OrderBy(s => s.AssignedDate)
                        .Take(10)
                        .ToList();

                    foreach (var s in wfSteps)
                    {
                        var entity = s.WorkflowInstance?.EntityType ?? "Request";
                        var isOffer = entity.Contains("Offer", StringComparison.OrdinalIgnoreCase);
                        approvals.Add(new ApprovalItem
                        {
                            Kind = entity,
                            Icon = isOffer ? "bi-file-earmark-text" : "bi-clipboard-check",
                            Title = $"{entity} approval",
                            Subtitle = "Awaiting your action",
                            AgeDays = (int)(now - s.AssignedDate).TotalDays,
                            Controller = isOffer ? "HrOffers" : "HrRequisitions",
                            Action = isOffer ? "MyApprovals" : "Inbox"
                        });
                    }
                }

                vm.PendingApprovalsCount = approvals.Count;
                vm.Approvals = approvals.OrderByDescending(a => a.AgeDays).Take(6).ToList();
            }
            catch { }

            // ── My snapshot (universal, needs linked employee) ──
            if (myEmpId.HasValue)
            {
                try
                {
                    var me = _context.emp
                        .Include(e => e.Department)
                        .Include(e => e.Designation)
                        .Include(e => e.Location)
                        .FirstOrDefault(e => e.Id == myEmpId.Value);

                    if (me != null)
                    {
                        var join = me.Dateofjoin.Year > 1900 ? me.Dateofjoin.ToDateTime(TimeOnly.MinValue) : (DateTime?)null;
                        vm.Me = new EmployeeSnapshot
                        {
                            Code = me.Code ?? "",
                            FullName = $"{me.FirstName} {me.LastName}".Trim(),
                            ProfileImage = me.ProfileImage,
                            Department = me.Department?.Name,
                            Designation = me.Designation?.Name,
                            ManagerCode = me.LineManagerEmpNo,
                            Location = me.Location?.Name,
                            TenureYears = join.HasValue ? Math.Round((now - join.Value).TotalDays / 365.25, 1) : 0,
                            ProfileCompleteness = ProfileScore(me),
                            Initials = Initials(me.FirstName, me.LastName)
                        };
                    }
                }
                catch { }
            }

            // ── Module: Leave balance ──
            if (myEmpId.HasValue && HasAccess(session, "Leaves", "LeaveRequests", "LeaveBalances"))
            {
                try
                {
                    var balances = _context.LeaveBalances
                        .Include(b => b.LeaveType)
                        .Where(b => b.EmployeeId == myEmpId.Value && b.Year == now.Year)
                        .ToList();

                    if (balances.Count > 0)
                    {
                        vm.ShowLeave = true;
                        vm.LeaveBalances = balances.Select(b => new LeaveBalanceItem
                        {
                            TypeName = b.LeaveType?.Name ?? "Leave",
                            ColorCode = b.LeaveType?.ColorCode,
                            Available = b.Available,
                            Allocated = b.Allocated
                        }).ToList();
                        vm.LeaveTotalAvailable = balances.Sum(b => b.Available);
                    }
                }
                catch { }
            }

            // ── Module: Attendance today ──
            if (myEmpId.HasValue && HasAccess(session, "Attendance", "AttendenceLogs", "DailyAttendence"))
            {
                try
                {
                    var logs = _context.AttendenceLogs
                        .Where(a => a.EmployeeID == myEmpId.Value && a.AttendenceDate == today)
                        .OrderBy(a => a.PunchTime)
                        .ToList();

                    vm.ShowAttendance = true;
                    if (logs.Count > 0)
                    {
                        vm.AttToday = new AttendanceToday
                        {
                            HasPunch = true,
                            CheckIn = logs.First().PunchTime.ToString("hh:mm tt"),
                            CheckOut = logs.Count > 1 ? logs.Last().PunchTime.ToString("hh:mm tt") : null,
                            Punches = logs.Count
                        };
                    }
                    else
                    {
                        vm.AttToday = new AttendanceToday { HasPunch = false };
                    }
                }
                catch { }
            }

            // ── Module: Document expiry radar ──
            if (myEmpId.HasValue && HasAccess(session, "Documents"))
            {
                try
                {
                    var horizon = now.AddDays(90);
                    var docs = _context.Documents
                        .Include(d => d.DocumentType)
                        .Where(d => d.EmployeeID == myEmpId.Value
                                 && d.ExpiryDate != null
                                 && d.ExpiryDate >= now
                                 && d.ExpiryDate <= horizon)
                        .OrderBy(d => d.ExpiryDate)
                        .Take(5)
                        .ToList();

                    if (docs.Count > 0)
                    {
                        vm.ShowDocuments = true;
                        vm.ExpiringDocs = docs.Select(d => new DocExpiryItem
                        {
                            Title = d.Title,
                            Type = d.DocumentType?.TypeName,
                            ExpiryDate = d.ExpiryDate!.Value,
                            DaysLeft = (int)(d.ExpiryDate!.Value - now).TotalDays
                        }).ToList();
                    }
                }
                catch { }
            }

            // ── Module: Payroll (latest net + last 6 trend) ──
            if (myEmpId.HasValue && HasAccess(session, "SalarySlips", "Payroll", "PayRollGenrate"))
            {
                try
                {
                    var slips = _context.SalarySlipMasters
                        .Where(s => s.EmployeeID == myEmpId.Value)
                        .OrderByDescending(s => s.SalaryPeriodID)
                        .Take(6)
                        .Select(s => s.NetSalary)
                        .ToList();

                    if (slips.Count > 0)
                    {
                        vm.ShowPayroll = true;
                        vm.LatestNetPay = slips.First();
                        slips.Reverse();
                        vm.PayTrend = slips;
                    }
                }
                catch { }
            }

            // ── On leave today (for digest / availability) ──
            try
            {
                vm.OnLeaveToday = _context.LeaveRequests
                    .Count(r => r.ApprovedAt != null && r.RejectedAt == null
                             && r.FromDate <= now && r.ToDate >= now.Date);
            }
            catch { }

            // ── Module: Attendance heatmap (current month) ──
            if (myEmpId.HasValue && HasAccess(session, "Attendance", "AttendenceLogs", "DailyAttendence"))
            {
                try
                {
                    var monthStart = new DateOnly(now.Year, now.Month, 1);
                    var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    var monthEnd = new DateOnly(now.Year, now.Month, daysInMonth);

                    var punchDays = _context.AttendenceLogs
                        .Where(a => a.EmployeeID == myEmpId.Value
                                 && a.AttendenceDate >= monthStart && a.AttendenceDate <= monthEnd)
                        .Select(a => a.AttendenceDate)
                        .Distinct()
                        .ToList()
                        .Select(d => d.Day)
                        .ToHashSet();

                    // approved leave overlapping this month → set of day numbers
                    var monthStartDt = new DateTime(now.Year, now.Month, 1);
                    var monthEndDt = monthStartDt.AddMonths(1);
                    var leaveRanges = _context.LeaveRequests
                        .Where(r => r.EmployeeId == myEmpId.Value && r.ApprovedAt != null && r.RejectedAt == null
                                 && r.FromDate < monthEndDt && r.ToDate >= monthStartDt)
                        .Select(r => new { r.FromDate, r.ToDate })
                        .ToList();

                    var leaveDays = new HashSet<int>();
                    foreach (var lr in leaveRanges)
                        for (var d = lr.FromDate.Date; d <= lr.ToDate.Date && d < monthEndDt; d = d.AddDays(1))
                            if (d >= monthStartDt) leaveDays.Add(d.Day);

                    vm.ShowHeatmap = true;
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var date = new DateTime(now.Year, now.Month, day);
                        string state;
                        if (date.Date > now.Date) state = "future";
                        else if (punchDays.Contains(day)) { state = "present"; vm.HeatPresent++; }
                        else if (leaveDays.Contains(day)) { state = "leave"; vm.HeatLeave++; }
                        else if (date.DayOfWeek == DayOfWeek.Sunday) state = "off";
                        else { state = "absent"; vm.HeatAbsent++; }
                        vm.Heatmap.Add(new HeatDay { Day = day, State = state });
                    }
                }
                catch { }
            }

            // ── Team availability today (managers only) ──
            if (myEmpId.HasValue)
            {
                try
                {
                    var myCode = vm.Me?.Code;
                    if (string.IsNullOrWhiteSpace(myCode))
                        myCode = _context.emp.Where(e => e.Id == myEmpId.Value).Select(e => e.Code).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(myCode))
                    {
                        var reports = _context.emp
                            .Where(e => e.LineManagerEmpNo == myCode && e.Id != myEmpId.Value)
                            .Select(e => new { e.Id, e.FirstName, e.LastName, e.ProfileImage })
                            .Take(30)
                            .ToList();

                        if (reports.Count > 0)
                        {
                            var ids = reports.Select(r => r.Id).ToList();

                            var punchedToday = _context.AttendenceLogs
                                .Where(a => ids.Contains(a.EmployeeID) && a.AttendenceDate == today)
                                .OrderBy(a => a.PunchTime)
                                .Select(a => new { a.EmployeeID, a.PunchTime })
                                .ToList()
                                .GroupBy(a => a.EmployeeID)
                                .ToDictionary(g => g.Key, g => g.First().PunchTime);

                            var onLeave = _context.LeaveRequests
                                .Where(r => ids.Contains(r.EmployeeId) && r.ApprovedAt != null && r.RejectedAt == null
                                         && r.FromDate <= now && r.ToDate >= now.Date)
                                .Select(r => r.EmployeeId)
                                .Distinct()
                                .ToList()
                                .ToHashSet();

                            var isSunday = now.DayOfWeek == DayOfWeek.Sunday;

                            foreach (var r in reports)
                            {
                                var tm = new TeamMember
                                {
                                    Name = $"{r.FirstName} {r.LastName}".Trim(),
                                    Initials = Initials(r.FirstName, r.LastName),
                                    ProfileImage = r.ProfileImage
                                };

                                if (onLeave.Contains(r.Id)) { tm.Status = "leave"; vm.TeamLeave++; }
                                else if (punchedToday.TryGetValue(r.Id, out var pt)) { tm.Status = "in"; tm.Detail = pt.ToString("hh:mm tt"); vm.TeamIn++; }
                                else if (isSunday) { tm.Status = "off"; }
                                else { tm.Status = "absent"; vm.TeamAbsent++; }

                                vm.Team.Add(tm);
                            }

                            // present first, then leave, off, absent last
                            vm.Team = vm.Team
                                .OrderBy(t => t.Status == "in" ? 0 : t.Status == "leave" ? 1 : t.Status == "off" ? 2 : 3)
                                .ThenBy(t => t.Name)
                                .ToList();
                            vm.ShowTeam = true;
                        }
                    }
                }
                catch { }
            }

            // ── Compose smart digest ──
            BuildDigest(vm);

            // ── Quick actions (permission-gated) ──
            vm.QuickActions = BuildQuickActions(session);

            return View(vm);
        }

        // ── helpers ──

        private static void BuildDigest(DashboardViewModel vm)
        {
            if (vm.PendingApprovalsCount > 0)
                vm.Digest.Add(new DigestChip { Text = $"{vm.PendingApprovalsCount} approval{(vm.PendingApprovalsCount == 1 ? "" : "s")} waiting", Tone = "warn" });

            var soon = vm.ExpiringDocs.OrderBy(d => d.DaysLeft).FirstOrDefault();
            if (soon != null)
                vm.Digest.Add(new DigestChip { Text = $"a document expires in {soon.DaysLeft} day{(soon.DaysLeft == 1 ? "" : "s")}", Tone = soon.DaysLeft <= 15 ? "crit" : "muted" });

            if (vm.ShowTeam && vm.TeamAbsent > 0)
                vm.Digest.Add(new DigestChip { Text = $"{vm.TeamAbsent} of your team absent", Tone = "crit" });

            if (vm.OnLeaveToday > 0)
                vm.Digest.Add(new DigestChip { Text = $"{vm.OnLeaveToday} on leave today", Tone = "muted" });

            if (vm.ShowPayroll && vm.LatestNetPay > 0)
                vm.Digest.Add(new DigestChip { Text = $"latest net pay {vm.LatestNetPay:N0}", Tone = "good" });

            if (vm.Digest.Count == 0)
                vm.Digest.Add(new DigestChip { Text = "you're all caught up — nothing needs you right now", Tone = "good" });
        }

        private static bool HasAccess(UserSession? session, params string[] controllers)
        {
            if (session == null) return false;
            return controllers.Any(c => session.HasControllerAccess(c));
        }

        private static List<QuickAction> BuildQuickActions(UserSession? session)
        {
            var candidates = new List<QuickAction>
            {
                new() { Label = "Apply Leave", Icon = "bi-calendar-plus", Controller = "LeaveRequests", Action = "Create", Primary = true },
                new() { Label = "My Attendance", Icon = "bi-clock-history", Controller = "AttendenceLogs", Action = "Index" },
                new() { Label = "Payslips", Icon = "bi-cash-stack", Controller = "SalarySlips", Action = "Index" },
                new() { Label = "Documents", Icon = "bi-folder2-open", Controller = "Documents", Action = "Index" },
                new() { Label = "Directory", Icon = "bi-people", Controller = "Employees", Action = "Index" },
                new() { Label = "Recruitment", Icon = "bi-person-badge", Controller = "HrRequisitions", Action = "Index" },
            };

            if (session == null) return new List<QuickAction>();
            return candidates.Where(q => session.HasControllerAccess(q.Controller)).Take(6).ToList();
        }

        private static string? FirstName(string? full)
        {
            if (string.IsNullOrWhiteSpace(full)) return null;
            return full.Trim().Split(' ')[0];
        }

        private static string Initials(string? first, string? last)
        {
            var a = string.IsNullOrWhiteSpace(first) ? "" : first.Trim().Substring(0, 1);
            var b = string.IsNullOrWhiteSpace(last) ? "" : last.Trim().Substring(0, 1);
            var res = (a + b).ToUpper();
            return string.IsNullOrEmpty(res) ? "?" : res;
        }

        private static int ProfileScore(Employee e)
        {
            // Completeness across commonly-populated fields — nudges data quality.
            var checks = new[]
            {
                !string.IsNullOrWhiteSpace(e.Code),
                !string.IsNullOrWhiteSpace(e.FirstName),
                !string.IsNullOrWhiteSpace(e.LastName),
                !string.IsNullOrWhiteSpace(e.ProfileImage),
                e.DepartmentFk > 0,
                e.LocationFk > 0,
                e.DesiginationId.HasValue,
                !string.IsNullOrWhiteSpace(e.NTN),
                !string.IsNullOrWhiteSpace(e.BankHolderName),
                !string.IsNullOrWhiteSpace(e.AccountNumber),
                !string.IsNullOrWhiteSpace(e.LineManagerEmpNo),
                e.Dateofjoin.Year > 1900,
            };
            var filled = checks.Count(c => c);
            return (int)Math.Round(filled * 100.0 / checks.Length);
        }
    }
}
