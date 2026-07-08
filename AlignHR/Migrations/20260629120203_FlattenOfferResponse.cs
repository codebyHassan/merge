using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class FlattenOfferResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HR_OfferResponse");

            migrationBuilder.AddColumn<DateTime>(
                name: "CandidateJoiningDate",
                table: "HR_Offer",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseComments",
                table: "HR_Offer",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDate",
                table: "HR_Offer",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandidateJoiningDate",
                table: "HR_Offer");

            migrationBuilder.DropColumn(
                name: "ResponseComments",
                table: "HR_Offer");

            migrationBuilder.DropColumn(
                name: "ResponseDate",
                table: "HR_Offer");

            migrationBuilder.CreateTable(
                name: "HR_OfferResponse",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferFk = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HR_OfferResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HR_OfferResponse_HR_Offer_OfferFk",
                        column: x => x.OfferFk,
                        principalTable: "HR_Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HR_OfferResponse_OfferFk",
                table: "HR_OfferResponse",
                column: "OfferFk");
        }
    }
}
