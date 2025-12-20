using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPrice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBackgroundJobSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns to ScrapingJobs table
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ScrapingJobs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "Unnamed Job");

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CronExpression",
                table: "ScrapingJobs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRunAt",
                table: "ScrapingJobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRunAt",
                table: "ScrapingJobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RunCount",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ScrapingJobs",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Timeout",
                table: "ScrapingJobs",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuccessCount",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FailureCount",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScrapingJobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ScrapingJobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            // Create ScrapingQueues table
            migrationBuilder.CreateTable(
                name: "ScrapingQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScrapingJobId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Marketplace = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrapingQueues_ScrapingJobs_ScrapingJobId",
                        column: x => x.ScrapingJobId,
                        principalTable: "ScrapingJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_ScrapingJobs_IsActive_NextRunAt",
                table: "ScrapingJobs",
                columns: new[] { "IsActive", "NextRunAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ScrapingQueues_ScrapingJobId",
                table: "ScrapingQueues",
                column: "ScrapingJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapingQueues_Status_Priority_ScheduledAt",
                table: "ScrapingQueues",
                columns: new[] { "Status", "Priority", "ScheduledAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop ScrapingQueues table
            migrationBuilder.DropTable(
                name: "ScrapingQueues");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_ScrapingJobs_IsActive_NextRunAt",
                table: "ScrapingJobs");

            // Remove columns from ScrapingJobs
            migrationBuilder.DropColumn(name: "Name", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "Frequency", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "Priority", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "CronExpression", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "NextRunAt", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "LastRunAt", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "RunCount", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "IsActive", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "MaxRetries", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "Timeout", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "SuccessCount", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "FailureCount", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "ScrapingJobs");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "ScrapingJobs");
        }
    }
}
