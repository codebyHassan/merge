using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApprovalSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old approval tables in FK order — safe even if they don't exist
            migrationBuilder.Sql(@"
                IF OBJECT_ID('WorkflowConditions','U') IS NOT NULL DROP TABLE [WorkflowConditions];
                IF OBJECT_ID('DocApprovalInstanceSteps','U') IS NOT NULL DROP TABLE [DocApprovalInstanceSteps];
                IF OBJECT_ID('DocApprovalInstances','U') IS NOT NULL DROP TABLE [DocApprovalInstances];
                IF OBJECT_ID('ApprovalTemplateSteps','U') IS NOT NULL DROP TABLE [ApprovalTemplateSteps];
                IF OBJECT_ID('DocApprovalAssignments','U') IS NOT NULL DROP TABLE [DocApprovalAssignments];
                IF OBJECT_ID('ApprovalTemplates','U') IS NOT NULL DROP TABLE [ApprovalTemplates];
            ");

            // Drop stale column on users if it exists
            migrationBuilder.Sql(@"
                IF COL_LENGTH('users','Self') IS NOT NULL
                    ALTER TABLE [users] DROP COLUMN [Self];
            ");

            // ApprovalTemplates
            migrationBuilder.CreateTable(
                name: "ApprovalTemplates",
                columns: table => new
                {
                    Id           = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description  = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive     = table.Column<bool>(type: "bit", nullable: false),
                    Version      = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    TypeId       = table.Column<int>(type: "int", nullable: false),
                    FlowType     = table.Column<int>(type: "int", nullable: false),
                    createdby    = table.Column<int>(type: "int", nullable: false),
                    createat     = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby    = table.Column<int>(type: "int", nullable: false),
                    updateat     = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplates_DocumentTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "DocumentTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            // ApprovalTemplateSteps
            migrationBuilder.CreateTable(
                name: "ApprovalTemplateSteps",
                columns: table => new
                {
                    Id           = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId   = table.Column<int>(type: "int", nullable: false),
                    StepNo       = table.Column<int>(type: "int", nullable: false),
                    RoleId       = table.Column<int>(type: "int", nullable: false),
                    ApprovalType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsOptional   = table.Column<bool>(type: "bit", nullable: false),
                    StepLabel    = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    createdby    = table.Column<int>(type: "int", nullable: false),
                    createat     = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplateSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateSteps_ApprovalTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateSteps_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // WorkflowConditions
            migrationBuilder.CreateTable(
                name: "WorkflowConditions",
                columns: table => new
                {
                    Id             = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowId     = table.Column<int>(type: "int", nullable: false),
                    FieldName      = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Operator       = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value          = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThenWorkflowId = table.Column<int>(type: "int", nullable: true),
                    createdby      = table.Column<int>(type: "int", nullable: false),
                    createat       = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowConditions_ApprovalTemplates_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowConditions_ApprovalTemplates_ThenWorkflowId",
                        column: x => x.ThenWorkflowId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // DocApprovalAssignments
            migrationBuilder.CreateTable(
                name: "DocApprovalAssignments",
                columns: table => new
                {
                    Id                 = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    UserId             = table.Column<int>(type: "int", nullable: false),
                    DmsDefinitionName  = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApprovalTemplateId = table.Column<int>(type: "int", nullable: false),
                    Notes              = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive           = table.Column<bool>(type: "bit", nullable: false),
                    createdby          = table.Column<int>(type: "int", nullable: false),
                    createat           = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby          = table.Column<int>(type: "int", nullable: false),
                    updateat           = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocApprovalAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocApprovalAssignments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalAssignments_ApprovalTemplates_ApprovalTemplateId",
                        column: x => x.ApprovalTemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // DocApprovalInstances
            migrationBuilder.CreateTable(
                name: "DocApprovalInstances",
                columns: table => new
                {
                    Id          = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    DocumentID  = table.Column<long>(type: "bigint", nullable: false),
                    WorkflowId  = table.Column<int>(type: "int", nullable: false),
                    CurrentStep = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Status      = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt   = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocApprovalInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstances_Documents_DocumentID",
                        column: x => x.DocumentID,
                        principalTable: "Documents",
                        principalColumn: "DocumentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstances_ApprovalTemplates_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstances_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // DocApprovalInstanceSteps
            migrationBuilder.CreateTable(
                name: "DocApprovalInstanceSteps",
                columns: table => new
                {
                    Id             = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId     = table.Column<long>(type: "bigint", nullable: false),
                    StepNo         = table.Column<int>(type: "int", nullable: false),
                    TemplateStepId = table.Column<int>(type: "int", nullable: true),
                    Action         = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Comments       = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionAt       = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverId     = table.Column<int>(type: "int", nullable: true),
                    DepartmentId   = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocApprovalInstanceSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstanceSteps_DocApprovalInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "DocApprovalInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstanceSteps_ApprovalTemplateSteps_TemplateStepId",
                        column: x => x.TemplateStepId,
                        principalTable: "ApprovalTemplateSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstanceSteps_users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstanceSteps_department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Indexes
            migrationBuilder.CreateIndex("IX_ApprovalTemplates_TypeId", "ApprovalTemplates", "TypeId");
            migrationBuilder.CreateIndex("IX_ApprovalTemplateStep_Template_StepNo", "ApprovalTemplateSteps", new[] { "TemplateId", "StepNo" }, unique: true);
            migrationBuilder.CreateIndex("IX_ApprovalTemplateSteps_RoleId", "ApprovalTemplateSteps", "RoleId");
            migrationBuilder.CreateIndex("IX_WorkflowConditions_WorkflowId", "WorkflowConditions", "WorkflowId");
            migrationBuilder.CreateIndex("IX_WorkflowConditions_ThenWorkflowId", "WorkflowConditions", "ThenWorkflowId");
            migrationBuilder.CreateIndex("IX_DocApprovalAssignments_UserId", "DocApprovalAssignments", "UserId");
            migrationBuilder.CreateIndex("IX_DocApprovalAssignments_ApprovalTemplateId", "DocApprovalAssignments", "ApprovalTemplateId");
            migrationBuilder.CreateIndex("IX_DocApprovalInstance_Document_Status", "DocApprovalInstances", new[] { "DocumentID", "Status" });
            migrationBuilder.CreateIndex("IX_DocApprovalInstances_WorkflowId", "DocApprovalInstances", "WorkflowId");
            migrationBuilder.CreateIndex("IX_DocApprovalInstances_CreatedById", "DocApprovalInstances", "CreatedById");
            migrationBuilder.CreateIndex("IX_DocApprovalInstanceSteps_InstanceId", "DocApprovalInstanceSteps", "InstanceId");
            migrationBuilder.CreateIndex("IX_DocApprovalInstanceSteps_TemplateStepId", "DocApprovalInstanceSteps", "TemplateStepId");
            migrationBuilder.CreateIndex("IX_DocApprovalInstanceSteps_ApproverId", "DocApprovalInstanceSteps", "ApproverId");
            migrationBuilder.CreateIndex("IX_DocApprovalInstanceSteps_DepartmentId", "DocApprovalInstanceSteps", "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('WorkflowConditions','U') IS NOT NULL DROP TABLE [WorkflowConditions];
                IF OBJECT_ID('DocApprovalInstanceSteps','U') IS NOT NULL DROP TABLE [DocApprovalInstanceSteps];
                IF OBJECT_ID('DocApprovalInstances','U') IS NOT NULL DROP TABLE [DocApprovalInstances];
                IF OBJECT_ID('ApprovalTemplateSteps','U') IS NOT NULL DROP TABLE [ApprovalTemplateSteps];
                IF OBJECT_ID('DocApprovalAssignments','U') IS NOT NULL DROP TABLE [DocApprovalAssignments];
                IF OBJECT_ID('ApprovalTemplates','U') IS NOT NULL DROP TABLE [ApprovalTemplates];
            ");
        }
    }
}
