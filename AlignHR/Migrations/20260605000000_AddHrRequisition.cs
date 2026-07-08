using AlignHR.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260605000000_AddHrRequisition")]
    public partial class AddHrRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_Requisition]', N'U') IS NULL
BEGIN
    CREATE TABLE [HR_Requisition]
    (
        [Id] NUMERIC(18,0) IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HR_Requisition] PRIMARY KEY,
        [RequisitionNo] NVARCHAR(30) NULL,
        [EmployeeFK] NUMERIC(18,0) NULL,
        [DepartmentFK] NUMERIC(18,0) NULL,
        [PositionTitle] NVARCHAR(200) NULL,
        [VacancyCount] NUMERIC(18,0) NULL,
        [Reason] NVARCHAR(1000) NULL,
        [Status] NVARCHAR(30) NULL,
        [RequiredByDate] DATE NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedOn] DATETIME2 NULL,
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedOn] DATETIME2 NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_HR_Requisition_RequisitionNo_Unique' AND [object_id] = OBJECT_ID(N'[HR_Requisition]'))
BEGIN
    CREATE UNIQUE INDEX [IX_HR_Requisition_RequisitionNo_Unique]
    ON [HR_Requisition] ([RequisitionNo])
    WHERE [RequisitionNo] IS NOT NULL;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[HR_Requisition]', N'U') IS NOT NULL
BEGIN
    DROP TABLE [HR_Requisition];
END;
");
        }
    }
}
