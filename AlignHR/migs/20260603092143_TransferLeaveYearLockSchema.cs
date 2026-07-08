using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class TransferLeaveYearLockSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Move to dbo only if it currently lives in ahrmaster schema
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[ahrmaster].[LeaveYearLocks]') IS NOT NULL
                    ALTER SCHEMA dbo TRANSFER [ahrmaster].[LeaveYearLocks];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[dbo].[LeaveYearLocks]') IS NOT NULL
                    ALTER SCHEMA ahrmaster TRANSFER [dbo].[LeaveYearLocks];
            ");
        }
    }
}
