using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605050000_AddHrRecruitmentSprint6")]
    public partial class AddHrRecruitmentSprint6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- ── HR_Offer ─────────────────────────────────────────────────
IF OBJECT_ID(N'[HR_Offer]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_Offer]
    (
        [Id]                 NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Offer] PRIMARY KEY,
        [JobApplicationFk]   NUMERIC(18,0) NULL,
        [OfferNumber]        NVARCHAR(50)  NULL,
        [ProposedSalary]     DECIMAL(18,2) NOT NULL,
        [ProposedJoiningDate] DATE         NULL,
        [ExpiryDate]         DATE          NULL,
        [Status]             NVARCHAR(50)  NULL CONSTRAINT [DF_HR_Offer_Status] DEFAULT(N'Draft'),
        [CreatedBy]          NVARCHAR(50)  NULL,
        [CreatedOn]          DATETIME2     NULL,
        [UpdatedBy]          NVARCHAR(50)  NULL,
        [UpdatedOn]          DATETIME2     NULL,
        CONSTRAINT [FK_HR_Offer_HR_JobApplication]
            FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_Offer_OfferNumber_Unique' AND object_id=OBJECT_ID(N'[HR_Offer]'))
    CREATE UNIQUE INDEX [IX_HR_Offer_OfferNumber_Unique]
    ON [HR_Offer] ([OfferNumber])
    WHERE [OfferNumber] IS NOT NULL;

-- ── HR_OfferVersion ──────────────────────────────────────────
IF OBJECT_ID(N'[HR_OfferVersion]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OfferVersion]
    (
        [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferVersion] PRIMARY KEY,
        [OfferFk]     NUMERIC(18,0) NULL,
        [VersionNo]   INT           NULL,
        [Salary]      DECIMAL(18,2) NULL,
        [JoiningDate] DATE          NULL,
        [Remarks]     NVARCHAR(1000) NULL,
        [CreatedBy]   NUMERIC(18,0) NULL,
        [CreatedDate] DATETIME2     NULL,
        CONSTRAINT [FK_HR_OfferVersion_HR_Offer]
            FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
    );
END;

-- ── HR_OfferApproval ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_OfferApproval]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OfferApproval]
    (
        [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferApproval] PRIMARY KEY,
        [OfferFk]              NUMERIC(18,0) NULL,
        [ApproverEmployeeFk]   NUMERIC(18,0) NULL,
        [ApprovalLevel]        INT           NULL,
        [Status]               NVARCHAR(50)  NULL CONSTRAINT [DF_HR_OfferApproval_Status] DEFAULT(N'Pending'),
        [Comments]             NVARCHAR(1000) NULL,
        [ActionDate]           DATETIME2     NULL,
        CONSTRAINT [FK_HR_OfferApproval_HR_Offer]
            FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
    );
END;

-- ── HR_OfferDocument ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_OfferDocument]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OfferDocument]
    (
        [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferDocument] PRIMARY KEY,
        [OfferFk]       NUMERIC(18,0) NULL,
        [FileName]      NVARCHAR(500)  NULL,
        [FilePath]      NVARCHAR(1000) NOT NULL,
        [GeneratedDate] DATETIME2      NULL,
        CONSTRAINT [FK_HR_OfferDocument_HR_Offer]
            FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
    );
END;

-- ── HR_OfferResponse ─────────────────────────────────────────
IF OBJECT_ID(N'[HR_OfferResponse]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_OfferResponse]
    (
        [Id]           NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_OfferResponse] PRIMARY KEY,
        [OfferFk]      NUMERIC(18,0) NULL,
        [ResponseType] NVARCHAR(50)  NOT NULL,
        [ResponseDate] DATETIME2     NOT NULL,
        [Comments]     NVARCHAR(1000) NULL,
        CONSTRAINT [FK_HR_OfferResponse_HR_Offer]
            FOREIGN KEY ([OfferFk]) REFERENCES [HR_Offer]([Id])
    );
END;

-- ── HR_HiringDecision ────────────────────────────────────────
IF OBJECT_ID(N'[HR_HiringDecision]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_HiringDecision]
    (
        [Id]               NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_HiringDecision] PRIMARY KEY,
        [JobApplicationFk] NUMERIC(18,0) NULL,
        [Decision]         NVARCHAR(50)  NOT NULL,
        [DecisionBy]       NUMERIC(18,0) NULL,
        [DecisionDate]     DATETIME2     NULL,
        [Remarks]          NVARCHAR(1000) NULL,
        CONSTRAINT [FK_HR_HiringDecision_HR_JobApplication]
            FOREIGN KEY ([JobApplicationFk]) REFERENCES [HR_JobApplication]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HR_HiringDecision_Application_Unique' AND object_id=OBJECT_ID(N'[HR_HiringDecision]'))
    CREATE UNIQUE INDEX [IX_HR_HiringDecision_Application_Unique]
    ON [HR_HiringDecision] ([JobApplicationFk])
    WHERE [JobApplicationFk] IS NOT NULL;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_HiringDecision]',  N'U') IS NOT NULL DROP TABLE [HR_HiringDecision];
IF OBJECT_ID(N'[HR_OfferResponse]',   N'U') IS NOT NULL DROP TABLE [HR_OfferResponse];
IF OBJECT_ID(N'[HR_OfferDocument]',   N'U') IS NOT NULL DROP TABLE [HR_OfferDocument];
IF OBJECT_ID(N'[HR_OfferApproval]',   N'U') IS NOT NULL DROP TABLE [HR_OfferApproval];
IF OBJECT_ID(N'[HR_OfferVersion]',    N'U') IS NOT NULL DROP TABLE [HR_OfferVersion];
IF OBJECT_ID(N'[HR_Offer]',           N'U') IS NOT NULL DROP TABLE [HR_Offer];
");
        }
    }
}
