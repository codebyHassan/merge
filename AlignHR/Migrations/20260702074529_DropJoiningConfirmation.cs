using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class DropJoiningConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HR_JoiningConfirmation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HR_JoiningConfirmation",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnboardingId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ConfirmedByEmployeeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "date", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_HR_JoiningConfirmation_Onboarding_Unique",
                table: "HR_JoiningConfirmation",
                column: "OnboardingId",
                unique: true);
        }
    }
}
