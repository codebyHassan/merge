using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.migs
{
    /// <inheritdoc />
    public partial class AddLeaveYearLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaveYearLocks",
                schema: "dbo",
                columns: table => new
                {
                    Year = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
            migrationBuilder.DropTable(
                name: "LeaveYearLocks",
                schema: "dbo");
        }
    }
}
