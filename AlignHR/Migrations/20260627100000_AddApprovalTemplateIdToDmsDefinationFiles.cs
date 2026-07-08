using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    public partial class AddApprovalTemplateIdToDmsDefinationFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalTemplateId",
                table: "DmsDefinationFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DmsDefinationFiles_ApprovalTemplateId",
                table: "DmsDefinationFiles",
                column: "ApprovalTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_DmsDefinationFiles_ApprovalTemplates_ApprovalTemplateId",
                table: "DmsDefinationFiles",
                column: "ApprovalTemplateId",
                principalTable: "ApprovalTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DmsDefinationFiles_ApprovalTemplates_ApprovalTemplateId",
                table: "DmsDefinationFiles");

            migrationBuilder.DropIndex(
                name: "IX_DmsDefinationFiles_ApprovalTemplateId",
                table: "DmsDefinationFiles");

            migrationBuilder.DropColumn(
                name: "ApprovalTemplateId",
                table: "DmsDefinationFiles");
        }
    }
}
