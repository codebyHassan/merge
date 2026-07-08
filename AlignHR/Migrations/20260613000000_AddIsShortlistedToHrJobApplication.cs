using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    public partial class AddIsShortlistedToHrJobApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_JobApplication' AND COLUMN_NAME='IsShortlisted')
                    ALTER TABLE [dbo].[HR_JobApplication] ADD [IsShortlisted] BIT NOT NULL DEFAULT(0);

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_JobApplication' AND COLUMN_NAME='ShortlistedOn')
                    ALTER TABLE [dbo].[HR_JobApplication] ADD [ShortlistedOn] DATETIME NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_JobApplication' AND COLUMN_NAME='ShortlistNotes')
                    ALTER TABLE [dbo].[HR_JobApplication] ADD [ShortlistNotes] NVARCHAR(500) NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_JobApplication' AND COLUMN_NAME='ShortlistNotes')
                    ALTER TABLE [dbo].[HR_JobApplication] DROP COLUMN [ShortlistNotes];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_JobApplication' AND COLUMN_NAME='ShortlistedOn')
                    ALTER TABLE [dbo].[HR_JobApplication] DROP COLUMN [ShortlistedOn];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_JobApplication' AND COLUMN_NAME='IsShortlisted')
                    ALTER TABLE [dbo].[HR_JobApplication] DROP COLUMN [IsShortlisted];
            ");
        }
    }
}
