using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTemplateSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepLabel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsFinalStep = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ApprovalTemplateSteps_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocApprovalAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DmsDefinitionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApprovalTemplateId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocApprovalAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocApprovalAssignments_ApprovalTemplates_ApprovalTemplateId",
                        column: x => x.ApprovalTemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalAssignments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocApprovalInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentID = table.Column<long>(type: "bigint", nullable: false),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    CurrentStepOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocApprovalInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstances_DocApprovalAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "DocApprovalAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstances_Documents_DocumentID",
                        column: x => x.DocumentID,
                        principalTable: "Documents",
                        principalColumn: "DocumentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocApprovalInstanceSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<long>(type: "bigint", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    ApproverId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocApprovalInstanceSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstanceSteps_DocApprovalInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "DocApprovalInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocApprovalInstanceSteps_users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateStep_Template_Order",
                table: "ApprovalTemplateSteps",
                columns: new[] { "TemplateId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateSteps_RoleId",
                table: "ApprovalTemplateSteps",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTemplateSteps_UserId",
                table: "ApprovalTemplateSteps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocApprovalAssignments_ApprovalTemplateId",
                table: "DocApprovalAssignments",
                column: "ApprovalTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DocApprovalAssignments_UserId",
                table: "DocApprovalAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocApprovalInstance_Document_Unique",
                table: "DocApprovalInstances",
                column: "DocumentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocApprovalInstances_AssignmentId",
                table: "DocApprovalInstances",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocApprovalInstanceSteps_ApproverId",
                table: "DocApprovalInstanceSteps",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_DocApprovalInstanceSteps_InstanceId",
                table: "DocApprovalInstanceSteps",
                column: "InstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalTemplateSteps");

            migrationBuilder.DropTable(
                name: "DocApprovalInstanceSteps");

            migrationBuilder.DropTable(
                name: "DocApprovalInstances");

            migrationBuilder.DropTable(
                name: "DocApprovalAssignments");

            migrationBuilder.DropTable(
                name: "ApprovalTemplates");
        }
    }
}
