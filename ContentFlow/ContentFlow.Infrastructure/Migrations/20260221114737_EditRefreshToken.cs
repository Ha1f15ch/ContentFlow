using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "security",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "TokenLookupHash",
                schema: "security",
                table: "RefreshTokens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "dbo",
                table: "Posts",
                type: "nvarchar(4000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10000)");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenLookupHash",
                schema: "security",
                table: "RefreshTokens",
                column: "TokenLookupHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_TokenLookupHash",
                schema: "security",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "TokenLookupHash",
                schema: "security",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "dbo",
                table: "Posts",
                type: "nvarchar(10000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "security",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);
        }
    }
}
