using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260609010000_AddHrRecruitmentUnitFunctions")]
    public partial class AddHrRecruitmentUnitFunctions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Index')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Recruitment Units - List', N'View recruitment units', N'/HrRecruitmentUnits/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Create')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Recruitment Units - Create', N'Create a new recruitment unit', N'/HrRecruitmentUnits/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Edit')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Recruitment Units - Edit', N'Edit a recruitment unit', N'/HrRecruitmentUnits/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRecruitmentUnits/Details')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Recruitment Units - Details', N'View recruitment unit details', N'/HrRecruitmentUnits/Details', 1, SYSDATETIME(), 1, SYSDATETIME());
");

            // Assign all 4 functions to Super Admin
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrRecruitmentUnits/Index',
    N'/HrRecruitmentUnits/Create',
    N'/HrRecruitmentUnits/Edit',
    N'/HrRecruitmentUnits/Details'
)
AND r.[Name] = N'Super Admin'
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr
    WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");

            // Index + Details to HRBP, HOD (read-only)
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (N'/HrRecruitmentUnits/Index', N'/HrRecruitmentUnits/Details')
AND r.[Name] IN (N'HRBP', N'HOD', N'Recruiter')
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr
    WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE fr FROM [FunctionRoles] fr
INNER JOIN [functions] f ON fr.[FunctionId] = f.[Id]
WHERE f.[route] IN (
    N'/HrRecruitmentUnits/Index',
    N'/HrRecruitmentUnits/Create',
    N'/HrRecruitmentUnits/Edit',
    N'/HrRecruitmentUnits/Details'
);

DELETE FROM [functions] WHERE [route] IN (
    N'/HrRecruitmentUnits/Index',
    N'/HrRecruitmentUnits/Create',
    N'/HrRecruitmentUnits/Edit',
    N'/HrRecruitmentUnits/Details'
);
");
        }
    }
}
