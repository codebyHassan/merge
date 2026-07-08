using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace AlignHR.Controllers
{
    public class DailyAttendenceController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DailyAttendenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // ✅ INDEX
        // ===============================
        public async Task<IActionResult> Index(DateOnly? date)
        {
            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Now);

            var result = await GetDailyAttendanceData(targetDate);

            ViewBag.Date = targetDate;

            return View(result);
        }

        // ===============================
        // ✅ COMMON METHOD (REUSABLE)
        // ===============================
        private async Task<List<DailyAttendanceVM>> GetDailyAttendanceData(DateOnly targetDate)
        {
            var logs = await _context.AttendenceLogs
                .Include(x => x.Employee)
                .Include(x => x.Shift)
                .Where(x => x.AttendenceDate == targetDate)
                .ToListAsync();

            var employeeIds = logs.Select(l => l.EmployeeID).Distinct().ToList();

            var employeeShifts = await _context.EmployeeShifts
                .Where(es => employeeIds.Contains(es.EmployeeId)
                            && es.EffiectiveForm <= targetDate
                            && (es.EffiectiveTo == null || es.EffiectiveTo >= targetDate))
                .ToListAsync();

            var shiftIds = employeeShifts.Select(es => es.ShiftId).Distinct().ToList();

            var allShifts = await _context.shifts
                .Where(s => shiftIds.Contains(s.Id))
                .ToListAsync();

            var result = logs
                .GroupBy(x => new { x.EmployeeID, x.AttendenceDate })
                .Select(g =>
                {
                    var inLog = g.OrderBy(x => x.PunchTime).FirstOrDefault();
                    var outLog = g.OrderByDescending(x => x.PunchTime).FirstOrDefault();

                    if (inLog == null || outLog == null)
                        return null;

                    var shift = inLog.Shift;

                    if (shift == null)
                    {
                        var empShift = employeeShifts.FirstOrDefault(es => es.EmployeeId == inLog.EmployeeID);
                        if (empShift != null)
                            shift = allShifts.FirstOrDefault(s => s.Id == empShift.ShiftId);
                    }

                    string shiftName = "-";
                    TimeOnly shiftStartTime = default;
                    TimeOnly shiftEndTime = default;
                    int late = 0, early = 0, extra = 0;

                    if (shift != null && shift.StartTime != null && shift.EndTime != null)
                    {
                        shiftName = shift.ShiftName ?? "-";
                        shiftStartTime = shift.StartTime.Value;
                        shiftEndTime = shift.EndTime.Value;

                        var shiftStart = targetDate.ToDateTime(shift.StartTime.Value);
                        var shiftEnd = targetDate.ToDateTime(shift.EndTime.Value);

                        if (shift.EndTime < shift.StartTime)
                            shiftEnd = shiftEnd.AddDays(1);

                        late = inLog.PunchTime > shiftStart
                            ? (int)(inLog.PunchTime - shiftStart).TotalMinutes : 0;

                        early = inLog.PunchTime < shiftStart
                            ? (int)(shiftStart - inLog.PunchTime).TotalMinutes : 0;
                    }

                    double workingHours = 0;
                    string totalTimeFormatted = "-";
                    string workingHoursFormatted = "-";
                    DateTime? finalOutTime = null;

                    if (inLog.id != outLog.id)
                    {
                        finalOutTime = outLog.PunchTime;
                        var totalTime = outLog.PunchTime - inLog.PunchTime;

                        workingHours = totalTime.TotalHours;
                        totalTimeFormatted = $"{(int)totalTime.TotalHours:D2}:{totalTime.Minutes:D2}:{totalTime.Seconds:D2}";
                        workingHoursFormatted = totalTimeFormatted;

                        if (shift != null && shift.EndTime != null)
                        {
                            var shiftEnd = targetDate.ToDateTime(shift.EndTime.Value);
                            if (shift.EndTime < shift.StartTime)
                                shiftEnd = shiftEnd.AddDays(1);

                            extra = finalOutTime.Value > shiftEnd
                                ? (int)(finalOutTime.Value - shiftEnd).TotalMinutes : 0;
                        }
                    }

                    return new DailyAttendanceVM
                    {
                        EmployeeId = g.Key.EmployeeID,
                        EmployeeCode = inLog.Employee?.Code ?? "N/A",
                        EmployeeName = (inLog.Employee?.FirstName ?? "") + " " + (inLog.Employee?.LastName ?? ""),
                        Date = g.Key.AttendenceDate,
                        InTime = inLog.PunchTime,
                        OutTime = finalOutTime,
                        WorkingHours = workingHours,
                        WorkingHoursFormatted = workingHoursFormatted,
                        TotalTimeFormatted = totalTimeFormatted,
                        LateMinutes = late,
                        EarlyMinutes = early,
                        ExtraMinutes = extra,
                        ShiftName = shiftName,
                        ShiftStart = shiftStartTime,
                        ShiftEnd = shiftEndTime
                    };
                })
                .Where(x => x != null)
                .ToList();

            return result!;
        }

        // ===============================
        // ✅ EXPORT TO EXCEL
        // ===============================
        public async Task<IActionResult> ExportToExcel(DateOnly? date)
        {
            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Now);

            var data = await GetDailyAttendanceData(targetDate);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Daily Attendance");

            // ===== HEADER =====
            string[] headers = {
                "Employee", "Date", "In Time", "Out Time",
                "Working Hours", "Late (min)", "Early (min)", "Extra (min)", "Shift" , "Shift Start Time" , 
                "Shift End Time"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
            }

            using (var range = ws.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // ===== DATA =====
            int row = 2;

            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = item.EmployeeName;
                ws.Cells[row, 2].Value = item.Date.ToString();
                ws.Cells[row, 3].Value = item.InTime?.ToString("HH:mm:ss");
                ws.Cells[row, 4].Value = item.OutTime?.ToString("HH:mm:ss");
                ws.Cells[row, 5].Value = item.WorkingHoursFormatted;
                ws.Cells[row, 6].Value = item.LateMinutes;
                ws.Cells[row, 7].Value = item.EarlyMinutes;
                ws.Cells[row, 8].Value = item.ExtraMinutes;
                ws.Cells[row, 9].Value = item.ShiftName;
                ws.Cells[row, 10].Value = item.ShiftStart.ToString("HH:mm:ss");
                ws.Cells[row, 11].Value = item.ShiftEnd.ToString("HH:mm:ss");


                row++;
            }

            ws.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"DailyAttendance_{targetDate}.xlsx";

            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
