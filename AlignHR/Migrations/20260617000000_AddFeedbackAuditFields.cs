using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260617000000_AddFeedbackAuditFields")]
    public partial class AddFeedbackAuditFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubmitted",
                table: "HR_InterviewFeedback",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "UpdatedBy",
                table: "HR_InterviewFeedback",
                type: "numeric(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "HR_InterviewFeedback",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubmittedBy",
                table: "HR_InterviewFeedback",
                type: "numeric(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "HR_InterviewFeedback",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsSubmitted",  table: "HR_InterviewFeedback");
            migrationBuilder.DropColumn(name: "UpdatedBy",    table: "HR_InterviewFeedback");
            migrationBuilder.DropColumn(name: "UpdatedAt",    table: "HR_InterviewFeedback");
            migrationBuilder.DropColumn(name: "SubmittedBy",  table: "HR_InterviewFeedback");
            migrationBuilder.DropColumn(name: "SubmittedAt",  table: "HR_InterviewFeedback");
        }
    }
}
