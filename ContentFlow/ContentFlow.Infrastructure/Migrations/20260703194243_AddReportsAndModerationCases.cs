using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReportsAndModerationCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModerationCases",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: true),
                    CommentId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ReportCount = table.Column<int>(type: "int", nullable: false),
                    UniqueReporterCount = table.Column<int>(type: "int", nullable: false),
                    FirstReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedModeratorId = table.Column<int>(type: "int", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedById = table.Column<int>(type: "int", nullable: true),
                    Decision = table.Column<int>(type: "int", nullable: true),
                    ModeratorNote = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationCases", x => x.Id);
                    table.CheckConstraint("CK_ModerationCases_Target", "([PostId] IS NOT NULL AND [CommentId] IS NULL) OR ([PostId] IS NULL AND [CommentId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ModerationCases_AspNetUsers_AssignedModeratorId",
                        column: x => x.AssignedModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModerationCases_AspNetUsers_ResolvedById",
                        column: x => x.ResolvedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModerationCases_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModerationCases_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "dbo",
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReporterId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: true),
                    CommentId = table.Column<int>(type: "int", nullable: true),
                    ReasonType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.CheckConstraint("CK_Reports_Target", "([PostId] IS NOT NULL AND [CommentId] IS NULL) OR ([PostId] IS NULL AND [CommentId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "dbo",
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_AssignedModeratorId",
                schema: "dbo",
                table: "ModerationCases",
                column: "AssignedModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_CommentId",
                schema: "dbo",
                table: "ModerationCases",
                column: "CommentId",
                unique: true,
                filter: "[CommentId] IS NOT NULL AND [Status] IN (0, 1)");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_LastReportedAt",
                schema: "dbo",
                table: "ModerationCases",
                column: "LastReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_PostId",
                schema: "dbo",
                table: "ModerationCases",
                column: "PostId",
                unique: true,
                filter: "[PostId] IS NOT NULL AND [Status] IN (0, 1)");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_Priority",
                schema: "dbo",
                table: "ModerationCases",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_ResolvedById",
                schema: "dbo",
                table: "ModerationCases",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationCases_Status",
                schema: "dbo",
                table: "ModerationCases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CommentId",
                schema: "dbo",
                table: "Reports",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedAt",
                schema: "dbo",
                table: "Reports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PostId",
                schema: "dbo",
                table: "Reports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReasonType",
                schema: "dbo",
                table: "Reports",
                column: "ReasonType");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReporterId_CommentId",
                schema: "dbo",
                table: "Reports",
                columns: new[] { "ReporterId", "CommentId" },
                unique: true,
                filter: "[CommentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReporterId_PostId",
                schema: "dbo",
                table: "Reports",
                columns: new[] { "ReporterId", "PostId" },
                unique: true,
                filter: "[PostId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModerationCases",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Reports",
                schema: "dbo");
        }
    }
}
