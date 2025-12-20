using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPrice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScraperEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns to ScrapingJobs table
            migrationBuilder.AddColumn<int>(
                name: "Marketplace",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "ScrapingJobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "ScrapingJobs",
                type: "interval",
                nullable: true);

            // Create ProxyServers table
            migrationBuilder.CreateTable(
                name: "ProxyServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FailureCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastCheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProxyServers", x => x.Id);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_ScrapingJobs_Marketplace",
                table: "ScrapingJobs",
                column: "Marketplace");

            migrationBuilder.CreateIndex(
                name: "IX_ProxyServers_Status",
                table: "ProxyServers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProxyServers_IpAddress_Port",
                table: "ProxyServers",
                columns: new[] { "IpAddress", "Port" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop ProxyServers table
            migrationBuilder.DropTable(
                name: "ProxyServers");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_ScrapingJobs_Marketplace",
                table: "ScrapingJobs");

            // Remove columns from ScrapingJobs
            migrationBuilder.DropColumn(
                name: "Marketplace",
                table: "ScrapingJobs");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "ScrapingJobs");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ScrapingJobs");
        }
    }
}
