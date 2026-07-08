using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605020000_AddHrRecruitmentSprint3")]
    public partial class AddHrRecruitmentSprint3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- ── HR_RequisitionAssignment ────────────────────────────────
IF OBJECT_ID(N'[HR_RequisitionAssignment]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RequisitionAssignment]
    (
        [Id]                  NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionAssignment] PRIMARY KEY,
        [RequisitionFk]       NUMERIC(18,0) NULL,
        [RecruiterEmployeeFK] NUMERIC(18,0) NULL,
        [AssignedDate]        DATETIME2     NULL,
        [Notes]               NVARCHAR(1000) NULL,
        [CreatedBy]           NVARCHAR(50)  NULL,
        [CreatedOn]           DATETIME2     NULL,
        [UpdatedBy]           NVARCHAR(50)  NULL,
        [UpdatedOn]           DATETIME2     NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_RequisitionAssignment_Requisition_Unique' AND object_id = OBJECT_ID(N'[HR_RequisitionAssignment]'))
    CREATE UNIQUE INDEX [IX_HR_RequisitionAssignment_Requisition_Unique]
    ON [HR_RequisitionAssignment] ([RequisitionFk])
    WHERE [RequisitionFk] IS NOT NULL;

-- ── HR_PostingChannel ────────────────────────────────────────
IF OBJECT_ID(N'[HR_PostingChannel]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_PostingChannel]
    (
        [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_PostingChannel] PRIMARY KEY,
        [ChannelName] NVARCHAR(100) NOT NULL,
        [IsInternal]  BIT NOT NULL,
        [IsActive]    BIT NOT NULL CONSTRAINT [DF_HR_PostingChannel_IsActive] DEFAULT(1)
    );
END;

-- ── HR_JobPosting ────────────────────────────────────────────
IF OBJECT_ID(N'[HR_JobPosting]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_JobPosting]
    (
        [Id]             NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JobPosting] PRIMARY KEY,
        [RequisitionFK]  NUMERIC(18,0)  NULL,
        [JobCode]        NVARCHAR(30)   NULL,
        [JobTitle]       NVARCHAR(200)  NULL,
        [JobDescription] NVARCHAR(1000) NULL,
        [EmploymentType] NVARCHAR(50)   NULL,
        [Location]       NVARCHAR(200)  NULL,
        [PostingStatus]  NVARCHAR(30)   NOT NULL,
        [OpenDate]       DATE           NOT NULL,
        [CloseDate]      DATE           NULL,
        [IsDeleted]      BIT            NOT NULL CONSTRAINT [DF_HR_JobPosting_IsDeleted] DEFAULT(0),
        [CreatedBy]      NVARCHAR(50)   NULL,
        [CreatedOn]      DATETIME2      NULL,
        [UpdatedBy]      NVARCHAR(50)   NULL,
        [UpdatedOn]      DATETIME2      NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_JobPosting_JobCode_Unique' AND object_id = OBJECT_ID(N'[HR_JobPosting]'))
    CREATE UNIQUE INDEX [IX_HR_JobPosting_JobCode_Unique]
    ON [HR_JobPosting] ([JobCode])
    WHERE [JobCode] IS NOT NULL;

-- ── HR_JobPostingChannel ─────────────────────────────────────
IF OBJECT_ID(N'[HR_JobPostingChannel]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_JobPostingChannel]
    (
        [Id]                NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_JobPostingChannel] PRIMARY KEY,
        [JobPostingFK]      NUMERIC(18,0) NOT NULL,
        [PostingChannelFK]  NUMERIC(18,0) NOT NULL,
        [PublishedDate]     DATETIME2     NULL,
        [ExternalReference] NVARCHAR(500) NULL,
        CONSTRAINT [FK_HR_JobPostingChannel_HR_JobPosting]      FOREIGN KEY ([JobPostingFK])     REFERENCES [HR_JobPosting]([Id]),
        CONSTRAINT [FK_HR_JobPostingChannel_HR_PostingChannel]  FOREIGN KEY ([PostingChannelFK]) REFERENCES [HR_PostingChannel]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HR_JobPostingChannel_Posting_Channel_Unique' AND object_id = OBJECT_ID(N'[HR_JobPostingChannel]'))
    CREATE UNIQUE INDEX [IX_HR_JobPostingChannel_Posting_Channel_Unique]
    ON [HR_JobPostingChannel] ([JobPostingFK], [PostingChannelFK]);

-- ── Seed posting channels ────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Internal Portal')
    INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Internal Portal', 1, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'LinkedIn')
    INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'LinkedIn', 0, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Indeed')
    INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Indeed', 0, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Naukri')
    INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Naukri', 0, 1);
IF NOT EXISTS (SELECT 1 FROM [HR_PostingChannel] WHERE [ChannelName] = N'Company Website')
    INSERT INTO [HR_PostingChannel] ([ChannelName],[IsInternal],[IsActive]) VALUES (N'Company Website', 0, 1);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_JobPostingChannel]',    N'U') IS NOT NULL DROP TABLE [HR_JobPostingChannel];
IF OBJECT_ID(N'[HR_JobPosting]',           N'U') IS NOT NULL DROP TABLE [HR_JobPosting];
IF OBJECT_ID(N'[HR_PostingChannel]',       N'U') IS NOT NULL DROP TABLE [HR_PostingChannel];
IF OBJECT_ID(N'[HR_RequisitionAssignment]',N'U') IS NOT NULL DROP TABLE [HR_RequisitionAssignment];
");
        }
    }
}
