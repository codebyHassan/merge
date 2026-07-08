using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605010000_AddHrRequisitionWorkflow")]
    public partial class AddHrRequisitionWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'HR_Requisition', N'WorkflowInstanceFK') IS NULL
BEGIN
    ALTER TABLE [HR_Requisition]
    ADD [WorkflowInstanceFK] NUMERIC(18,0) NULL;
END;

IF OBJECT_ID(N'[HR_WorkflowDefinition]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_WorkflowDefinition]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowDefinition] PRIMARY KEY,
        [WorkflowName] NVARCHAR(100) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_HR_WorkflowDefinition_IsActive] DEFAULT(1),
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedOn] DATETIME2 NULL,
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedOn] DATETIME2 NULL
    );
END;

IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_WorkflowStep]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowStep] PRIMARY KEY,
        [WorkflowDefinitionId] NUMERIC(18,0) NOT NULL,
        [StepOrder] INT NOT NULL,
        [StepName] NVARCHAR(100) NOT NULL,
        [RoleId] NUMERIC(18,0) NOT NULL,
        [IsFinalStep] BIT NOT NULL CONSTRAINT [DF_HR_WorkflowStep_IsFinalStep] DEFAULT(0),
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedOn] DATETIME2 NULL,
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedOn] DATETIME2 NULL,
        CONSTRAINT [FK_HR_WorkflowStep_HR_WorkflowDefinition_WorkflowDefinitionId]
            FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [HR_WorkflowDefinition]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowStep_Definition_Order' AND [object_id] = OBJECT_ID(N'[HR_WorkflowStep]'))
BEGIN
    CREATE UNIQUE INDEX [IX_HR_WorkflowStep_Definition_Order]
    ON [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder]);
END;

IF OBJECT_ID(N'[HR_WorkflowInstance]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_WorkflowInstance]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowInstance] PRIMARY KEY,
        [WorkflowDefinitionId] NUMERIC(18,0) NOT NULL,
        [EntityType] NVARCHAR(50) NOT NULL,
        [EntityId] NUMERIC(18,0) NOT NULL,
        [CurrentStepId] NUMERIC(18,0) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [StartedDate] DATETIME2 NOT NULL,
        [CompletedDate] DATETIME2 NULL,
        CONSTRAINT [FK_HR_WorkflowInstance_HR_WorkflowDefinition_WorkflowDefinitionId]
            FOREIGN KEY ([WorkflowDefinitionId]) REFERENCES [HR_WorkflowDefinition]([Id]),
        CONSTRAINT [FK_HR_WorkflowInstance_HR_WorkflowStep_CurrentStepId]
            FOREIGN KEY ([CurrentStepId]) REFERENCES [HR_WorkflowStep]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowInstance_Entity' AND [object_id] = OBJECT_ID(N'[HR_WorkflowInstance]'))
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstance_Entity]
    ON [HR_WorkflowInstance] ([EntityType], [EntityId]);
END;

IF OBJECT_ID(N'[HR_WorkflowHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_WorkflowHistory]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowHistory] PRIMARY KEY,
        [WorkflowInstanceFK] NUMERIC(18,0) NOT NULL,
        [WorkflowStepFK] NUMERIC(18,0) NOT NULL,
        [ActionBy] NUMERIC(18,0) NOT NULL,
        [ActionType] NVARCHAR(50) NOT NULL,
        [Comments] NVARCHAR(1000) NULL,
        [ActionDate] DATETIME2 NOT NULL,
        CONSTRAINT [FK_HR_WorkflowHistory_HR_WorkflowInstance_WorkflowInstanceFK]
            FOREIGN KEY ([WorkflowInstanceFK]) REFERENCES [HR_WorkflowInstance]([Id]),
        CONSTRAINT [FK_HR_WorkflowHistory_HR_WorkflowStep_WorkflowStepFK]
            FOREIGN KEY ([WorkflowStepFK]) REFERENCES [HR_WorkflowStep]([Id])
    );
END;

DECLARE @WorkflowDefinitionId NUMERIC(18,0);

SELECT @WorkflowDefinitionId = [Id]
FROM [HR_WorkflowDefinition]
WHERE [WorkflowName] = N'Recruitment Requisition Approval';

IF @WorkflowDefinitionId IS NULL
BEGIN
    INSERT INTO [HR_WorkflowDefinition] ([WorkflowName], [IsActive], [CreatedBy], [CreatedOn])
    VALUES (N'Recruitment Requisition Approval', 1, N'Migration', SYSDATETIME());

    SET @WorkflowDefinitionId = SCOPE_IDENTITY();
END;

IF EXISTS (SELECT 1 FROM [roles] WHERE [Name] = N'HRBP')
   AND NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 1)
BEGIN
    INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [RoleId], [IsFinalStep], [CreatedBy], [CreatedOn])
    SELECT @WorkflowDefinitionId, 1, N'HRBP Review', CAST([Id] AS NUMERIC(18,0)), 0, N'Migration', SYSDATETIME()
    FROM [roles]
    WHERE [Name] = N'HRBP';
END;

IF EXISTS (SELECT 1 FROM [roles] WHERE [Name] = N'HOD')
   AND NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 2)
BEGIN
    INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [RoleId], [IsFinalStep], [CreatedBy], [CreatedOn])
    SELECT @WorkflowDefinitionId, 2, N'HOD Approval', CAST([Id] AS NUMERIC(18,0)), 0, N'Migration', SYSDATETIME()
    FROM [roles]
    WHERE [Name] = N'HOD';
END;

IF EXISTS (SELECT 1 FROM [roles] WHERE [Name] = N'Recruiter')
   AND NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 3)
BEGIN
    INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [RoleId], [IsFinalStep], [CreatedBy], [CreatedOn])
    SELECT @WorkflowDefinitionId, 3, N'Recruiter Assignment', CAST([Id] AS NUMERIC(18,0)), 1, N'Migration', SYSDATETIME()
    FROM [roles]
    WHERE [Name] = N'Recruiter';
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_WorkflowHistory]', N'U') IS NOT NULL DROP TABLE [HR_WorkflowHistory];
IF OBJECT_ID(N'[HR_WorkflowInstance]', N'U') IS NOT NULL DROP TABLE [HR_WorkflowInstance];
IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL DROP TABLE [HR_WorkflowStep];
IF OBJECT_ID(N'[HR_WorkflowDefinition]', N'U') IS NOT NULL DROP TABLE [HR_WorkflowDefinition];
IF COL_LENGTH(N'HR_Requisition', N'WorkflowInstanceFK') IS NOT NULL
BEGIN
    ALTER TABLE [HR_Requisition] DROP COLUMN [WorkflowInstanceFK];
END;
");
        }
    }
}
