using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddedCommands",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddedCommands", x => new { x.Id, x.RoomId });
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    IsTrophy = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => new { x.Id, x.RoomId });
                });

            migrationBuilder.CreateTable(
                name: "RoomParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsShowingErrorMessages = table.Column<bool>(type: "boolean", nullable: true),
                    IsCommandAutocorrectEnabled = table.Column<bool>(type: "boolean", nullable: true),
                    Locale = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    OnTime = table.Column<long>(type: "bigint", nullable: true),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => new { x.Id, x.RoomId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    RegDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BadgeHoldings",
                columns: table => new
                {
                    BadgeHoldersId = table.Column<string>(type: "text", nullable: false),
                    BadgeHoldersRoomId = table.Column<string>(type: "text", nullable: false),
                    BadgesId = table.Column<string>(type: "text", nullable: false),
                    BadgesRoomId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeHoldings", x => new { x.BadgeHoldersId, x.BadgeHoldersRoomId, x.BadgesId, x.BadgesRoomId });
                    table.ForeignKey(
                        name: "FK_BadgeHoldings_Badges_BadgesId_BadgesRoomId",
                        columns: x => new { x.BadgesId, x.BadgesRoomId },
                        principalTable: "Badges",
                        principalColumns: new[] { "Id", "RoomId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BadgeHoldings_UserData_BadgeHoldersId_BadgeHoldersRoomId",
                        columns: x => new { x.BadgeHoldersId, x.BadgeHoldersRoomId },
                        principalTable: "UserData",
                        principalColumns: new[] { "Id", "RoomId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BadgeHoldings_BadgesId_BadgesRoomId",
                table: "BadgeHoldings",
                columns: new[] { "BadgesId", "BadgesRoomId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddedCommands");

            migrationBuilder.DropTable(
                name: "BadgeHoldings");

            migrationBuilder.DropTable(
                name: "RoomParameters");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "UserData");
        }
    }
}
