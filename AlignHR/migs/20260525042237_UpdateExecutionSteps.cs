using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class UpdateExecutionSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BonusFetched",
                table: "Executions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EOBIExecuted",
                table: "Executions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncomeTaxExecuted",
                table: "Executions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LoanDeductionExecuted",
                table: "Executions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PFExecuted",
                table: "Executions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SalaryAdjustmentFetched",
                table: "Executions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusFetched",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "EOBIExecuted",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxExecuted",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "LoanDeductionExecuted",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "PFExecuted",
                table: "Executions");

            migrationBuilder.DropColumn(
                name: "SalaryAdjustmentFetched",
                table: "Executions");
        }
    }
}
