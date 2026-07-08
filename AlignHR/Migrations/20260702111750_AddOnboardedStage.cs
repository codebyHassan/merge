using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardedStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE [HR_ApplicationStage] SET [StageOrder] = 8  WHERE [StageName] = N'Rejected';
                UPDATE [HR_ApplicationStage] SET [StageOrder] = 9  WHERE [StageName] = N'Withdrawn';
                IF NOT EXISTS (SELECT 1 FROM [HR_ApplicationStage] WHERE [StageName] = N'Onboarded')
                    INSERT INTO [HR_ApplicationStage] ([StageName],[StageOrder],[IsActive]) VALUES (N'Onboarded', 7, 1);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [HR_ApplicationStage] WHERE [StageName] = N'Onboarded';
                UPDATE [HR_ApplicationStage] SET [StageOrder] = 7  WHERE [StageName] = N'Rejected';
                UPDATE [HR_ApplicationStage] SET [StageOrder] = 8  WHERE [StageName] = N'Withdrawn';
            ");
        }
    }
}
