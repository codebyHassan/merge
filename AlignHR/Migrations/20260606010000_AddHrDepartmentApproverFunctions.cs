using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260606010000_AddHrDepartmentApproverFunctions")]
    public partial class AddHrDepartmentApproverFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Seed function records for HrDepartmentApprovers ──────────────
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/Index')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Dept. Approvers - List', N'View department approver configuration', N'/HrDepartmentApprovers/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/Create')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Dept. Approvers - Create', N'Add a new department approver', N'/HrDepartmentApprovers/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/Edit')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Dept. Approvers - Edit', N'Edit an existing department approver', N'/HrDepartmentApprovers/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());
");

            // ── Assign all 3 functions to Super Admin ─────────────────────────
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrDepartmentApprovers/Index',
    N'/HrDepartmentApprovers/Create',
    N'/HrDepartmentApprovers/Edit'
)
AND r.[Name] = N'Super Admin'
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr
    WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");

            // ── Assign Index-only to HRBP and HOD (read-only access) ──────────
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] = N'/HrDepartmentApprovers/Index'
AND r.[Name] IN (N'HRBP', N'HOD')
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr
    WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Remove FunctionRoles for HrDepartmentApprovers
DELETE fr
FROM [FunctionRoles] fr
INNER JOIN [functions] f ON fr.[FunctionId] = f.[Id]
WHERE f.[route] IN (
    N'/HrDepartmentApprovers/Index',
    N'/HrDepartmentApprovers/Create',
    N'/HrDepartmentApprovers/Edit'
);

-- Remove the function records themselves
DELETE FROM [functions]
WHERE [route] IN (
    N'/HrDepartmentApprovers/Index',
    N'/HrDepartmentApprovers/Create',
    N'/HrDepartmentApprovers/Edit'
);
");
        }
    }
}
