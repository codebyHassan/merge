using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class AddTrnEmailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // All other HR tables already exist in the database.
            // Only create TRN_EML_EMAILS if it is not already there.
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TRN_EML_EMAILS'
)
BEGIN
    CREATE TABLE [dbo].[TRN_EML_EMAILS] (
        [EML_ID]        numeric(18,0)  IDENTITY(1,1) NOT NULL,
        [EML_FROM]      nvarchar(max)  NULL,
        [EML_TO]        nvarchar(max)  NULL,
        [EML_CC]        nvarchar(max)  NULL,
        [EML_SUBJECT]   nvarchar(max)  NULL,
        [EML_BODY]      nvarchar(max)  NULL,
        [EML_IS_SENT]   bit            NULL,
        [EML_SENT_ON]   datetime2      NULL,
        [EML_ADD_TAG]   nvarchar(50)   NULL,
        [EML_ADD_STAMP] datetime2      NULL,
        CONSTRAINT [PK_TRN_EML_EMAILS] PRIMARY KEY ([EML_ID])
    );
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TRN_EML_EMAILS'
)
BEGIN
    DROP TABLE [dbo].[TRN_EML_EMAILS];
END
");
        }
    }
}
