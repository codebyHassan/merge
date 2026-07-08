using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class MoveLeaveYearLocksToDbo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER SCHEMA dbo TRANSFER [ahrmaster].[LeaveYearLocks];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER SCHEMA ahrmaster TRANSFER [dbo].[LeaveYearLocks];");
        }
    }
}
