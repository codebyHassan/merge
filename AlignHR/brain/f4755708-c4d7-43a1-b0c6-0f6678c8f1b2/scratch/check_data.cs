using System;
using System.Linq;
using AlignHR.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AlignHR.Scratch
{
    public class CheckData
    {
        public static void Run(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            var policies = context.EmployeeLeavePolicies
                .Include(p => p.Employee)
                .Include(p => p.LeavePolicy)
                .ToList();
            
            Console.WriteLine($"Total Employee Policies: {policies.Count}");
            foreach(var p in policies)
            {
                Console.WriteLine($"Emp: {p.Employee?.FullName} (ID: {p.EmployeeId}), Policy: {p.LeavePolicy?.PolicyName}, Active: {p.IsActive}");
            }

            var employees = context.emp.ToList();
            Console.WriteLine($"Total Employees: {employees.Count}");
            foreach(var e in employees)
            {
                Console.WriteLine($"Emp: {e.FullName} (ID: {e.Id})");
            }
        }
    }
}
