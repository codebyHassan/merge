using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260606000000_RefactorHrRequisitionWorkflowApprovers")]
    public partial class RefactorHrRequisitionWorkflowApprovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_DepartmentApprover]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_DepartmentApprover]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_DepartmentApprover] PRIMARY KEY,
        [DepartmentId] NUMERIC(18,0) NOT NULL,
        [ApproverType] NVARCHAR(50) NOT NULL,
        [EmployeeId] NUMERIC(18,0) NOT NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_HR_DepartmentApprover_IsActive] DEFAULT(1)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_DepartmentApprover_Department_Type_Active' AND [object_id] = OBJECT_ID(N'[HR_DepartmentApprover]'))
BEGIN
    CREATE INDEX [IX_HR_DepartmentApprover_Department_Type_Active]
    ON [HR_DepartmentApprover] ([DepartmentId], [ApproverType], [IsActive]);
END;

IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL AND COL_LENGTH(N'HR_WorkflowStep', N'ApproverType') IS NULL
BEGIN
    ALTER TABLE [HR_WorkflowStep] ADD [ApproverType] NVARCHAR(50) NULL;
END;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL
BEGIN
    UPDATE [HR_WorkflowStep]
    SET [ApproverType] = CASE
        WHEN [StepOrder] = 1 THEN N'HRBP'
        WHEN [StepOrder] = 2 THEN N'HOD'
        WHEN [StepOrder] = 3 THEN N'Recruiter'
        ELSE REPLACE(REPLACE([StepName], N' ', N''), N'Approval', N'Approver')
    END
    WHERE [ApproverType] IS NULL OR LTRIM(RTRIM([ApproverType])) = N'';

    IF EXISTS (SELECT 1 FROM sys.columns WHERE [object_id] = OBJECT_ID(N'[HR_WorkflowStep]') AND [name] = N'ApproverType' AND [is_nullable] = 1)
    BEGIN
        ALTER TABLE [HR_WorkflowStep] ALTER COLUMN [ApproverType] NVARCHAR(50) NOT NULL;
    END;
END;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'HR_WorkflowStep', N'RoleId') IS NOT NULL
    BEGIN
        ALTER TABLE [HR_WorkflowStep] DROP COLUMN [RoleId];
    END;
END;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_WorkflowInstanceStep]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_WorkflowInstanceStep]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowInstanceStep] PRIMARY KEY,
        [WorkflowInstanceId] NUMERIC(18,0) NOT NULL,
        [WorkflowStepId] NUMERIC(18,0) NOT NULL,
        [StepOrder] INT NOT NULL,
        [ApproverType] NVARCHAR(50) NOT NULL,
        [EmployeeId] NUMERIC(18,0) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [AssignedDate] DATETIME2 NOT NULL,
        [ActionDate] DATETIME2 NULL,
        [Comments] NVARCHAR(1000) NULL,
        CONSTRAINT [FK_HR_WorkflowInstanceStep_HR_WorkflowInstance_WorkflowInstanceId]
            FOREIGN KEY ([WorkflowInstanceId]) REFERENCES [HR_WorkflowInstance]([Id]),
        CONSTRAINT [FK_HR_WorkflowInstanceStep_HR_WorkflowStep_WorkflowStepId]
            FOREIGN KEY ([WorkflowStepId]) REFERENCES [HR_WorkflowStep]([Id])
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowInstanceStep_Instance_Order' AND [object_id] = OBJECT_ID(N'[HR_WorkflowInstanceStep]'))
BEGIN
    CREATE UNIQUE INDEX [IX_HR_WorkflowInstanceStep_Instance_Order]
    ON [HR_WorkflowInstanceStep] ([WorkflowInstanceId], [StepOrder]);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_WorkflowInstanceStep_Employee_Status' AND [object_id] = OBJECT_ID(N'[HR_WorkflowInstanceStep]'))
BEGIN
    CREATE INDEX [IX_HR_WorkflowInstanceStep_Employee_Status]
    ON [HR_WorkflowInstanceStep] ([EmployeeId], [Status]);
END;

IF OBJECT_ID(N'[HR_WorkflowAction]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_WorkflowAction]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_WorkflowAction] PRIMARY KEY,
        [WorkflowInstanceStepId] NUMERIC(18,0) NOT NULL,
        [ActionByEmployeeId] NUMERIC(18,0) NOT NULL,
        [ActionType] NVARCHAR(50) NOT NULL,
        [Comments] NVARCHAR(1000) NULL,
        [ActionDate] DATETIME2 NOT NULL,
        CONSTRAINT [FK_HR_WorkflowAction_HR_WorkflowInstanceStep_WorkflowInstanceStepId]
            FOREIGN KEY ([WorkflowInstanceStepId]) REFERENCES [HR_WorkflowInstanceStep]([Id])
    );
END;
");

            migrationBuilder.Sql(@"
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
");

            migrationBuilder.Sql(@"
DECLARE @WorkflowDefinitionId NUMERIC(18,0);

SELECT @WorkflowDefinitionId = [Id]
FROM [HR_WorkflowDefinition]
WHERE [WorkflowName] = N'Recruitment Requisition Approval';

IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 1)
BEGIN
    INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [ApproverType], [IsFinalStep], [CreatedBy], [CreatedOn])
    VALUES (@WorkflowDefinitionId, 1, N'HRBP Review', N'HRBP', 0, N'Migration', SYSDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 2)
BEGIN
    INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [ApproverType], [IsFinalStep], [CreatedBy], [CreatedOn])
    VALUES (@WorkflowDefinitionId, 2, N'HOD Approval', N'HOD', 0, N'Migration', SYSDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM [HR_WorkflowStep] WHERE [WorkflowDefinitionId] = @WorkflowDefinitionId AND [StepOrder] = 3)
BEGIN
    INSERT INTO [HR_WorkflowStep] ([WorkflowDefinitionId], [StepOrder], [StepName], [ApproverType], [IsFinalStep], [CreatedBy], [CreatedOn])
    VALUES (@WorkflowDefinitionId, 3, N'Recruiter Assignment', N'Recruiter', 1, N'Migration', SYSDATETIME());
END;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_WorkflowHistory]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [HR_WorkflowHistory];
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_WorkflowAction]', N'U') IS NOT NULL DROP TABLE [HR_WorkflowAction];
IF OBJECT_ID(N'[HR_WorkflowInstanceStep]', N'U') IS NOT NULL DROP TABLE [HR_WorkflowInstanceStep];
IF OBJECT_ID(N'[HR_DepartmentApprover]', N'U') IS NOT NULL DROP TABLE [HR_DepartmentApprover];

IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL AND COL_LENGTH(N'HR_WorkflowStep', N'RoleId') IS NULL
BEGIN
    ALTER TABLE [HR_WorkflowStep] ADD [RoleId] NUMERIC(18,0) NOT NULL CONSTRAINT [DF_HR_WorkflowStep_RoleId] DEFAULT(0);
END;

IF OBJECT_ID(N'[HR_WorkflowStep]', N'U') IS NOT NULL AND COL_LENGTH(N'HR_WorkflowStep', N'ApproverType') IS NOT NULL
BEGIN
    ALTER TABLE [HR_WorkflowStep] DROP COLUMN [ApproverType];
END;
");
        }
    }
}
