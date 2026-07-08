using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260616020000_AddLineManagerFeedbackFunction")]
    public partial class AddLineManagerFeedbackFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/LineManagerFeedback')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Requisitions - Interview Feedback', N'Line manager gives interview feedback for their requisition', N'/HrRequisitions/LineManagerFeedback', 1, SYSDATETIME(), 1, SYSDATETIME());

INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] = N'/HrRequisitions/LineManagerFeedback'
AND r.[Name] IN (N'Super Admin', N'HRBP', N'Recruiter', N'HOD', N'Employee')
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE fr FROM [FunctionRoles] fr
INNER JOIN [functions] f ON fr.[FunctionId] = f.[Id]
WHERE f.[route] = N'/HrRequisitions/LineManagerFeedback';

DELETE FROM [functions] WHERE [route] = N'/HrRequisitions/LineManagerFeedback';
");
        }
    }
}
