using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260609000000_AddHrRequisitionRefactorSprint2Rev")]
    public partial class AddHrRequisitionRefactorSprint2Rev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. HR_Requisition: add new columns ──────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_Requisition]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'HR_Requisition', N'InitialDate') IS NULL
        ALTER TABLE [HR_Requisition] ADD [InitialDate] DATE NULL;

    IF COL_LENGTH(N'HR_Requisition', N'PromisedDate') IS NULL
        ALTER TABLE [HR_Requisition] ADD [PromisedDate] DATE NULL;

    IF COL_LENGTH(N'HR_Requisition', N'RequisitionType') IS NULL
        ALTER TABLE [HR_Requisition] ADD [RequisitionType] NVARCHAR(50) NULL;

    IF COL_LENGTH(N'HR_Requisition', N'Nature') IS NULL
        ALTER TABLE [HR_Requisition] ADD [Nature] NVARCHAR(50) NULL;

    IF COL_LENGTH(N'HR_Requisition', N'BudgetAmountPerMonth') IS NULL
        ALTER TABLE [HR_Requisition] ADD [BudgetAmountPerMonth] DECIMAL(18,2) NULL;

    IF COL_LENGTH(N'HR_Requisition', N'ReplacementEmployeeId') IS NULL
        ALTER TABLE [HR_Requisition] ADD [ReplacementEmployeeId] NUMERIC(18,0) NULL;

    IF COL_LENGTH(N'HR_Requisition', N'TransferFromDepartmentId') IS NULL
        ALTER TABLE [HR_Requisition] ADD [TransferFromDepartmentId] NUMERIC(18,0) NULL;

    IF COL_LENGTH(N'HR_Requisition', N'TransferToDepartmentId') IS NULL
        ALTER TABLE [HR_Requisition] ADD [TransferToDepartmentId] NUMERIC(18,0) NULL;
END;
");

            // ── 2. HR_RequisitionType master table ──────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RequisitionType]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RequisitionType]
    (
        [Id]   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionType] PRIMARY KEY,
        [Code] NVARCHAR(50)  NOT NULL,
        [Name] NVARCHAR(100) NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionType] WHERE [Code] = N'Replacement')
    INSERT INTO [HR_RequisitionType] ([Code], [Name]) VALUES (N'Replacement', N'Replacement');

IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionType] WHERE [Code] = N'Transfer')
    INSERT INTO [HR_RequisitionType] ([Code], [Name]) VALUES (N'Transfer', N'Transfer');

IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionType] WHERE [Code] = N'NewVacancy')
    INSERT INTO [HR_RequisitionType] ([Code], [Name]) VALUES (N'NewVacancy', N'New Vacancy');
");

            // ── 3. HR_RequisitionNature master table ────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RequisitionNature]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RequisitionNature]
    (
        [Id]   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionNature] PRIMARY KEY,
        [Code] NVARCHAR(50)  NOT NULL,
        [Name] NVARCHAR(100) NOT NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionNature] WHERE [Code] = N'Budgeted')
    INSERT INTO [HR_RequisitionNature] ([Code], [Name]) VALUES (N'Budgeted', N'Budgeted');

IF NOT EXISTS (SELECT 1 FROM [HR_RequisitionNature] WHERE [Code] = N'NonBudgeted')
    INSERT INTO [HR_RequisitionNature] ([Code], [Name]) VALUES (N'NonBudgeted', N'Non-Budgeted');
");

            // ── 4. HR_RequisitionSkill ───────────────────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RequisitionSkill]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RequisitionSkill]
    (
        [Id]              NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionSkill] PRIMARY KEY,
        [RequisitionId]   NUMERIC(18,0) NOT NULL,
        [SkillName]       NVARCHAR(200) NOT NULL,
        [YearsExperience] DECIMAL(5,2)  NULL,
        [IsMandatory]     BIT           NOT NULL CONSTRAINT [DF_HR_RequisitionSkill_IsMandatory] DEFAULT(1),
        CONSTRAINT [FK_HR_RequisitionSkill_HR_Requisition]
            FOREIGN KEY ([RequisitionId]) REFERENCES [HR_Requisition]([Id])
    );
END;
");

            // ── 5. HR_RequisitionOffering ────────────────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RequisitionOffering]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RequisitionOffering]
    (
        [Id]            NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RequisitionOffering] PRIMARY KEY,
        [RequisitionId] NUMERIC(18,0) NOT NULL,
        [OfferingName]  NVARCHAR(200) NOT NULL,
        [Description]   NVARCHAR(500) NULL,
        CONSTRAINT [FK_HR_RequisitionOffering_HR_Requisition]
            FOREIGN KEY ([RequisitionId]) REFERENCES [HR_Requisition]([Id])
    );
END;
");

            // ── 6. HR_RecruitmentUnit ────────────────────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RecruitmentUnit]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RecruitmentUnit]
    (
        [Id]                         NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RecruitmentUnit] PRIMARY KEY,
        [UnitName]                   NVARCHAR(200) NOT NULL,
        [RecruitmentHeadEmployeeId]  NUMERIC(18,0) NOT NULL,
        [IsActive]                   BIT           NOT NULL CONSTRAINT [DF_HR_RecruitmentUnit_IsActive] DEFAULT(1)
    );
