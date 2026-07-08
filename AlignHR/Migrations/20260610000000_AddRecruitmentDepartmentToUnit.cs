using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260610000000_AddRecruitmentDepartmentToUnit")]
    public partial class AddRecruitmentDepartmentToUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentDepartmentId'
)
BEGIN
    ALTER TABLE [HR_RecruitmentUnit] ADD [RecruitmentDepartmentId] INT NULL;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'HR_RecruitmentUnit' AND COLUMN_NAME = 'RecruitmentDepartmentId'
)
BEGIN
    ALTER TABLE [HR_RecruitmentUnit] DROP COLUMN [RecruitmentDepartmentId];
END
");
        }
    }
}
