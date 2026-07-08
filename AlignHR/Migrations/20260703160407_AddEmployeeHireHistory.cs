using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeHireHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateFk",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "HireType",
                table: "emp");

            migrationBuilder.CreateTable(
                name: "HR_EmployeeHireHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    HireType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CandidateFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    OfferId = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    RequisitionFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    ReplacedEmployeeId = table.Column<int>(type: "int", nullable: true),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_EmployeeHireHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_EmployeeHireHistory_department_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_EmployeeHireHistory_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HR_EmployeeHireHistory_emp_ReplacedEmployeeId",
                        column: x => x.ReplacedEmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HR_EmployeeHireHistory_EmployeeId",
                table: "HR_EmployeeHireHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_EmployeeHireHistory_FromDepartmentId",
                table: "HR_EmployeeHireHistory",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HR_EmployeeHireHistory_ReplacedEmployeeId",
                table: "HR_EmployeeHireHistory",
                column: "ReplacedEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HR_EmployeeHireHistory");

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
    }
}
