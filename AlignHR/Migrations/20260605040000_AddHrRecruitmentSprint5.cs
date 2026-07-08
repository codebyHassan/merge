using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605040000_AddHrRecruitmentSprint5")]
    public partial class AddHrRecruitmentSprint5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- ── HR_InterviewRound ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_InterviewRound]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_InterviewRound]
    (
        [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewRound] PRIMARY KEY,
        [JobPostingId] NUMERIC(18,0) NOT NULL,
        [RoundName]   NVARCHAR(100) NOT NULL,
        [RoundOrder]  INT NOT NULL,
        [IsMandatory] BIT NOT NULL CONSTRAINT [DF_HR_InterviewRound_IsMandatory] DEFAULT(1),
        [IsActive]    BIT NOT NULL CONSTRAINT [DF_HR_InterviewRound_IsActive]    DEFAULT(1)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_InterviewRound_Posting_Order' AND object_id=OBJECT_ID(N'[HR_InterviewRound]'))
    CREATE UNIQUE INDEX [IX_HR_InterviewRound_Posting_Order]
    ON [HR_InterviewRound] ([JobPostingId], [RoundOrder])
    WHERE [IsActive] = 1;

-- ── HR_InterviewSchedule ──────────────────────────────────────
IF OBJECT_ID(N'[HR_InterviewSchedule]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_InterviewSchedule]
    (
        [Id]                  NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewSchedule] PRIMARY KEY,
        [JobApplicationId]    NUMERIC(18,0) NOT NULL,
        [InterviewRoundId]    NUMERIC(18,0) NOT NULL,
        [ScheduledDateTime]   DATETIME2     NOT NULL,
        [MeetingLink]         NVARCHAR(1000) NULL,
        [Location]            NVARCHAR(500)  NULL,
        [Status]              NVARCHAR(50)  NOT NULL,
        [CreatedBy]           NUMERIC(18,0) NOT NULL,
        [CreatedDate]         DATETIME2     NOT NULL,
        CONSTRAINT [FK_HR_InterviewSchedule_HR_JobApplication]
            FOREIGN KEY ([JobApplicationId]) REFERENCES [HR_JobApplication]([Id]),
        CONSTRAINT [FK_HR_InterviewSchedule_HR_InterviewRound]
            FOREIGN KEY ([InterviewRoundId]) REFERENCES [HR_InterviewRound]([Id])
    );
END;

-- ── HR_InterviewPanel ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_InterviewPanel]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_InterviewPanel]
    (
        [Id]                  NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewPanel] PRIMARY KEY,
        [InterviewScheduleId] NUMERIC(18,0) NOT NULL,
        [EmployeeFk]          NUMERIC(18,0) NOT NULL,
        CONSTRAINT [FK_HR_InterviewPanel_HR_InterviewSchedule]
            FOREIGN KEY ([InterviewScheduleId]) REFERENCES [HR_InterviewSchedule]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_InterviewPanel_Schedule_Employee' AND object_id=OBJECT_ID(N'[HR_InterviewPanel]'))
    CREATE UNIQUE INDEX [IX_HR_InterviewPanel_Schedule_Employee]
    ON [HR_InterviewPanel] ([InterviewScheduleId], [EmployeeFk]);

-- ── HR_InterviewFeedback ──────────────────────────────────────
IF OBJECT_ID(N'[HR_InterviewFeedback]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_InterviewFeedback]
    (
        [Id]                     NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_InterviewFeedback] PRIMARY KEY,
        [InterviewScheduleFk]    NUMERIC(18,0) NOT NULL,
        [InterviewerEmployeeFk]  NUMERIC(18,0) NOT NULL,
        [OverallScore]           DECIMAL(5,2)  NULL,
        [Recommendation]         NVARCHAR(50)  NOT NULL,
        [Strengths]              NVARCHAR(MAX) NULL,
        [Concerns]               NVARCHAR(MAX) NULL,
        [Comments]               NVARCHAR(MAX) NULL,
        [SubmittedDate]          DATETIME2     NOT NULL,
        CONSTRAINT [FK_HR_InterviewFeedback_HR_InterviewSchedule]
            FOREIGN KEY ([InterviewScheduleFk]) REFERENCES [HR_InterviewSchedule]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_InterviewFeedback_Schedule_Interviewer' AND object_id=OBJECT_ID(N'[HR_InterviewFeedback]'))
    CREATE UNIQUE INDEX [IX_HR_InterviewFeedback_Schedule_Interviewer]
    ON [HR_InterviewFeedback] ([InterviewScheduleFk], [InterviewerEmployeeFk]);

-- ── HR_EvaluationCriteria ─────────────────────────────────────
IF OBJECT_ID(N'[HR_EvaluationCriteria]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_EvaluationCriteria]
    (
        [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_EvaluationCriteria] PRIMARY KEY,
        [JobPostingId]  NUMERIC(18,0) NOT NULL,
        [CriteriaName]  NVARCHAR(200) NOT NULL,
        [MaxScore]      DECIMAL(5,2)  NOT NULL
    );
END;

-- ── HR_EvaluationScore ────────────────────────────────────────
IF OBJECT_ID(N'[HR_EvaluationScore]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_EvaluationScore]
    (
        [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_EvaluationScore] PRIMARY KEY,
        [InterviewFeedbackFk]  NUMERIC(18,0) NOT NULL,
        [EvaluationCriteriaFk] NUMERIC(18,0) NOT NULL,
        [Score]                DECIMAL(5,2)  NOT NULL,
        CONSTRAINT [FK_HR_EvaluationScore_HR_InterviewFeedback]
            FOREIGN KEY ([InterviewFeedbackFk])  REFERENCES [HR_InterviewFeedback]([Id]),
        CONSTRAINT [FK_HR_EvaluationScore_HR_EvaluationCriteria]
            FOREIGN KEY ([EvaluationCriteriaFk]) REFERENCES [HR_EvaluationCriteria]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_EvaluationScore_Feedback_Criteria' AND object_id=OBJECT_ID(N'[HR_EvaluationScore]'))
    CREATE UNIQUE INDEX [IX_HR_EvaluationScore_Feedback_Criteria]
    ON [HR_EvaluationScore] ([InterviewFeedbackFk], [EvaluationCriteriaFk]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_EvaluationScore]',    N'U') IS NOT NULL DROP TABLE [HR_EvaluationScore];
IF OBJECT_ID(N'[HR_EvaluationCriteria]', N'U') IS NOT NULL DROP TABLE [HR_EvaluationCriteria];
IF OBJECT_ID(N'[HR_InterviewFeedback]',  N'U') IS NOT NULL DROP TABLE [HR_InterviewFeedback];
IF OBJECT_ID(N'[HR_InterviewPanel]',     N'U') IS NOT NULL DROP TABLE [HR_InterviewPanel];
IF OBJECT_ID(N'[HR_InterviewSchedule]',  N'U') IS NOT NULL DROP TABLE [HR_InterviewSchedule];
IF OBJECT_ID(N'[HR_InterviewRound]',     N'U') IS NOT NULL DROP TABLE [HR_InterviewRound];
");
        }
    }
}
