using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateFkAndHireTypeToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CandidateFk",
                table: "emp",
                type: "numeric(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HireType",
                table: "emp",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateFk",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "HireType",
                table: "emp");
        }
    }
}
