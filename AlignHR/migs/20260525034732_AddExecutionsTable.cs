using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class AddExecutionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            return;
// migrationBuilder.DropColumn(
//     name: "CalculationType",
//     table: "PFMembers");

// migrationBuilder.AddColumn<decimal>(
//     name: "PFPercentage",
//     table: "SalaryPeriods",
//     type: "decimal(18,2)",
//     nullable: false,
//     defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PFMembers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "EOBIDetuction",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "updatedby",
                table: "emp",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updateat",
                table: "emp",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "createdby",
                table: "emp",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationFk",
                table: "emp",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentFk",
                table: "emp",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dateofjoin",
                table: "emp",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "emp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankHolderName",
                table: "emp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankNameFk",
                table: "emp",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFixedPFAmount",
                table: "emp",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PFAmount",
                table: "emp",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bonuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    SalaryPeriodID = table.Column<int>(type: "int", nullable: false),
                    BonusTypeID = table.Column<int>(type: "int", nullable: false),
                    BonusPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPercentage = table.Column<bool>(type: "bit", nullable: false),
                    BonusAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: true),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bonuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bonuses_SalaryPeriods_SalaryPeriodID",
                        column: x => x.SalaryPeriodID,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bonuses_emp_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bonuses_valuesets_BonusTypeID",
                        column: x => x.BonusTypeID,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Executions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Period = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsExecuted = table.Column<bool>(type: "bit", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Executions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryPeriodId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AdjustmentCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsAppliedInPayroll = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryAdjustments_SalaryPeriods_SalaryPeriodId",
                        column: x => x.SalaryPeriodId,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalaryAdjustments_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalarySlipMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    SalaryPeriodID = table.Column<int>(type: "int", nullable: false),
                    EmployeeNameSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesignationSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankNameSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumberSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalDaysInMonth = table.Column<int>(type: "int", nullable: false),
                    WithoutPayDaysFK = table.Column<int>(type: "int", nullable: true),
                    UnpaidDaysSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPaidDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossSalaryFK = table.Column<int>(type: "int", nullable: true),
                    GrossSalarySnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAllowances = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ArrearAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BonusAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousCarryForward = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedNetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PayableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewCarryForward = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: true),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalarySlipMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalarySlipMasters_PayRollGenrate_GrossSalaryFK",
                        column: x => x.GrossSalaryFK,
                        principalTable: "PayRollGenrate",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalarySlipMasters_SalaryPeriods_SalaryPeriodID",
                        column: x => x.SalaryPeriodID,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalarySlipMasters_WithoutPayDays_WithoutPayDaysFK",
                        column: x => x.WithoutPayDaysFK,
                        principalTable: "WithoutPayDays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalarySlipMasters_emp_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalarySlipDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalarySlipMasterID = table.Column<int>(type: "int", nullable: false),
                    ComponentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayRollDefinationFK = table.Column<int>(type: "int", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: true),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalarySlipDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalarySlipDetails_PayRollDefinationFile_PayRollDefinationFK",
                        column: x => x.PayRollDefinationFK,
                        principalTable: "PayRollDefinationFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalarySlipDetails_SalarySlipMasters_SalarySlipMasterID",
                        column: x => x.SalarySlipMasterID,
                        principalTable: "SalarySlipMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emp_BankNameFk",
                table: "emp",
                column: "BankNameFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_DesiginationId",
                table: "emp",
                column: "DesiginationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_BonusTypeID",
                table: "Bonuses",
                column: "BonusTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_EmployeeID",
                table: "Bonuses",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Bonuses_SalaryPeriodID",
                table: "Bonuses",
                column: "SalaryPeriodID");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryAdjustments_EmployeeId",
                table: "SalaryAdjustments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryAdjustments_SalaryPeriodId",
                table: "SalaryAdjustments",
                column: "SalaryPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipDetails_PayRollDefinationFK",
                table: "SalarySlipDetails",
                column: "PayRollDefinationFK");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipDetails_SalarySlipMasterID",
                table: "SalarySlipDetails",
                column: "SalarySlipMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipMasters_EmployeeID",
                table: "SalarySlipMasters",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipMasters_GrossSalaryFK",
                table: "SalarySlipMasters",
                column: "GrossSalaryFK");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipMasters_SalaryPeriodID",
                table: "SalarySlipMasters",
                column: "SalaryPeriodID");

            migrationBuilder.CreateIndex(
                name: "IX_SalarySlipMasters_WithoutPayDaysFK",
                table: "SalarySlipMasters",
                column: "WithoutPayDaysFK");

            migrationBuilder.AddForeignKey(
                name: "FK_emp_valuesets_BankNameFk",
                table: "emp",
                column: "BankNameFk",
                principalTable: "valuesets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_emp_valuesets_DesiginationId",
                table: "emp",
                column: "DesiginationId",
                principalTable: "valuesets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            return;
            migrationBuilder.DropForeignKey(
                name: "FK_emp_valuesets_BankNameFk",
                table: "emp");

            migrationBuilder.DropForeignKey(
                name: "FK_emp_valuesets_DesiginationId",
                table: "emp");

            migrationBuilder.DropTable(
                name: "Bonuses");

            migrationBuilder.DropTable(
                name: "Executions");

            migrationBuilder.DropTable(
                name: "SalaryAdjustments");

            migrationBuilder.DropTable(
                name: "SalarySlipDetails");

            migrationBuilder.DropTable(
                name: "SalarySlipMasters");

            migrationBuilder.DropIndex(
                name: "IX_emp_BankNameFk",
                table: "emp");

            migrationBuilder.DropIndex(
                name: "IX_emp_DesiginationId",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "PFPercentage",
                table: "SalaryPeriods");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "EOBIDetuction");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "BankHolderName",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "BankNameFk",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "IsFixedPFAmount",
                table: "emp");

            migrationBuilder.DropColumn(
                name: "PFAmount",
                table: "emp");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "PFMembers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalculationType",
                table: "PFMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "updatedby",
                table: "emp",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updateat",
                table: "emp",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "createdby",
                table: "emp",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LocationFk",
                table: "emp",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentFk",
                table: "emp",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dateofjoin",
                table: "emp",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
