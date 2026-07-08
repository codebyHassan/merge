using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260606020000_AddHrRequisitionWorkflowFunctions")]
    public partial class AddHrRequisitionWorkflowFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Seed core Requisitions function records ───────────────────────
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Index')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Requisitions - List', N'View all HR requisitions', N'/HrRequisitions/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Create')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Requisitions - Create', N'Create a new HR requisition', N'/HrRequisitions/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Edit')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Requisitions - Edit', N'Edit a draft HR requisition', N'/HrRequisitions/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Details')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Requisitions - Details', N'View requisition details', N'/HrRequisitions/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Inbox')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Requisitions - Approval Inbox', N'View pending requisition approvals assigned to me', N'/HrRequisitions/Inbox', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrRequisitions/Action')
    INSERT INTO [functions] ([Name], [Description], [route], [createdby], [createat], [updatedby], [updateat])
    VALUES (N'Requisitions - Approve/Reject', N'Approve, reject or send back a requisition', N'/HrRequisitions/Action', 1, SYSDATETIME(), 1, SYSDATETIME());
");

            // ── Assign all Requisition functions to Super Admin ───────────────
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrRequisitions/Index',
    N'/HrRequisitions/Create',
    N'/HrRequisitions/Edit',
    N'/HrRequisitions/Details',
    N'/HrRequisitions/Inbox',
    N'/HrRequisitions/Action'
)
AND r.[Name] = N'Super Admin'
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr
    WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");

            // ── Assign Inbox + Action + Details to HRBP, HOD, Recruiter ───────
            // These roles are the approvers — they need Inbox and Action to do their job
            // They also get Details (read-only) and Index (to see the list)
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrRequisitions/Index',
    N'/HrRequisitions/Details',
    N'/HrRequisitions/Inbox',
    N'/HrRequisitions/Action'
)
AND r.[Name] IN (N'HRBP', N'HOD', N'Recruiter')
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
-- Remove FunctionRoles for HrRequisitions workflow functions
DELETE fr
FROM [FunctionRoles] fr
INNER JOIN [functions] f ON fr.[FunctionId] = f.[Id]
WHERE f.[route] IN (
    N'/HrRequisitions/Index',
    N'/HrRequisitions/Create',
    N'/HrRequisitions/Edit',
    N'/HrRequisitions/Details',
    N'/HrRequisitions/Inbox',
    N'/HrRequisitions/Action'
);

-- Remove the function records
DELETE FROM [functions]
WHERE [route] IN (
    N'/HrRequisitions/Index',
    N'/HrRequisitions/Create',
    N'/HrRequisitions/Edit',
    N'/HrRequisitions/Details',
    N'/HrRequisitions/Inbox',
    N'/HrRequisitions/Action'
);
");
        }
    }
}
