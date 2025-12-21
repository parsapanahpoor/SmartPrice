using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPrice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelegramBotSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create TelegramUsers table
            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastInteractionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true, defaultValue: "fa"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                });

            // Create UserProductTrackings table
            migrationBuilder.CreateTable(
                name: "UserProductTrackings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    NotifyOnAnyPriceChange = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    NotifyOnAvailability = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastNotifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProductTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProductTrackings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProductTrackings_TelegramUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TelegramUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create NotificationLogs table
            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_TelegramUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "TelegramUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for TelegramUsers
            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_ChatId",
                table: "TelegramUsers",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_Username",
                table: "TelegramUsers",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_IsActive",
                table: "TelegramUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUsers_LastInteractionAt",
                table: "TelegramUsers",
                column: "LastInteractionAt");

            // Create indexes for UserProductTrackings
            migrationBuilder.CreateIndex(
                name: "IX_UserProductTrackings_UserId",
                table: "UserProductTrackings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProductTrackings_ProductId",
                table: "UserProductTrackings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProductTrackings_UserId_ProductId_IsActive",
                table: "UserProductTrackings",
                columns: new[] { "UserId", "ProductId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProductTrackings_IsActive",
                table: "UserProductTrackings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserProductTrackings_LastNotifiedAt",
                table: "UserProductTrackings",
                column: "LastNotifiedAt");

            // Create indexes for NotificationLogs
            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_UserId",
                table: "NotificationLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_ProductId",
                table: "NotificationLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_Type",
                table: "NotificationLogs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_IsSent",
                table: "NotificationLogs",
                column: "IsSent");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_SentAt",
                table: "NotificationLogs",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_UserId_SentAt",
                table: "NotificationLogs",
                columns: new[] { "UserId", "SentAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "NotificationLogs");
            migrationBuilder.DropTable(name: "UserProductTrackings");
            migrationBuilder.DropTable(name: "TelegramUsers");
        }
    }
}
