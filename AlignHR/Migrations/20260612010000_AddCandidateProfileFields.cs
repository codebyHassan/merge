using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    public partial class AddCandidateProfileFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='City')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [City] NVARCHAR(100) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='Country')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [Country] NVARCHAR(100) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='DateOfBirth')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [DateOfBirth] DATE NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='Gender')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [Gender] NVARCHAR(50) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='HighestDegree')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [HighestDegree] NVARCHAR(100) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='FieldOfStudy')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [FieldOfStudy] NVARCHAR(200) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='University')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [University] NVARCHAR(200) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='IsCurrentlyEmployed')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [IsCurrentlyEmployed] BIT NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='CurrentEmployer')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [CurrentEmployer] NVARCHAR(200) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='CurrentDesignation')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [CurrentDesignation] NVARCHAR(200) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='NoticePeriod')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [NoticePeriod] NVARCHAR(50) NULL;

                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='ResumeFileName')
                    ALTER TABLE [dbo].[HR_Candidate] ADD [ResumeFileName] NVARCHAR(500) NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='City')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [City];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='Country')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [Country];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='DateOfBirth')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [DateOfBirth];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='Gender')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [Gender];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='HighestDegree')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [HighestDegree];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='FieldOfStudy')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [FieldOfStudy];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='University')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [University];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='IsCurrentlyEmployed')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [IsCurrentlyEmployed];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='CurrentEmployer')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [CurrentEmployer];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='CurrentDesignation')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [CurrentDesignation];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='NoticePeriod')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [NoticePeriod];
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='HR_Candidate' AND COLUMN_NAME='ResumeFileName')
                    ALTER TABLE [dbo].[HR_Candidate] DROP COLUMN [ResumeFileName];
            ");
        }
    }
}
