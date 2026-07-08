using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    public partial class DropHrRequisitionVacancyCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('[ahrmaster].[HR_Requisition]', 'VacancyCount') IS NOT NULL
                    ALTER TABLE [ahrmaster].[HR_Requisition] DROP COLUMN [VacancyCount];

                IF COL_LENGTH('[ahrmaster].[HR_Requisition]', 'RequiredByDate') IS NOT NULL
                    ALTER TABLE [ahrmaster].[HR_Requisition] DROP COLUMN [RequiredByDate];

                IF COL_LENGTH('[ahrmaster].[HR_Requisition]', 'TransferToDepartmentId') IS NOT NULL
                    ALTER TABLE [ahrmaster].[HR_Requisition] DROP COLUMN [TransferToDepartmentId];
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('[ahrmaster].[HR_Requisition]', 'VacancyCount') IS NULL
                    ALTER TABLE [ahrmaster].[HR_Requisition] ADD [VacancyCount] NUMERIC(18,0) NULL;

                IF COL_LENGTH('[ahrmaster].[HR_Requisition]', 'RequiredByDate') IS NULL
                    ALTER TABLE [ahrmaster].[HR_Requisition] ADD [RequiredByDate] DATE NULL;

                IF COL_LENGTH('[ahrmaster].[HR_Requisition]', 'TransferToDepartmentId') IS NULL
                    ALTER TABLE [ahrmaster].[HR_Requisition] ADD [TransferToDepartmentId] NUMERIC(18,0) NULL;
            ");
        }
    }
}
