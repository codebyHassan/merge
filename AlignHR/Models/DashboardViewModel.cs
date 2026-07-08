namespace AlignHR.Models
{
    /// <summary>
    /// Aggregated data for the General Dashboard. Universal sections are always
    /// populated; module sections are filled only when the user has access to the
    /// relevant area (see DashboardController). Every field maps to existing tables.
    /// </summary>
    public class DashboardViewModel
    {
        // ── Identity / greeting (universal) ──
        public string GreetingName { get; set; } = "there";
        public string? Role { get; set; }
        public bool HasEmployee { get; set; }
        public EmployeeSnapshot? Me { get; set; }

        // ── Smart digest (composed sentence) ──
        public List<DigestChip> Digest { get; set; } = new();
        public int OnLeaveToday { get; set; }

        // ── Team availability today (managers only, role-derived) ──
        public bool ShowTeam { get; set; }
        public List<TeamMember> Team { get; set; } = new();
        public int TeamIn { get; set; }
        public int TeamLeave { get; set; }
        public int TeamAbsent { get; set; }

        // ── Module: Payroll (latest net + trend) ──
        public bool ShowPayroll { get; set; }
        public decimal LatestNetPay { get; set; }
        public List<decimal> PayTrend { get; set; } = new();

        // ── Module: Attendance heatmap (current month) ──
        public bool ShowHeatmap { get; set; }
        public List<HeatDay> Heatmap { get; set; } = new();
        public int HeatPresent { get; set; }
        public int HeatLeave { get; set; }
        public int HeatAbsent { get; set; }

        // ── Org KPIs (universal) ──
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int NewHiresThisMonth { get; set; }
        public int TotalDepartments { get; set; }
        public int PendingApprovalsCount { get; set; }

        // ── Department distribution (universal) ──
        public List<string> DeptLabels { get; set; } = new();
        public List<int> DeptData { get; set; } = new();

        // ── Unified approvals inbox (smart) ──
        public List<ApprovalItem> Approvals { get; set; } = new();

        // ── Celebrations (smart) ──
        public List<CelebrationItem> Anniversaries { get; set; } = new();
        public List<CelebrationItem> NewJoiners { get; set; } = new();

        // ── Hire-type mix (smart) ──
        public bool HasHireHistory { get; set; }
        public int HireNew { get; set; }
        public int HireReplacement { get; set; }
        public int HireTransfer { get; set; }
        public int HireTotal => HireNew + HireReplacement + HireTransfer;

        // ── Module: Leave ──
        public bool ShowLeave { get; set; }
        public List<LeaveBalanceItem> LeaveBalances { get; set; } = new();
        public decimal LeaveTotalAvailable { get; set; }

        // ── Module: Attendance ──
        public bool ShowAttendance { get; set; }
        public AttendanceToday? AttToday { get; set; }

        // ── Module: Documents ──
        public bool ShowDocuments { get; set; }
        public List<DocExpiryItem> ExpiringDocs { get; set; } = new();

        // ── Quick actions (permission-gated) ──
        public List<QuickAction> QuickActions { get; set; } = new();
    }

    public class EmployeeSnapshot
    {
        public string Code { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? ProfileImage { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public string? ManagerCode { get; set; }
        public string? Location { get; set; }
        public double TenureYears { get; set; }
        public int ProfileCompleteness { get; set; }
        public string Initials { get; set; } = "";
    }

    public class ApprovalItem
    {
        public string Kind { get; set; } = "";        // Leave, Requisition, Offer, Document
        public string Icon { get; set; } = "bi-inbox";
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public int AgeDays { get; set; }
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        // ok (<=1d) / warn (<=3d) / late (>3d)
        public string Severity => AgeDays <= 1 ? "ok" : AgeDays <= 3 ? "warn" : "late";
        public string AgeLabel => AgeDays <= 0 ? "today" : AgeDays == 1 ? "1d" : $"{AgeDays}d";
    }

    public class CelebrationItem
    {
        public string Name { get; set; } = "";
        public string? ProfileImage { get; set; }
        public string Initials { get; set; } = "";
        public string? Department { get; set; }
        public int Years { get; set; }
    }

    public class LeaveBalanceItem
    {
        public string TypeName { get; set; } = "";
        public string? ColorCode { get; set; }
        public decimal Available { get; set; }
        public decimal Allocated { get; set; }
    }

    public class AttendanceToday
    {
        public bool HasPunch { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
        public int Punches { get; set; }
    }

    public class DocExpiryItem
    {
        public string Title { get; set; } = "";
        public string? Type { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysLeft { get; set; }
        // <=15d crit / <=45d warn / else ok
        public string Severity => DaysLeft <= 15 ? "late" : DaysLeft <= 45 ? "warn" : "ok";
    }

    public class TeamMember
    {
        public string Name { get; set; } = "";
        public string Initials { get; set; } = "";
        public string? ProfileImage { get; set; }
        public string Status { get; set; } = "absent"; // in / leave / off / absent
        public string? Detail { get; set; }            // e.g. check-in time
        public string Tone => Status switch { "in" => "good", "leave" => "info", "off" => "muted", _ => "crit" };
        public string Label => Status switch { "in" => "In" + (Detail != null ? " · " + Detail : ""), "leave" => "On leave", "off" => "Off", _ => "Absent" };
    }

    public class DigestChip
    {
        public string Text { get; set; } = "";
        public string Tone { get; set; } = "muted"; // accent / good / warn / crit / muted
    }

    public class HeatDay
    {
        public int Day { get; set; }
        // present / leave / absent / off / future / blank
        public string State { get; set; } = "blank";
    }

    public class QuickAction
    {
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "bi-box-arrow-up-right";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool Primary { get; set; }
    }
}
