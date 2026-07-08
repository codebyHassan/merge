using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendencePolicies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LateGraceTime = table.Column<int>(type: "int", nullable: false),
                    OtAfterMintues = table.Column<int>(type: "int", nullable: false),
                    MaxOtPerDay = table.Column<int>(type: "int", nullable: false),
                    WeeklyHourLimit = table.Column<int>(type: "int", nullable: false),
                    EffictiveTime = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFlexiable = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendencePolicies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiscalYear",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startdate = table.Column<DateOnly>(type: "date", nullable: false),
                    enddate = table.Column<DateOnly>(type: "date", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalYear", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "functions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    route = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_functions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveConfigurations",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDepartment = table.Column<bool>(type: "bit", nullable: false),
                    IsSubDepartment = table.Column<bool>(type: "bit", nullable: false),
                    IsGrade = table.Column<bool>(type: "bit", nullable: false),
                    IsDivision = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicy",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    RequiresAttachment = table.Column<bool>(type: "bit", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Overtimes",
                columns: table => new
                {
                    OvertimeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DayType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    RateType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RateValue = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CalculationBase = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AfterShiftMinutes = table.Column<int>(type: "int", nullable: false),
                    RoundType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RoundValue = table.Column<int>(type: "int", nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overtimes", x => x.OvertimeId);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryPeriods",
                columns: table => new
                {
                    SalaryPeriodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    IsPostedToGL = table.Column<bool>(type: "bit", nullable: false),
                    EOBIAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PFPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryPeriods", x => x.SalaryPeriodID);
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GroupName = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    GraceTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    BreakStartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    BreakEndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsNightShift = table.Column<bool>(type: "bit", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "valuesets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_valuesets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxSlabMaster",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiscalFK = table.Column<int>(type: "int", nullable: false),
                    IncomeFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeTo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FixedTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RatePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxSlabMaster", x => x.id);
                    table.ForeignKey(
                        name: "FK_TaxSlabMaster_FiscalYear_FiscalFK",
                        column: x => x.FiscalFK,
                        principalTable: "FiscalYear",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaxSurcharge",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiscalFK = table.Column<int>(type: "int", nullable: false),
                    IncomeThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RatePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IncomeFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeTo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxSurcharge", x => x.id);
                    table.ForeignKey(
                        name: "FK_TaxSurcharge_FiscalYear_FiscalFK",
                        column: x => x.FiscalFK,
                        principalTable: "FiscalYear",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveConfigurationDetails",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveConfigurationId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    ReferenceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApproverEmpNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApproverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveConfigurationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveConfigurationDetails_LeaveConfigurations_LeaveConfigurationId",
                        column: x => x.LeaveConfigurationId,
                        principalTable: "LeaveConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeavePolicyRules",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeavePolicyId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    LeaveTypeId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    AnnualQuota = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MonthlyAccrual = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CarryForwardLimit = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsEncashable = table.Column<bool>(type: "bit", nullable: false),
                    AllowNegativeBalance = table.Column<bool>(type: "bit", nullable: false),
                    AllowHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    AllowDuringProbation = table.Column<bool>(type: "bit", nullable: false),
                    MaxConsecutiveDays = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MinNoticeDays = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ApplySandwichRule = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavePolicyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeavePolicyRules_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeavePolicyRules_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FunctionRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FunctionId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FunctionRoles_functions_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "functions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FunctionRoles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "emp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesiginationId = table.Column<int>(type: "int", nullable: true),
                    Dateofjoin = table.Column<DateOnly>(type: "date", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentTypeFk = table.Column<int>(type: "int", nullable: true),
                    EmploymentStatusFk = table.Column<int>(type: "int", nullable: true),
                    DepartmentFk = table.Column<int>(type: "int", nullable: false),
                    LocationFk = table.Column<int>(type: "int", nullable: false),
                    BankHolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankNameFk = table.Column<int>(type: "int", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NTN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsFiler = table.Column<bool>(type: "bit", nullable: false),
                    TaxExempted = table.Column<bool>(type: "bit", nullable: false),
                    ZakatDeduction = table.Column<bool>(type: "bit", nullable: false),
                    IsServiceEnded = table.Column<bool>(type: "bit", nullable: false),
                    ServiceEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSalaryStopped = table.Column<bool>(type: "bit", nullable: false),
                    SalaryStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EobiNumber = table.Column<int>(type: "int", nullable: true),
                    isEobi = table.Column<bool>(type: "bit", nullable: false),
                    SocailSecuirty = table.Column<int>(type: "int", nullable: true),
                    isSocailSecuirty = table.Column<bool>(type: "bit", nullable: false),
                    isOverTime = table.Column<bool>(type: "bit", nullable: false),
                    OvertimePolicyId = table.Column<int>(type: "int", nullable: true),
                    isPfMember = table.Column<bool>(type: "bit", nullable: false),
                    pfmembernumber = table.Column<int>(type: "int", nullable: true),
                    PFAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsFixedPFAmount = table.Column<bool>(type: "bit", nullable: false),
                    LineManagerEmpNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LeaveApproverEmpNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SubDepartmentFk = table.Column<int>(type: "int", nullable: true),
                    GradeFk = table.Column<int>(type: "int", nullable: true),
                    DivisionFk = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_emp_Overtimes_OvertimePolicyId",
                        column: x => x.OvertimePolicyId,
                        principalTable: "Overtimes",
                        principalColumn: "OvertimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_department_DepartmentFk",
                        column: x => x.DepartmentFk,
                        principalTable: "department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_location_LocationFk",
                        column: x => x.LocationFk,
                        principalTable: "location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_BankNameFk",
                        column: x => x.BankNameFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_DesiginationId",
                        column: x => x.DesiginationId,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_DivisionFk",
                        column: x => x.DivisionFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_EmploymentStatusFk",
                        column: x => x.EmploymentStatusFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_EmploymentTypeFk",
                        column: x => x.EmploymentTypeFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_GradeFk",
                        column: x => x.GradeFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_emp_valuesets_SubDepartmentFk",
                        column: x => x.SubDepartmentFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayRollDefinationFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefinitionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayRollComFK = table.Column<int>(type: "int", nullable: false),
                    label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percentage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRollDefinationFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayRollDefinationFile_valuesets_PayRollComFK",
                        column: x => x.PayRollComFK,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendenceException",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendeceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ExpectionTypeFk = table.Column<int>(type: "int", nullable: true),
                    ExpectionStatusFk = table.Column<int>(type: "int", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendenceException", x => x.id);
                    table.ForeignKey(
                        name: "FK_AttendenceException_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendenceException_valuesets_ExpectionStatusFk",
                        column: x => x.ExpectionStatusFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendenceException_valuesets_ExpectionTypeFk",
                        column: x => x.ExpectionTypeFk,
                        principalTable: "valuesets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendenceLogs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    AttendenceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PunchTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PunchType = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Devicesd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    longitude = table.Column<decimal>(type: "decimal(18,10)", nullable: false),
                    shiftidFk = table.Column<int>(type: "int", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendenceLogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_AttendenceLogs_emp_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendenceLogs_shifts_shiftidFk",
                        column: x => x.shiftidFk,
                        principalTable: "shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "EmployeeLeavePolicies",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeavePolicyId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLeavePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLeavePolicies_LeavePolicy_LeavePolicyId",
                        column: x => x.LeavePolicyId,
                        principalTable: "LeavePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeavePolicies_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeShifts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffiectiveForm = table.Column<DateOnly>(type: "date", nullable: false),
                    EffiectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    RestDay = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeShifts", x => x.id);
                    table.ForeignKey(
                        name: "FK_EmployeeShifts_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeShifts_shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EOBIDetuction",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodSalery = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EOBIDetuction", x => x.id);
                    table.ForeignKey(
                        name: "FK_EOBIDetuction_SalaryPeriods_PeriodSalery",
                        column: x => x.PeriodSalery,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EOBIDetuction_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTaxDetuction",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    incometaxdetection = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeriodSalery = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTaxDetuction", x => x.id);
                    table.ForeignKey(
                        name: "FK_IncomeTaxDetuction_SalaryPeriods_PeriodSalery",
                        column: x => x.PeriodSalery,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomeTaxDetuction_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalances",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Allocated = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Used = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Pending = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CarriedForward = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Available = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveBalances_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveBalances_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveOpenings",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeFk = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveOpenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveOpenings_LeaveTypes_LeaveTypeFk",
                        column: x => x.LeaveTypeFk,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveOpenings_emp_EmployeeFk",
                        column: x => x.EmployeeFk,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "date", nullable: false),
                    ToDate = table.Column<DateTime>(type: "date", nullable: false),
                    DaysCount = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsHalfDay = table.Column<bool>(type: "bit", nullable: false),
                    HalfDaySession = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentApproverId = table.Column<int>(type: "int", nullable: true),
                    CurrentStage = table.Column<int>(type: "int", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByEmpNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_emp_CurrentApproverId",
                        column: x => x.CurrentApproverId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterLoanAdvance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    MonthlyInstallment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanDetuction = table.Column<bool>(type: "bit", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterLoanAdvance", x => x.id);
                    table.ForeignKey(
                        name: "FK_MasterLoanAdvance_emp_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OverTimeHours",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryPeriodFK = table.Column<int>(type: "int", nullable: false),
                    OTHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverTimeHours", x => x.id);
                    table.ForeignKey(
                        name: "FK_OverTimeHours_SalaryPeriods_SalaryPeriodFK",
                        column: x => x.SalaryPeriodFK,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OverTimeHours_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PFMembers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SalaryPeriodFK = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PFMembers", x => x.id);
                    table.ForeignKey(
                        name: "FK_PFMembers_SalaryPeriods_SalaryPeriodFK",
                        column: x => x.SalaryPeriodFK,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PFMembers_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsrIsActive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WithoutPayDays",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    withoutpaydays = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PeriodSalery = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithoutPayDays", x => x.id);
                    table.ForeignKey(
                        name: "FK_WithoutPayDays_SalaryPeriods_PeriodSalery",
                        column: x => x.PeriodSalery,
                        principalTable: "SalaryPeriods",
                        principalColumn: "SalaryPeriodID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WithoutPayDays_emp_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayRollGenrate",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeFK = table.Column<int>(type: "int", nullable: false),
                    PayRollDefinationFK = table.Column<int>(type: "int", nullable: false),
                    salery = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRollGenrate", x => x.id);
                    table.ForeignKey(
                        name: "FK_PayRollGenrate_PayRollDefinationFile_PayRollDefinationFK",
                        column: x => x.PayRollDefinationFK,
                        principalTable: "PayRollDefinationFile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayRollGenrate_emp_EmployeeFK",
                        column: x => x.EmployeeFK,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveApprovals",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveRequestId = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ApproverEmployeeId = table.Column<int>(type: "int", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApprovals_LeaveRequests_LeaveRequestId",
                        column: x => x.LeaveRequestId,
                        principalTable: "LeaveRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApprovals_emp_ApproverEmployeeId",
                        column: x => x.ApproverEmployeeId,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTransactions",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveRequestFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    EmployeeFk = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeFk = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReversalOfTransactionId = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveTransactions_LeaveRequests_LeaveRequestFk",
                        column: x => x.LeaveRequestFk,
                        principalTable: "LeaveRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveTransactions_LeaveTypes_LeaveTypeFk",
                        column: x => x.LeaveTypeFk,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveTransactions_emp_EmployeeFk",
                        column: x => x.EmployeeFk,
                        principalTable: "emp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterLoanAdvanceDetail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterId = table.Column<int>(type: "int", nullable: false),
                    InstallmentNo = table.Column<int>(type: "int", nullable: false),
                    DeductionMonth = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalDeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedby = table.Column<int>(type: "int", nullable: false),
                    updateat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterLoanAdvanceDetail", x => x.id);
                    table.ForeignKey(
                        name: "FK_MasterLoanAdvanceDetail_MasterLoanAdvance_MasterId",
                        column: x => x.MasterId,
                        principalTable: "MasterLoanAdvance",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    createdby = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
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
                name: "IX_AttendenceException_EmployeeId",
                table: "AttendenceException",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceException_ExpectionStatusFk",
                table: "AttendenceException",
                column: "ExpectionStatusFk");

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceException_ExpectionTypeFk",
                table: "AttendenceException",
                column: "ExpectionTypeFk");

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceLogs_EmployeeID",
                table: "AttendenceLogs",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_AttendenceLogs_shiftidFk",
                table: "AttendenceLogs",
                column: "shiftidFk");

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
                name: "IX_emp_BankNameFk",
                table: "emp",
                column: "BankNameFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_DepartmentFk",
                table: "emp",
                column: "DepartmentFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_DesiginationId",
                table: "emp",
                column: "DesiginationId");

            migrationBuilder.CreateIndex(
                name: "IX_emp_DivisionFk",
                table: "emp",
                column: "DivisionFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_EmploymentStatusFk",
                table: "emp",
                column: "EmploymentStatusFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_EmploymentTypeFk",
                table: "emp",
                column: "EmploymentTypeFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_GradeFk",
                table: "emp",
                column: "GradeFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_LocationFk",
                table: "emp",
                column: "LocationFk");

            migrationBuilder.CreateIndex(
                name: "IX_emp_OvertimePolicyId",
                table: "emp",
                column: "OvertimePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_emp_SubDepartmentFk",
                table: "emp",
                column: "SubDepartmentFk");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Code_Unique",
                table: "emp",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeavePolicies_EmployeeId",
                table: "EmployeeLeavePolicies",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeavePolicies_LeavePolicyId",
                table: "EmployeeLeavePolicies",
                column: "LeavePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_EmployeeId",
                table: "EmployeeShifts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_ShiftId",
                table: "EmployeeShifts",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_EOBIDetuction_EmployeeId",
                table: "EOBIDetuction",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EOBIDetuction_PeriodSalery",
                table: "EOBIDetuction",
                column: "PeriodSalery");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionRoles_FunctionId",
                table: "FunctionRoles",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionRoles_RoleId",
                table: "FunctionRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTaxDetuction_EmployeeId",
                table: "IncomeTaxDetuction",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTaxDetuction_PeriodSalery",
                table: "IncomeTaxDetuction",
                column: "PeriodSalery");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApprovals_ApproverEmployeeId",
                table: "LeaveApprovals",
                column: "ApproverEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApprovals_LeaveRequestId",
                table: "LeaveApprovals",
                column: "LeaveRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_EmployeeId",
                table: "LeaveBalances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalances_LeaveTypeId",
                table: "LeaveBalances",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveConfigDetail_ApproverEmpNo",
                table: "LeaveConfigurationDetails",
                column: "ApproverEmpNo");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveConfigDetail_Config_Ref_Unique",
                table: "LeaveConfigurationDetails",
                columns: new[] { "LeaveConfigurationId", "ReferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveOpening_Employee_LeaveType_Year",
                table: "LeaveOpenings",
                columns: new[] { "EmployeeFk", "LeaveTypeFk", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveOpenings_LeaveTypeFk",
                table: "LeaveOpenings",
                column: "LeaveTypeFk");

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyRule_Policy_LeaveType_Unique",
                table: "LeavePolicyRules",
                columns: new[] { "LeavePolicyId", "LeaveTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavePolicyRules_LeaveTypeId",
                table: "LeavePolicyRules",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_CurrentApproverId",
                table: "LeaveRequests",
                column: "CurrentApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_LeaveTypeId",
                table: "LeaveRequests",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTransaction_Employee_LeaveType_Year",
                table: "LeaveTransactions",
                columns: new[] { "EmployeeFk", "LeaveTypeFk", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTransaction_ReferenceNo",
                table: "LeaveTransactions",
                column: "ReferenceNo");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTransactions_LeaveRequestFk",
                table: "LeaveTransactions",
                column: "LeaveRequestFk");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTransactions_LeaveTypeFk",
                table: "LeaveTransactions",
                column: "LeaveTypeFk");

            migrationBuilder.CreateIndex(
                name: "IX_MasterLoanAdvance_EmployeeID",
                table: "MasterLoanAdvance",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_MasterLoanAdvanceDetail_MasterId",
                table: "MasterLoanAdvanceDetail",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_OverTimeHours_EmployeeId",
                table: "OverTimeHours",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_OverTimeHours_SalaryPeriodFK",
                table: "OverTimeHours",
                column: "SalaryPeriodFK");

            migrationBuilder.CreateIndex(
                name: "IX_PayRollDefinationFile_PayRollComFK",
                table: "PayRollDefinationFile",
                column: "PayRollComFK");

            migrationBuilder.CreateIndex(
                name: "IX_PayRollGenrate_EmployeeFK",
                table: "PayRollGenrate",
                column: "EmployeeFK");

            migrationBuilder.CreateIndex(
                name: "IX_PayRollGenrate_PayRollDefinationFK",
                table: "PayRollGenrate",
                column: "PayRollDefinationFK");

            migrationBuilder.CreateIndex(
                name: "IX_PFMembers_EmployeeId",
                table: "PFMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PFMembers_SalaryPeriodFK",
                table: "PFMembers",
                column: "SalaryPeriodFK");

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

            migrationBuilder.CreateIndex(
                name: "IX_TaxSlabMaster_FiscalFK",
                table: "TaxSlabMaster",
                column: "FiscalFK");

            migrationBuilder.CreateIndex(
                name: "IX_TaxSurcharge_FiscalFK",
                table: "TaxSurcharge",
                column: "FiscalFK");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_EmployeeId",
                table: "users",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WithoutPayDays_EmployeeId",
                table: "WithoutPayDays",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WithoutPayDays_PeriodSalery",
                table: "WithoutPayDays",
                column: "PeriodSalery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendenceException");

            migrationBuilder.DropTable(
                name: "AttendenceLogs");

            migrationBuilder.DropTable(
                name: "AttendencePolicies");

            migrationBuilder.DropTable(
                name: "Bonuses");

            migrationBuilder.DropTable(
                name: "EmployeeLeavePolicies");

            migrationBuilder.DropTable(
                name: "EmployeeShifts");

            migrationBuilder.DropTable(
                name: "EOBIDetuction");

            migrationBuilder.DropTable(
                name: "FunctionRoles");

            migrationBuilder.DropTable(
                name: "IncomeTaxDetuction");

            migrationBuilder.DropTable(
                name: "LeaveApprovals");

            migrationBuilder.DropTable(
                name: "LeaveBalances");

            migrationBuilder.DropTable(
                name: "LeaveConfigurationDetails");

            migrationBuilder.DropTable(
                name: "LeaveOpenings");

            migrationBuilder.DropTable(
                name: "LeavePolicyRules");

            migrationBuilder.DropTable(
                name: "LeaveTransactions");

            migrationBuilder.DropTable(
                name: "MasterLoanAdvanceDetail");

            migrationBuilder.DropTable(
                name: "OverTimeHours");

            migrationBuilder.DropTable(
                name: "PFMembers");

            migrationBuilder.DropTable(
                name: "SalaryAdjustments");

            migrationBuilder.DropTable(
                name: "SalarySlipDetails");

            migrationBuilder.DropTable(
                name: "TaxSlabMaster");

            migrationBuilder.DropTable(
                name: "TaxSurcharge");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "shifts");

            migrationBuilder.DropTable(
                name: "functions");

            migrationBuilder.DropTable(
                name: "LeaveConfigurations");

            migrationBuilder.DropTable(
                name: "LeavePolicy");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "MasterLoanAdvance");

            migrationBuilder.DropTable(
                name: "SalarySlipMasters");

            migrationBuilder.DropTable(
                name: "FiscalYear");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "LeaveTypes");

            migrationBuilder.DropTable(
                name: "PayRollGenrate");

            migrationBuilder.DropTable(
                name: "WithoutPayDays");

            migrationBuilder.DropTable(
                name: "PayRollDefinationFile");

            migrationBuilder.DropTable(
                name: "SalaryPeriods");

            migrationBuilder.DropTable(
                name: "emp");

            migrationBuilder.DropTable(
                name: "Overtimes");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "location");

            migrationBuilder.DropTable(
                name: "valuesets");
        }
    }
}
