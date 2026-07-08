using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605030000_AddHrRecruitmentSprint4")]
    public partial class AddHrRecruitmentSprint4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- ── HR_Candidate ─────────────────────────────────────────────
IF OBJECT_ID(N'[HR_Candidate]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_Candidate]
    (
        [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Candidate] PRIMARY KEY,
        [FirstName]            NVARCHAR(100) NOT NULL,
        [LastName]             NVARCHAR(100) NULL,
        [Email]                NVARCHAR(200) NOT NULL,
        [Phone]                NVARCHAR(50)  NULL,
        [CurrentLocation]      NVARCHAR(200) NULL,
        [TotalExperienceYears] DECIMAL(5,2)  NULL,
        [CurrentEmployer]      NVARCHAR(200) NULL,
        [CurrentDesignation]   NVARCHAR(200) NULL,
        [LinkedInProfile]      NVARCHAR(500) NULL,
        [CreatedDate]          DATETIME2     NULL,
        [ModifiedDate]         DATETIME2     NULL,
        [IsDeleted]            BIT NOT NULL  CONSTRAINT [DF_HR_Candidate_IsDeleted] DEFAULT(0)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_Candidate_Email_Unique' AND object_id = OBJECT_ID(N'[HR_Candidate]'))
    CREATE UNIQUE INDEX [IX_HR_Candidate_Email_Unique]
    ON [HR_Candidate] ([Email])
    WHERE [IsDeleted] = 0;

-- ── HR_CandidateDocument ──────────────────────────────────────
IF OBJECT_ID(N'[HR_CandidateDocument]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_CandidateDocument]
    (
        [Id]           NUMERIC(18,0)  IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_CandidateDocument] PRIMARY KEY,
        [CandidateFk]  NUMERIC(18,0)  NULL,
        [DocumentType] NVARCHAR(50)   NULL,
        [FileName]     NVARCHAR(500)  NULL,
        [FilePath]     NVARCHAR(1000) NULL,
        [UploadedDate] DATETIME2      NULL,
        CONSTRAINT [FK_HR_CandidateDocument_HR_Candidate]
            FOREIGN KEY ([CandidateFk]) REFERENCES [HR_Candidate]([Id])
    );
END;

-- ── HR_ApplicationStage ───────────────────────────────────────
IF OBJECT_ID(N'[HR_ApplicationStage]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_ApplicationStage]
    (
        [Id]         NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_ApplicationStage] PRIMARY KEY,
        [StageName]  NVARCHAR(100) NOT NULL,
        [StageOrder] INT           NULL,
        [IsActive]   BIT NOT NULL  CONSTRAINT [DF_HR_ApplicationStage_IsActive] DEFAULT(1)
    );
END;

-- Seed stages
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Applied')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Applied',    1, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Screening')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Screening',  2, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Shortlisted')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Shortlisted',3, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Interview')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Interview',  4, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Final Review')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Final Review',5,1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Selected')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Selected',   6, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Rejected')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Rejected',   7, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Withdrawn')
    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Withdrawn',  8, 1);

-- ── HR_JobApplication ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_JobApplication]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_JobApplication]
    (
        [Id]              NUMERIC(18,0)  IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JobApplication] PRIMARY KEY,
        [CandidateFK]     NUMERIC(18,0)  NULL,
        [JobPostingFK]    NUMERIC(18,0)  NULL,
        [CurrentStageFK]  NUMERIC(18,0)  NULL,
        [AppliedDate]     DATETIME2      NULL,
        [RecruiterNotes]  NVARCHAR(MAX)  NULL,
        [IsActive]        BIT NOT NULL   CONSTRAINT [DF_HR_JobApplication_IsActive] DEFAULT(1),
        CONSTRAINT [FK_HR_JobApplication_HR_Candidate]
            FOREIGN KEY ([CandidateFK])    REFERENCES [HR_Candidate]([Id]),
        CONSTRAINT [FK_HR_JobApplication_HR_JobPosting]
            FOREIGN KEY ([JobPostingFK])   REFERENCES [HR_JobPosting]([Id]),
        CONSTRAINT [FK_HR_JobApplication_HR_ApplicationStage]
            FOREIGN KEY ([CurrentStageFK]) REFERENCES [HR_ApplicationStage]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_JobApplication_Candidate_Posting_Active' AND object_id = OBJECT_ID(N'[HR_JobApplication]'))
    CREATE INDEX [IX_HR_JobApplication_Candidate_Posting_Active]
    ON [HR_JobApplication] ([CandidateFK], [JobPostingFK])
    WHERE [IsActive] = 1;

-- ── HR_ApplicationStageHistory ────────────────────────────────
IF OBJECT_ID(N'[HR_ApplicationStageHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_ApplicationStageHistory]
    (
        [Id]               NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_ApplicationStageHistory] PRIMARY KEY,
        [JobApplicationFk] NUMERIC(18,0) NULL,
        [FromStageId]      NUMERIC(18,0) NULL,
        [ToStageFk]        NUMERIC(18,0) NULL,
        [ChangedBy]        NUMERIC(18,0) NULL,
        [Comments]         NVARCHAR(1000) NULL,
        [ChangedDate]      DATETIME2     NULL,
        CONSTRAINT [FK_HR_StageHistory_HR_JobApplication]
            FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication]([Id]),
        CONSTRAINT [FK_HR_StageHistory_FromStage]
            FOREIGN KEY ([FromStageId]) REFERENCES [HR_ApplicationStage]([Id]),
        CONSTRAINT [FK_HR_StageHistory_ToStage]
            FOREIGN KEY ([ToStageFk])   REFERENCES [HR_ApplicationStage]([Id])
    );
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_ApplicationStageHistory]',N'U') IS NOT NULL DROP TABLE [HR_ApplicationStageHistory];
IF OBJECT_ID(N'[HR_JobApplication]',         N'U') IS NOT NULL DROP TABLE [HR_JobApplication];
IF OBJECT_ID(N'[HR_ApplicationStage]',       N'U') IS NOT NULL DROP TABLE [HR_ApplicationStage];
IF OBJECT_ID(N'[HR_CandidateDocument]',      N'U') IS NOT NULL DROP TABLE [HR_CandidateDocument];
IF OBJECT_ID(N'[HR_Candidate]',              N'U') IS NOT NULL DROP TABLE [HR_Candidate];
");
        }
    }
}
