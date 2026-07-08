/*
using AlignHR.Data;
using AlignHR.Models;
using AlignHR.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Controllers
{
    public class AttendanceController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireUrlPermission]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            var attendanceQuery = _context.Attendance
                .Include(a => a.Employee)
                .Include(a => a.AttendanceDevice)
                .Include(a => a.Location)
                .OrderByDescending(a => a.AttendanceID);

            var attendanceList = attendanceQuery.ToPagedList(page, pageSize);

            return View(attendanceList);
        }

    }
}
*/

