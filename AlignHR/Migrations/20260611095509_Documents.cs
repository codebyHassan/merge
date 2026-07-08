using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class Documents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeavePolicies_EmployeeId",
                table: "EmployeeLeavePolicies");

            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "HR_ApplicationStage",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_ApplicationStage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_Candidate",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TotalExperienceYears = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CurrentEmployer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentDesignation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LinkedInProfile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_Candidate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_DepartmentApprover",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ApproverType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_DepartmentApprover", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_EvaluationCriteria",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobPostingId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    CriteriaName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_EvaluationCriteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_InterviewRound",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobPostingId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    RoundName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoundOrder = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_InterviewRound", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_JobPosting",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    JobCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostingStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OpenDate = table.Column<DateTime>(type: "date", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_JobPosting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_OnboardingTaskTemplate",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ResponsibleDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OnboardingTaskTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_PostingChannel",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_PostingChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_Requisition",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmployeeFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DepartmentFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    PositionTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VacancyCount = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RequiredByDate = table.Column<DateTime>(type: "date", nullable: true),
                    WorkflowInstanceFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_Requisition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_RequisitionAssignment",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    RecruiterEmployeeFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_RequisitionAssignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HR_WorkflowDefinition",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_WorkflowDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    HasExpiry = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.DocumentTypeID);
                    table.ForeignKey(
                        name: "FK_DocumentTypes_DocumentCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "DocumentCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_CandidateDocument",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_CandidateDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_CandidateDocument_HR_Candidate_CandidateFk",
                        column: x => x.CandidateFk,
                        principalTable: "HR_Candidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_JobApplication",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    JobPostingFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    CurrentStageFK = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecruiterNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_JobApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_JobApplication_HR_ApplicationStage_CurrentStageFK",
                        column: x => x.CurrentStageFK,
                        principalTable: "HR_ApplicationStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_JobApplication_HR_Candidate_CandidateFK",
                        column: x => x.CandidateFK,
                        principalTable: "HR_Candidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_JobApplication_HR_JobPosting_JobPostingFK",
                        column: x => x.JobPostingFK,
                        principalTable: "HR_JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_JobPostingChannel",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobPostingFK = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    PostingChannelFK = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_JobPostingChannel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_JobPostingChannel_HR_JobPosting_JobPostingFK",
                        column: x => x.JobPostingFK,
                        principalTable: "HR_JobPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_JobPostingChannel_HR_PostingChannel_PostingChannelFK",
                        column: x => x.PostingChannelFK,
                        principalTable: "HR_PostingChannel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_WorkflowStep",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefinitionId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApproverType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsFinalStep = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_WorkflowStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_WorkflowStep_HR_WorkflowDefinition_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "HR_WorkflowDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsConfidential = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "DocumentCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentTypes_DocumentTypeID",
                        column: x => x.DocumentTypeID,
                        principalTable: "DocumentTypes",
                        principalColumn: "DocumentTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_emp_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_ApplicationStageHistory",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobApplicationFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    FromStageId = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ToStageFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ChangedBy = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_ApplicationStageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_ApplicationStageHistory_HR_ApplicationStage_FromStageId",
                        column: x => x.FromStageId,
                        principalTable: "HR_ApplicationStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_ApplicationStageHistory_HR_ApplicationStage_ToStageFk",
                        column: x => x.ToStageFk,
                        principalTable: "HR_ApplicationStage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_ApplicationStageHistory_HR_JobApplication_JobApplicationFk",
                        column: x => x.JobApplicationFk,
                        principalTable: "HR_JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_HiringDecision",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobApplicationFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Decision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DecisionBy = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_HiringDecision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_HiringDecision_HR_JobApplication_JobApplicationFk",
                        column: x => x.JobApplicationFk,
                        principalTable: "HR_JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_InterviewSchedule",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobApplicationId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    InterviewRoundId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingLink = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_InterviewSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_InterviewSchedule_HR_InterviewRound_InterviewRoundId",
                        column: x => x.InterviewRoundId,
                        principalTable: "HR_InterviewRound",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_InterviewSchedule_HR_JobApplication_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "HR_JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_Offer",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobApplicationFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    OfferNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProposedSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProposedJoiningDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_Offer_HR_JobApplication_JobApplicationFk",
                        column: x => x.JobApplicationFk,
                        principalTable: "HR_JobApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_WorkflowInstance",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefinitionId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    CurrentStepId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_WorkflowInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_WorkflowInstance_HR_WorkflowDefinition_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "HR_WorkflowDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_WorkflowInstance_HR_WorkflowStep_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "HR_WorkflowStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_InterviewFeedback",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewScheduleFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    InterviewerEmployeeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Concerns = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_InterviewFeedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_InterviewFeedback_HR_InterviewSchedule_InterviewScheduleFk",
                        column: x => x.InterviewScheduleFk,
                        principalTable: "HR_InterviewSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_InterviewPanel",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewScheduleId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    EmployeeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_InterviewPanel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_InterviewPanel_HR_InterviewSchedule_InterviewScheduleId",
                        column: x => x.InterviewScheduleId,
                        principalTable: "HR_InterviewSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_OfferApproval",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ApproverEmployeeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OfferApproval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OfferApproval_HR_Offer_OfferFk",
                        column: x => x.OfferFk,
                        principalTable: "HR_Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_OfferDocument",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OfferDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OfferDocument_HR_Offer_OfferFk",
                        column: x => x.OfferFk,
                        principalTable: "HR_Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_OfferResponse",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ResponseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OfferResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OfferResponse_HR_Offer_OfferFk",
                        column: x => x.OfferFk,
                        principalTable: "HR_Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_OfferVersion",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    VersionNo = table.Column<int>(type: "int", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OfferVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OfferVersion_HR_Offer_OfferFk",
                        column: x => x.OfferFk,
                        principalTable: "HR_Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_Onboarding",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    OfferId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    PlannedJoiningDate = table.Column<DateTime>(type: "date", nullable: false),
                    ActualJoiningDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_Onboarding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_Onboarding_HR_Candidate_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "HR_Candidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_Onboarding_HR_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "HR_Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_WorkflowInstanceStep",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstanceId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    WorkflowStepId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    ApproverType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_WorkflowInstanceStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_WorkflowInstanceStep_HR_WorkflowInstance_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "HR_WorkflowInstance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_WorkflowInstanceStep_HR_WorkflowStep_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "HR_WorkflowStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_EvaluationScore",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewFeedbackFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    EvaluationCriteriaFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_EvaluationScore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_EvaluationScore_HR_EvaluationCriteria_EvaluationCriteriaFk",
                        column: x => x.EvaluationCriteriaFk,
                        principalTable: "HR_EvaluationCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_EvaluationScore_HR_InterviewFeedback_InterviewFeedbackFk",
                        column: x => x.InterviewFeedbackFk,
                        principalTable: "HR_InterviewFeedback",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_JoiningConfirmation",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnboardingId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "date", nullable: false),
                    ConfirmedByEmployeeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_JoiningConfirmation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_JoiningConfirmation_HR_Onboarding_OnboardingId",
                        column: x => x.OnboardingId,
                        principalTable: "HR_Onboarding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_OnboardingDocument",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnboardingFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OnboardingDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OnboardingDocument_HR_Onboarding_OnboardingFk",
                        column: x => x.OnboardingFk,
                        principalTable: "HR_Onboarding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_OnboardingTask",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnboardingFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TaskTemplateFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    AssignedToEmployeeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OnboardingTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OnboardingTask_HR_OnboardingTaskTemplate_TaskTemplateFk",
                        column: x => x.TaskTemplateFk,
                        principalTable: "HR_OnboardingTaskTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_OnboardingTask_HR_Onboarding_OnboardingFk",
                        column: x => x.OnboardingFk,
                        principalTable: "HR_Onboarding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HR_WorkflowAction",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowInstanceStepId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ActionByEmployeeId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_WorkflowAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_WorkflowAction_HR_WorkflowInstanceStep_WorkflowInstanceStepId",
                        column: x => x.WorkflowInstanceStepId,
                        principalTable: "HR_WorkflowInstanceStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeavePolicies_Employee_Policy_Unique",
                table: "EmployeeLeavePolicies",
                columns: new[] { "EmployeeId", "LeavePolicyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CategoryID",
                table: "Documents",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentNo_Unique",
                table: "Documents",
                column: "DocumentNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeID",
                table: "Documents",
                column: "DocumentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EmployeeID",
                table: "Documents",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UploadedBy",
                table: "Documents",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_CategoryID",
                table: "DocumentTypes",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_HR_ApplicationStageHistory_FromStageId",
                table: "HR_ApplicationStageHistory",
                column: "FromStageId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_ApplicationStageHistory_JobApplicationFk",
                table: "HR_ApplicationStageHistory",
                column: "JobApplicationFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_ApplicationStageHistory_ToStageFk",
                table: "HR_ApplicationStageHistory",
                column: "ToStageFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Candidate_Email_Unique",
                table: "HR_Candidate",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_HR_CandidateDocument_CandidateFk",
                table: "HR_CandidateDocument",
                column: "CandidateFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_DepartmentApprover_Department_Type_Active",
                table: "HR_DepartmentApprover",
                columns: new[] { "DepartmentId", "ApproverType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_HR_EvaluationScore_EvaluationCriteriaFk",
                table: "HR_EvaluationScore",
                column: "EvaluationCriteriaFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_EvaluationScore_Feedback_Criteria",
                table: "HR_EvaluationScore",
                columns: new[] { "InterviewFeedbackFk", "EvaluationCriteriaFk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_HiringDecision_Application_Unique",
                table: "HR_HiringDecision",
                column: "JobApplicationFk",
                unique: true,
                filter: "[JobApplicationFk] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HR_InterviewFeedback_Schedule_Interviewer",
                table: "HR_InterviewFeedback",
                columns: new[] { "InterviewScheduleFk", "InterviewerEmployeeFk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_InterviewPanel_Schedule_Employee",
                table: "HR_InterviewPanel",
                columns: new[] { "InterviewScheduleId", "EmployeeFk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_InterviewRound_Posting_Order",
                table: "HR_InterviewRound",
                columns: new[] { "JobPostingId", "RoundOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_InterviewSchedule_InterviewRoundId",
                table: "HR_InterviewSchedule",
                column: "InterviewRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_InterviewSchedule_JobApplicationId",
                table: "HR_InterviewSchedule",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_JobApplication_Candidate_Posting_Active",
                table: "HR_JobApplication",
                columns: new[] { "CandidateFK", "JobPostingFK" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_HR_JobApplication_CurrentStageFK",
                table: "HR_JobApplication",
                column: "CurrentStageFK");

            migrationBuilder.CreateIndex(
                name: "IX_HR_JobApplication_JobPostingFK",
                table: "HR_JobApplication",
                column: "JobPostingFK");

            migrationBuilder.CreateIndex(
                name: "IX_HR_JobPosting_JobCode_Unique",
                table: "HR_JobPosting",
                column: "JobCode",
                unique: true,
                filter: "[JobCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HR_JobPostingChannel_Posting_Channel_Unique",
                table: "HR_JobPostingChannel",
                columns: new[] { "JobPostingFK", "PostingChannelFK" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_JobPostingChannel_PostingChannelFK",
                table: "HR_JobPostingChannel",
                column: "PostingChannelFK");

            migrationBuilder.CreateIndex(
                name: "IX_HR_JoiningConfirmation_Onboarding_Unique",
                table: "HR_JoiningConfirmation",
                column: "OnboardingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_Offer_JobApplicationFk",
                table: "HR_Offer",
                column: "JobApplicationFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Offer_OfferNumber_Unique",
                table: "HR_Offer",
                column: "OfferNumber",
                unique: true,
                filter: "[OfferNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HR_OfferApproval_OfferFk",
                table: "HR_OfferApproval",
                column: "OfferFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_OfferDocument_OfferFk",
                table: "HR_OfferDocument",
                column: "OfferFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_OfferResponse_OfferFk",
                table: "HR_OfferResponse",
                column: "OfferFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_OfferVersion_OfferFk",
                table: "HR_OfferVersion",
                column: "OfferFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Onboarding_CandidateId",
                table: "HR_Onboarding",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Onboarding_Offer_Unique",
                table: "HR_Onboarding",
                column: "OfferId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_OnboardingDocument_OnboardingFk",
                table: "HR_OnboardingDocument",
                column: "OnboardingFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_OnboardingTask_OnboardingFk",
                table: "HR_OnboardingTask",
                column: "OnboardingFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_OnboardingTask_TaskTemplateFk",
                table: "HR_OnboardingTask",
                column: "TaskTemplateFk");

            migrationBuilder.CreateIndex(
                name: "IX_HR_Requisition_RequisitionNo_Unique",
                table: "HR_Requisition",
                column: "RequisitionNo",
                unique: true,
                filter: "[RequisitionNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HR_RequisitionAssignment_Requisition_Unique",
                table: "HR_RequisitionAssignment",
                column: "RequisitionFk",
                unique: true,
                filter: "[RequisitionFk] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowAction_WorkflowInstanceStepId",
                table: "HR_WorkflowAction",
                column: "WorkflowInstanceStepId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowInstance_CurrentStepId",
                table: "HR_WorkflowInstance",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowInstance_Entity",
                table: "HR_WorkflowInstance",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowInstance_WorkflowDefinitionId",
                table: "HR_WorkflowInstance",
                column: "WorkflowDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowInstanceStep_Employee_Status",
                table: "HR_WorkflowInstanceStep",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowInstanceStep_Instance_Order",
                table: "HR_WorkflowInstanceStep",
                columns: new[] { "WorkflowInstanceId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowInstanceStep_WorkflowStepId",
                table: "HR_WorkflowInstanceStep",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_WorkflowStep_Definition_Order",
                table: "HR_WorkflowStep",
                columns: new[] { "WorkflowDefinitionId", "StepOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "HR_ApplicationStageHistory");

            migrationBuilder.DropTable(
                name: "HR_CandidateDocument");

            migrationBuilder.DropTable(
                name: "HR_DepartmentApprover");

            migrationBuilder.DropTable(
                name: "HR_EvaluationScore");

            migrationBuilder.DropTable(
                name: "HR_HiringDecision");

            migrationBuilder.DropTable(
                name: "HR_InterviewPanel");

            migrationBuilder.DropTable(
                name: "HR_JobPostingChannel");

            migrationBuilder.DropTable(
                name: "HR_JoiningConfirmation");

            migrationBuilder.DropTable(
                name: "HR_OfferApproval");

            migrationBuilder.DropTable(
                name: "HR_OfferDocument");

            migrationBuilder.DropTable(
                name: "HR_OfferResponse");

            migrationBuilder.DropTable(
                name: "HR_OfferVersion");

            migrationBuilder.DropTable(
                name: "HR_OnboardingDocument");

            migrationBuilder.DropTable(
                name: "HR_OnboardingTask");

            migrationBuilder.DropTable(
                name: "HR_Requisition");

            migrationBuilder.DropTable(
                name: "HR_RequisitionAssignment");

            migrationBuilder.DropTable(
                name: "HR_WorkflowAction");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "HR_EvaluationCriteria");

            migrationBuilder.DropTable(
                name: "HR_InterviewFeedback");

            migrationBuilder.DropTable(
                name: "HR_PostingChannel");

            migrationBuilder.DropTable(
                name: "HR_OnboardingTaskTemplate");

            migrationBuilder.DropTable(
                name: "HR_Onboarding");

            migrationBuilder.DropTable(
                name: "HR_WorkflowInstanceStep");

            migrationBuilder.DropTable(
                name: "DocumentCategories");

            migrationBuilder.DropTable(
                name: "HR_InterviewSchedule");

            migrationBuilder.DropTable(
                name: "HR_Offer");

            migrationBuilder.DropTable(
                name: "HR_WorkflowInstance");

            migrationBuilder.DropTable(
                name: "HR_InterviewRound");

            migrationBuilder.DropTable(
                name: "HR_JobApplication");

            migrationBuilder.DropTable(
                name: "HR_WorkflowStep");

            migrationBuilder.DropTable(
                name: "HR_ApplicationStage");

            migrationBuilder.DropTable(
                name: "HR_Candidate");

            migrationBuilder.DropTable(
                name: "HR_JobPosting");

            migrationBuilder.DropTable(
                name: "HR_WorkflowDefinition");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeavePolicies_Employee_Policy_Unique",
                table: "EmployeeLeavePolicies");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeavePolicies_EmployeeId",
                table: "EmployeeLeavePolicies",
                column: "EmployeeId");
        }
    }
}
