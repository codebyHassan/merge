using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class FixLeaveYearLockIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server cannot alter identity on an existing column — drop and recreate.
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[dbo].[LeaveYearLocks]') IS NOT NULL DROP TABLE [dbo].[LeaveYearLocks];
                IF OBJECT_ID('[ahrmaster].[LeaveYearLocks]') IS NOT NULL DROP TABLE [ahrmaster].[LeaveYearLocks];
            ");

            migrationBuilder.CreateTable(
                name: "LeaveYearLocks",
                schema: "dbo",
                columns: table => new
                {
                    Year     = table.Column<int>(type: "int", nullable: false),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LockedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveYearLocks", x => x.Year);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "LeaveYearLocks", schema: "dbo");
        }
    }
}
