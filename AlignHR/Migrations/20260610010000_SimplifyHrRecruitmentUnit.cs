using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260610010000_SimplifyHrRecruitmentUnit")]
    public partial class SimplifyHrRecruitmentUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add BusinessDepartmentId column
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'BusinessDepartmentId'
)
BEGIN
    ALTER TABLE [HR_RecruitmentUnit] ADD [BusinessDepartmentId] INT NOT NULL DEFAULT 0;
END
");

            // Ensure RecruitmentDepartmentId exists and is non-nullable with default 0
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentDepartmentId'
)
BEGIN
    ALTER TABLE [HR_RecruitmentUnit] ADD [RecruitmentDepartmentId] INT NOT NULL DEFAULT 0;
END
ELSE
BEGIN
    -- Update any NULLs before making non-nullable
    UPDATE [HR_RecruitmentUnit] SET [RecruitmentDepartmentId] = 0 WHERE [RecruitmentDepartmentId] IS NULL;
    ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [RecruitmentDepartmentId] INT NOT NULL;
END
");

            // Make legacy columns nullable so EF inserts no longer need to provide them
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'UnitName'
)
BEGIN
    -- Drop the NOT NULL constraint by making the column nullable
    ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [UnitName] NVARCHAR(200) NULL;
END

IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentHeadEmployeeId'
)
BEGIN
    ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [RecruitmentHeadEmployeeId] NUMERIC(18,0) NULL;
END
");

            // Register the RecruitmentHead approver type permission entry in functions table
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [functions] WHERE [route] = N'/HrDepartmentApprovers/RecruitmentHead')
BEGIN
    -- No separate route needed; RecruitmentHead is handled by existing DeptApprovers routes
    -- Just ensure existing DeptApprovers functions cover Create/Edit for this type (they do)
    SELECT 1; -- no-op placeholder
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'BusinessDepartmentId'
)
BEGIN
    ALTER TABLE [HR_RecruitmentUnit] DROP COLUMN [BusinessDepartmentId];
END

-- Restore NOT NULL on legacy columns
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'UnitName'
)
BEGIN
    UPDATE [HR_RecruitmentUnit] SET [UnitName] = N'' WHERE [UnitName] IS NULL;
    ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [UnitName] NVARCHAR(200) NOT NULL;
END

IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentHeadEmployeeId'
)
BEGIN
    UPDATE [HR_RecruitmentUnit] SET [RecruitmentHeadEmployeeId] = 0 WHERE [RecruitmentHeadEmployeeId] IS NULL;
    ALTER TABLE [HR_RecruitmentUnit] ALTER COLUMN [RecruitmentHeadEmployeeId] NUMERIC(18,0) NOT NULL;
END
");
        }
    }
}
