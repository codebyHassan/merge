namespace AlignHR.Models
{
    public class DailyAttendanceVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public DateOnly Date { get; set; }

        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }

        public double WorkingHours { get; set; }

        public int LateMinutes { get; set; }
        public int EarlyMinutes { get; set; }
        public int ExtraMinutes { get; set; }

        public string ShiftName { get; set; }
        public TimeOnly ShiftStart { get; set; }
        public TimeOnly ShiftEnd { get; set; }

        public string WorkingHoursFormatted { get; set; }
        public string TotalTimeFormatted { get; set; }
    }
}
