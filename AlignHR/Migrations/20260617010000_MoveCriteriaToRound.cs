using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260617010000_MoveCriteriaToRound")]
    public partial class MoveCriteriaToRound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop old FK if it exists
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_HR_EvaluationCriteria_HR_JobPosting'
      AND parent_object_id = OBJECT_ID('HR_EvaluationCriteria'))
    ALTER TABLE [HR_EvaluationCriteria] DROP CONSTRAINT [FK_HR_EvaluationCriteria_HR_JobPosting];
");

            // Step 2: Add new nullable column (separate batch so SQL Server registers it)
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria] ADD [InterviewRoundId] NUMERIC(18,0) NULL;
");

            // Step 3: Populate from existing JobPostingId via the first active round of that posting
            migrationBuilder.Sql(@"
UPDATE ec
SET ec.[InterviewRoundId] = r.[Id]
FROM [HR_EvaluationCriteria] ec
INNER JOIN (
    SELECT [JobPostingId], MIN([Id]) AS [Id]
    FROM [HR_InterviewRound]
    WHERE [IsActive] = 1
    GROUP BY [JobPostingId]
) r ON r.[JobPostingId] = ec.[JobPostingId];
");

            // Step 4: Delete any criteria that had no matching round (orphans)
            migrationBuilder.Sql(@"
DELETE FROM [HR_EvaluationCriteria] WHERE [InterviewRoundId] IS NULL;
");

            // Step 5: Make the column non-nullable
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria] ALTER COLUMN [InterviewRoundId] NUMERIC(18,0) NOT NULL;
");

            // Step 6: Drop old column
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria] DROP COLUMN [JobPostingId];
");

            // Step 7: Add FK to HR_InterviewRound
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria]
    ADD CONSTRAINT [FK_HR_EvaluationCriteria_HR_InterviewRound]
    FOREIGN KEY ([InterviewRoundId]) REFERENCES [HR_InterviewRound]([Id]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria] DROP CONSTRAINT [FK_HR_EvaluationCriteria_HR_InterviewRound];
");
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria] ADD [JobPostingId] NUMERIC(18,0) NOT NULL DEFAULT 0;
");
            migrationBuilder.Sql(@"
ALTER TABLE [HR_EvaluationCriteria] DROP COLUMN [InterviewRoundId];
");
        }
    }
}
