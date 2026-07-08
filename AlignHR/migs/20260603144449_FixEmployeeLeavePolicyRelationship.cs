using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class FixEmployeeLeavePolicyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove orphaned assignments where the LeavePolicy was deleted
            migrationBuilder.Sql(@"
                DELETE FROM EmployeeLeavePolicies
                WHERE LeavePolicyId NOT IN (SELECT Id FROM LeavePolicy);
            ");

            // Unique index: one assignment record per employee per policy
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeavePolicies_Employee_Policy_Unique",
                table: "EmployeeLeavePolicies",
                columns: new[] { "EmployeeId", "LeavePolicyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeavePolicies_Employee_Policy_Unique",
                table: "EmployeeLeavePolicies");
        }
    }
}
