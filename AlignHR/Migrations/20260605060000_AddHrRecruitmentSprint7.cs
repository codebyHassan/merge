using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605060000_AddHrRecruitmentSprint7")]
    public partial class AddHrRecruitmentSprint7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- ── HR_OnboardingTaskTemplate ─────────────────────────────────
IF OBJECT_ID(N'[HR_OnboardingTaskTemplate]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OnboardingTaskTemplate]
    (
        [Id]                    NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OnboardingTaskTemplate] PRIMARY KEY,
        [TaskName]              NVARCHAR(200) NOT NULL,
        [ResponsibleDepartment] NVARCHAR(100) NOT NULL,
        [IsMandatory]           BIT NOT NULL CONSTRAINT [DF_HR_OnboardingTaskTemplate_IsMandatory] DEFAULT(1),
        [IsActive]              BIT NOT NULL CONSTRAINT [DF_HR_OnboardingTaskTemplate_IsActive]    DEFAULT(1)
    );
END;

-- Seed default task templates
IF NOT EXISTS (SELECT 1 FROM [HR_OnboardingTaskTemplate] WHERE [TaskName] = N'Create Email Account')
BEGIN
    INSERT INTO [HR_OnboardingTaskTemplate] ([TaskName], [ResponsibleDepartment], [IsMandatory], [IsActive]) VALUES
    (N'Create Email Account',   N'IT',       1, 1),
    (N'Allocate Laptop',        N'IT',       1, 1),
    (N'Issue Access Card',      N'Admin',    1, 1),
    (N'Assign Buddy',           N'HR',       0, 1),
    (N'Prepare Workstation',    N'Admin',    1, 1),
    (N'Payroll Registration',   N'Payroll',  1, 1),
    (N'Add to Leave System',    N'HR',       1, 1),
    (N'Safety Induction',       N'HR',       1, 1);
END;

-- ── HR_Onboarding ─────────────────────────────────────────────
IF OBJECT_ID(N'[HR_Onboarding]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_Onboarding]
    (
        [Id]                 NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Onboarding] PRIMARY KEY,
        [CandidateId]        NUMERIC(18,0) NOT NULL,
        [OfferId]            NUMERIC(18,0) NOT NULL,
        [PlannedJoiningDate] DATE          NOT NULL,
        [ActualJoiningDate]  DATE          NULL,
        [Status]             NVARCHAR(50)  NOT NULL CONSTRAINT [DF_HR_Onboarding_Status] DEFAULT(N'Initiated'),
        [CreatedBy]          NVARCHAR(50)  NULL,
        [CreatedOn]          DATETIME2     NULL,
        [UpdatedBy]          NVARCHAR(50)  NULL,
        [UpdatedOn]          DATETIME2     NULL,
        CONSTRAINT [FK_HR_Onboarding_HR_Candidate]
            FOREIGN KEY ([CandidateId]) REFERENCES [HR_Candidate]([Id]),
        CONSTRAINT [FK_HR_Onboarding_HR_Offer]
            FOREIGN KEY ([OfferId]) REFERENCES [HR_Offer]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_Onboarding_Offer_Unique' AND object_id=OBJECT_ID(N'[HR_Onboarding]'))
    CREATE UNIQUE INDEX [IX_HR_Onboarding_Offer_Unique]
    ON [HR_Onboarding] ([OfferId]);

-- ── HR_OnboardingTask ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_OnboardingTask]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OnboardingTask]
    (
        [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OnboardingTask] PRIMARY KEY,
        [OnboardingFk]         NUMERIC(18,0) NOT NULL,
        [TaskTemplateFk]       NUMERIC(18,0) NOT NULL,
        [AssignedToEmployeeFk] NUMERIC(18,0) NULL,
        [DueDate]              DATE          NULL,
        [Status]               NVARCHAR(50)  NOT NULL CONSTRAINT [DF_HR_OnboardingTask_Status] DEFAULT(N'Pending'),
        [CompletedDate]        DATETIME2     NULL,
        [Remarks]              NVARCHAR(1000) NULL,
        CONSTRAINT [FK_HR_OnboardingTask_HR_Onboarding]
            FOREIGN KEY ([OnboardingFk]) REFERENCES [HR_Onboarding]([Id]),
        CONSTRAINT [FK_HR_OnboardingTask_HR_OnboardingTaskTemplate]
            FOREIGN KEY ([TaskTemplateFk]) REFERENCES [HR_OnboardingTaskTemplate]([Id])
    );
END;

-- ── HR_OnboardingDocument ─────────────────────────────────────
IF OBJECT_ID(N'[HR_OnboardingDocument]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OnboardingDocument]
    (
        [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OnboardingDocument] PRIMARY KEY,
        [OnboardingFk]  NUMERIC(18,0) NOT NULL,
        [DocumentType]  NVARCHAR(100) NOT NULL,
        [FileName]      NVARCHAR(500) NOT NULL,
        [FilePath]      NVARCHAR(1000) NOT NULL,
        [UploadedDate]  DATETIME2     NOT NULL,
        CONSTRAINT [FK_HR_OnboardingDocument_HR_Onboarding]
            FOREIGN KEY ([OnboardingFk]) REFERENCES [HR_Onboarding]([Id])
    );
END;

-- ── HR_JoiningConfirmation ────────────────────────────────────
IF OBJECT_ID(N'[HR_JoiningConfirmation]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_JoiningConfirmation]
    (
        [Id]                    NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JoiningConfirmation] PRIMARY KEY,
        [OnboardingId]          NUMERIC(18,0) NOT NULL,
        [JoinedDate]            DATE          NOT NULL,
        [ConfirmedByEmployeeFk] NUMERIC(18,0) NOT NULL,
        [Remarks]               NVARCHAR(1000) NULL,
        [ConfirmedDate]         DATETIME2     NOT NULL,
        CONSTRAINT [FK_HR_JoiningConfirmation_HR_Onboarding]
            FOREIGN KEY ([OnboardingId]) REFERENCES [HR_Onboarding]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_JoiningConfirmation_Onboarding_Unique' AND object_id=OBJECT_ID(N'[HR_JoiningConfirmation]'))
    CREATE UNIQUE INDEX [IX_HR_JoiningConfirmation_Onboarding_Unique]
    ON [HR_JoiningConfirmation] ([OnboardingId]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_JoiningConfirmation]',    N'U') IS NOT NULL DROP TABLE [HR_JoiningConfirmation];
IF OBJECT_ID(N'[HR_OnboardingDocument]',     N'U') IS NOT NULL DROP TABLE [HR_OnboardingDocument];
IF OBJECT_ID(N'[HR_OnboardingTask]',         N'U') IS NOT NULL DROP TABLE [HR_OnboardingTask];
IF OBJECT_ID(N'[HR_Onboarding]',             N'U') IS NOT NULL DROP TABLE [HR_Onboarding];
IF OBJECT_ID(N'[HR_OnboardingTaskTemplate]', N'U') IS NOT NULL DROP TABLE [HR_OnboardingTaskTemplate];
");
        }
    }
}