END;
");

            // ── 7. HR_RecruitmentUnitDepartment ─────────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RecruitmentUnitDepartment]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RecruitmentUnitDepartment]
    (
        [Id]                NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RecruitmentUnitDepartment] PRIMARY KEY,
        [RecruitmentUnitId] NUMERIC(18,0) NOT NULL,
        [DepartmentId]      NUMERIC(18,0) NOT NULL,
        CONSTRAINT [FK_HR_RecruitmentUnitDepartment_HR_RecruitmentUnit]
            FOREIGN KEY ([RecruitmentUnitId]) REFERENCES [HR_RecruitmentUnit]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_RecruitmentUnitDepartment_Unit_Dept'
               AND [object_id] = OBJECT_ID(N'[HR_RecruitmentUnitDepartment]'))
BEGIN
    CREATE UNIQUE INDEX [IX_HR_RecruitmentUnitDepartment_Unit_Dept]
    ON [HR_RecruitmentUnitDepartment] ([RecruitmentUnitId], [DepartmentId]);
END;
");

            // ── 8. HR_RecruitmentTeam ────────────────────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RecruitmentTeam]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_RecruitmentTeam]
    (
        [Id]                   NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_RecruitmentTeam] PRIMARY KEY,
        [RecruitmentUnitId]    NUMERIC(18,0) NOT NULL,
        [RecruiterEmployeeId]  NUMERIC(18,0) NOT NULL,
        [IsActive]             BIT           NOT NULL CONSTRAINT [DF_HR_RecruitmentTeam_IsActive] DEFAULT(1),
        CONSTRAINT [FK_HR_RecruitmentTeam_HR_RecruitmentUnit]
            FOREIGN KEY ([RecruitmentUnitId]) REFERENCES [HR_RecruitmentUnit]([Id])
    );
END;
");

            // ── 9. HR_RequisitionAssignment: add AssignedByEmployeeId ────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RequisitionAssignment]', N'U') IS NOT NULL
    AND COL_LENGTH(N'HR_RequisitionAssignment', N'AssignedByEmployeeId') IS NULL
BEGIN
    ALTER TABLE [HR_RequisitionAssignment] ADD [AssignedByEmployeeId] NUMERIC(18,0) NULL;
END;
");

            // ── 10. Update workflow: HRBP (1) → HOD (2) → RecruitmentHead (3, final)
            // Cannot DELETE steps that existing HR_WorkflowInstanceStep rows reference (FK).
            // UPDATE in place instead — preserves FK integrity.
            migrationBuilder.Sql(@"
DECLARE @DefId NUMERIC(18,0);
SELECT @DefId = [Id] FROM [HR_WorkflowDefinition]
WHERE [WorkflowName] = N'Recruitment Requisition Approval';

IF @DefId IS NOT NULL
BEGIN
    -- Step 1: ensure HRBP
    IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 1)
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId],[StepOrder],[StepName],[ApproverType],[IsFinalStep],[CreatedBy],[CreatedOn])
        VALUES (@DefId, 1, N'HRBP Review', N'HRBP', 0, N'Migration', SYSDATETIME());
    ELSE
        UPDATE [HR_WorkflowStep]
        SET [StepName] = N'HRBP Review', [ApproverType] = N'HRBP', [IsFinalStep] = 0
        WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 1;

    -- Step 2: ensure HOD
    IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 2)
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId],[StepOrder],[StepName],[ApproverType],[IsFinalStep],[CreatedBy],[CreatedOn])
        VALUES (@DefId, 2, N'HOD Approval', N'HOD', 0, N'Migration', SYSDATETIME());
    ELSE
        UPDATE [HR_WorkflowStep]
        SET [StepName] = N'HOD Approval', [ApproverType] = N'HOD', [IsFinalStep] = 0
        WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 2;

    -- Step 3: UPDATE old Recruiter step → RecruitmentHead (keeps FK refs intact)
    IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 3)
        INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId],[StepOrder],[StepName],[ApproverType],[IsFinalStep],[CreatedBy],[CreatedOn])
        VALUES (@DefId, 3, N'Recruitment Head Review', N'RecruitmentHead', 1, N'Migration', SYSDATETIME());
    ELSE
        UPDATE [HR_WorkflowStep]
        SET [StepName] = N'Recruitment Head Review', [ApproverType] = N'RecruitmentHead', [IsFinalStep] = 1
        WHERE [WorkflowDefinitionId] = @DefId AND [StepOrder] = 3;

    -- Patch any existing WorkflowInstanceStep rows that still carry the old ApproverType
    UPDATE [HR_WorkflowInstanceStep]
    SET [ApproverType] = N'RecruitmentHead'
    WHERE [ApproverType] = N'Recruiter'
      AND [WorkflowInstanceId] IN (
          SELECT [Id] FROM [HR_WorkflowInstance] WHERE [EntityType] = N'Requisition'
      );
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_RecruitmentTeam]', N'U') IS NOT NULL DROP TABLE [HR_RecruitmentTeam];
IF OBJECT_ID(N'[HR_RecruitmentUnitDepartment]', N'U') IS NOT NULL DROP TABLE [HR_RecruitmentUnitDepartment];
IF OBJECT_ID(N'[HR_RecruitmentUnit]', N'U') IS NOT NULL DROP TABLE [HR_RecruitmentUnit];
IF OBJECT_ID(N'[HR_RequisitionOffering]', N'U') IS NOT NULL DROP TABLE [HR_RequisitionOffering];
IF OBJECT_ID(N'[HR_RequisitionSkill]', N'U') IS NOT NULL DROP TABLE [HR_RequisitionSkill];
IF OBJECT_ID(N'[HR_RequisitionNature]', N'U') IS NOT NULL DROP TABLE [HR_RequisitionNature];
IF OBJECT_ID(N'[HR_RequisitionType]', N'U') IS NOT NULL DROP TABLE [HR_RequisitionType];
");
        }
    }
}
