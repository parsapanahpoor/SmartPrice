using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPrice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationAndRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // افزودن ستون‌های جدید به جدول AdminUsers
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AdminUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AdminUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // حذف ستون‌های جدید
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AdminUsers");
        }
    }
}
