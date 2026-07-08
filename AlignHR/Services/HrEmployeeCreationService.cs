using AlignHR.Data;
using AlignHR.Models;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Services
{
    public class HrEmployeeCreationService : IEmployeeCreationService
    {
        private readonly ApplicationDbContext _context;

        public HrEmployeeCreationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateEmployeeFromCandidateAsync(
            decimal candidateId,
            decimal onboardingId,
            HrJoiningConfirmationFormViewModel form)
        {
            var candidate = await _context.HrCandidates
                .FirstOrDefaultAsync(c => c.Id == candidateId);

            if (candidate == null)
                throw new InvalidOperationException("Candidate not found.");

            var code = await GenerateEmployeeCodeAsync();

            var employee = new Employee
            {
                Code = code,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName ?? string.Empty,
                DepartmentFk = form.DepartmentFk,
                LocationFk = form.LocationFk,
                Dateofjoin = DateOnly.FromDateTime(form.JoinedDate),
                NTN = form.NTN ?? "N/A",
                IsFiler = form.IsFiler,
                SalaryStatus = "Active",
                createdby = 1,
                createat = DateTime.Now,
                updatedby = 1,
                updateat = DateTime.Now
            };

            await _context.emp.AddAsync(employee);
            await _context.SaveChangesAsync();

            // Create User account linked to the new employee
            var user = new User
            {
                FirstName = candidate.FirstName,
                LastName = candidate.LastName ?? string.Empty,
                Email = candidate.Email,
                Password = BCryptHashPassword(code), // default password = employee code
                UsrIsActive = "Y",
                EmployeeId = employee.Id,
                createdby = 1,
                createat = DateTime.Now,
                updatedby = 1,
                updateat = DateTime.Now
            };

            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync(); // flush to get user.Id

            // Assign default Employee role if it exists
            var defaultRole = await _context.roles
                .FirstOrDefaultAsync(r => r.Name == "Employee" || r.Name == "Staff");
            if (defaultRole != null)
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                });
                await _context.SaveChangesAsync();
            }

            return employee.Id;
        }

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            var maxId = await _context.emp.MaxAsync(e => (int?)e.Id) ?? 0;
            var seq = maxId + 1;
            var code = $"EMP{seq:D5}";
            while (await _context.emp.AnyAsync(e => e.Code == code))
            {
                seq++;
                code = $"EMP{seq:D5}";
            }
            return code;
        }

        private static string BCryptHashPassword(string plain)
        {
            // Simple SHA256 fallback if BCrypt not available; project should use BCrypt.Net
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
