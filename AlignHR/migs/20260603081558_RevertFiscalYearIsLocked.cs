using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class RevertFiscalYearIsLocked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "FiscalYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "FiscalYear",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
