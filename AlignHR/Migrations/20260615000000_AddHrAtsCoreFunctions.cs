using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260615000000_AddHrAtsCoreFunctions")]
    public partial class AddHrAtsCoreFunctions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Seed function records ──────────────────────────────
            migrationBuilder.Sql(@"
-- HrJobPostings
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Index')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Job Postings - List', N'View all job postings', N'/HrJobPostings/Index', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Create')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Job Postings - Create', N'Create a new job posting', N'/HrJobPostings/Create', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Edit')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Job Postings - Edit', N'Edit an existing job posting', N'/HrJobPostings/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrJobPostings/Details')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Job Postings - Details', N'View job posting details', N'/HrJobPostings/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

-- HrCandidates
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrCandidates/Index')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Candidates - List', N'View all candidates', N'/HrCandidates/Index', 1, SYSDATETIME(), 1, SYSDATETIME());


IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrCandidates/Edit')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Candidates - Edit', N'Edit a candidate profile', N'/HrCandidates/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrCandidates/Details')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Candidates - Details', N'View candidate profile', N'/HrCandidates/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

-- HrApplications
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/Index')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Applications - List', N'View applications for a job posting', N'/HrApplications/Index', 1, SYSDATETIME(), 1, SYSDATETIME());


IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/Details')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Applications - Details', N'View application (candidate card)', N'/HrApplications/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/Pipeline')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Applications - Pipeline', N'View Kanban pipeline for a job posting', N'/HrApplications/Pipeline', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrApplications/MoveStage')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Applications - Move Stage', N'Move candidate to another pipeline stage', N'/HrApplications/MoveStage', 1, SYSDATETIME(), 1, SYSDATETIME());

-- HrInterviews
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/ApplicationInterviews')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Interviews - Index', N'View all interviews for an application', N'/HrInterviews/ApplicationInterviews', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/Schedule')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Interviews - Schedule', N'Schedule an interview for a candidate', N'/HrInterviews/Schedule', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/Details')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Interviews - Details', N'View interview schedule details', N'/HrInterviews/Details', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/SubmitFeedback')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Interviews - Submit Feedback', N'Submit interview feedback / scorecard', N'/HrInterviews/SubmitFeedback', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/Edit')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Interviews - Edit', N'Edit an existing interview schedule', N'/HrInterviews/Edit', 1, SYSDATETIME(), 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrInterviews/FeedbackDetails')
    INSERT INTO [functions] ([Name],[Description],[route],[createdby],[createat],[updatedby],[updateat])
    VALUES (N'Interviews - Feedback Details', N'View submitted interview feedback', N'/HrInterviews/FeedbackDetails', 1, SYSDATETIME(), 1, SYSDATETIME());
");

            // ── Assign ALL above functions to Super Admin ──────────
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrJobPostings/Index',   N'/HrJobPostings/Create',  N'/HrJobPostings/Edit',   N'/HrJobPostings/Details',
    N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
    N'/HrApplications/Index',  N'/HrApplications/Details',
    N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
    N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Schedule', N'/HrInterviews/Edit', N'/HrInterviews/Details',
    N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
)
AND r.[Name] = N'Super Admin'
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");

            // ── Assign to HRBP: full access (manage postings, candidates, applications, interviews) ──
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrJobPostings/Index',   N'/HrJobPostings/Create',  N'/HrJobPostings/Edit',   N'/HrJobPostings/Details',
    N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
    N'/HrApplications/Index',  N'/HrApplications/Details',
    N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
    N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Schedule', N'/HrInterviews/Edit', N'/HrInterviews/Details',
    N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
)
AND r.[Name] = N'HRBP'
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");

            // ── Assign to Recruiter: full candidate + application + interview access ──
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrJobPostings/Index',    N'/HrJobPostings/Details',
    N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
    N'/HrApplications/Index',  N'/HrApplications/Details',
    N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
    N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Schedule', N'/HrInterviews/Edit', N'/HrInterviews/Details',
    N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
)
AND r.[Name] = N'Recruiter'
AND NOT EXISTS (
    SELECT 1 FROM [FunctionRoles] fr WHERE fr.[FunctionId] = f.[Id] AND fr.[RoleId] = r.[Id]
);
");

            // ── HOD: read-only on postings, candidates, applications; can submit interview feedback ──
            migrationBuilder.Sql(@"
INSERT INTO [FunctionRoles] ([FunctionId], [RoleId], [createdby], [createat])
SELECT f.[Id], r.[Id], 1, SYSDATETIME()
FROM [functions] f
CROSS JOIN [roles] r
WHERE f.[route] IN (
    N'/HrJobPostings/Index',    N'/HrJobPostings/Details',
    N'/HrCandidates/Index',     N'/HrCandidates/Details',
    N'/HrApplications/Index',   N'/HrApplications/Details', N'/HrApplications/Pipeline',
    N'/HrInterviews/ApplicationInterviews', N'/HrInterviews/Details',   N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
)
AND r.[Name] = N'HOD'
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
WHERE f.[route] IN (
    N'/HrJobPostings/Index',   N'/HrJobPostings/Create',  N'/HrJobPostings/Edit',   N'/HrJobPostings/Details',
    N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
    N'/HrApplications/Index',  N'/HrApplications/Details',
    N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
    N'/HrInterviews/Schedule', N'/HrInterviews/Details',
    N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
);

DELETE FROM [functions] WHERE [route] IN (
    N'/HrJobPostings/Index',   N'/HrJobPostings/Create',  N'/HrJobPostings/Edit',   N'/HrJobPostings/Details',
    N'/HrCandidates/Index',    N'/HrCandidates/Edit',    N'/HrCandidates/Details',
    N'/HrApplications/Index',  N'/HrApplications/Details',
    N'/HrApplications/Pipeline', N'/HrApplications/MoveStage',
    N'/HrInterviews/Schedule', N'/HrInterviews/Details',
    N'/HrInterviews/SubmitFeedback', N'/HrInterviews/FeedbackDetails'
);
");
        }
    }
}
