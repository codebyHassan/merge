using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlignHR.Migrations
{
    public partial class DropPostingChannels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[ahrmaster].[HR_JobPostingChannel]', 'U') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HR_JobPostingChannel_Posting_Channel_Unique')
                        DROP INDEX [IX_HR_JobPostingChannel_Posting_Channel_Unique] ON [ahrmaster].[HR_JobPostingChannel];

                    DROP TABLE [ahrmaster].[HR_JobPostingChannel];
                END

                IF OBJECT_ID('[ahrmaster].[HR_PostingChannel]', 'U') IS NOT NULL
                    DROP TABLE [ahrmaster].[HR_PostingChannel];
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[ahrmaster].[HR_PostingChannel]', 'U') IS NULL
                BEGIN
                    CREATE TABLE [ahrmaster].[HR_PostingChannel] (
                        [Id]          NUMERIC(18,0) IDENTITY(1,1) NOT NULL,
                        [ChannelName] NVARCHAR(100) NOT NULL,
                        [IsInternal]  BIT NOT NULL DEFAULT 0,
                        [IsActive]    BIT NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_HR_PostingChannel] PRIMARY KEY ([Id])
                    );
                END

                IF OBJECT_ID('[ahrmaster].[HR_JobPostingChannel]', 'U') IS NULL
                BEGIN
                    CREATE TABLE [ahrmaster].[HR_JobPostingChannel] (
                        [Id]                NUMERIC(18,0) IDENTITY(1,1) NOT NULL,
                        [JobPostingFK]      NUMERIC(18,0) NOT NULL,
                        [PostingChannelFK]  NUMERIC(18,0) NOT NULL,
                        [PublishedDate]     DATETIME2 NULL,
                        [ExternalReference] NVARCHAR(500) NULL,
                        CONSTRAINT [PK_HR_JobPostingChannel] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_HR_JobPostingChannel_JobPosting] FOREIGN KEY ([JobPostingFK])
                            REFERENCES [ahrmaster].[HR_JobPosting] ([Id]),
                        CONSTRAINT [FK_HR_JobPostingChannel_PostingChannel] FOREIGN KEY ([PostingChannelFK])
                            REFERENCES [ahrmaster].[HR_PostingChannel] ([Id])
                    );

                    CREATE UNIQUE INDEX [IX_HR_JobPostingChannel_Posting_Channel_Unique]
                        ON [ahrmaster].[HR_JobPostingChannel] ([JobPostingFK], [PostingChannelFK]);
                END
            ");
        }
    }
}
