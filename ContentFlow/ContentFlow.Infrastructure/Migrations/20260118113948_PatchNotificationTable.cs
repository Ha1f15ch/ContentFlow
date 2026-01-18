using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PatchNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserProfiles_UserProfileId",
                schema: "dbo",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                schema: "dbo",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserProfileId",
                schema: "dbo",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserProfiles_UserId",
                schema: "dbo",
                table: "Notifications",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserProfiles_UserId",
                schema: "dbo",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "Notifications",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                schema: "dbo",
                table: "Notifications",
                newName: "IX_Notifications_UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserProfiles_UserProfileId",
                schema: "dbo",
                table: "Notifications",
                column: "UserProfileId",
                principalSchema: "dbo",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
