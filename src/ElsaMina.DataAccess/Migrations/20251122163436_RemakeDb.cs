using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemakeDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BadgeHoldings_UserData_UserId_RoomId",
                table: "BadgeHoldings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBotParameterValues_RoomInfo_RoomId",
                table: "RoomBotParameterValues");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomTeams_RoomInfo_RoomId",
                table: "RoomTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPolls_RoomInfo_RoomInfoId",
                table: "SavedPolls");

            migrationBuilder.DropTable(
                name: "PollSuggestions");

            migrationBuilder.DropTable(
                name: "Repeats");

            migrationBuilder.DropTable(
                name: "RoomInfo");

            migrationBuilder.DropTable(
                name: "UserData");

            migrationBuilder.DropTable(
                name: "UserPlayTimes");

            migrationBuilder.DropIndex(
                name: "IX_SavedPolls_RoomInfoId",
                table: "SavedPolls");

            migrationBuilder.DropColumn(
                name: "RoomInfoId",
                table: "SavedPolls");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "SavedPolls",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrophy",
                table: "Badges",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTeamTournament",
                table: "Badges",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ArcadeLevels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "AddedCommands",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    JoinPhrase = table.Column<string>(type: "text", nullable: true),
                    PlayTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomUsers", x => new { x.Id, x.RoomId });
                });

            migrationBuilder.CreateTable(
                name: "TournamentRecords",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    TournamentsEnteredCount = table.Column<int>(type: "integer", nullable: false),
                    WinsCount = table.Column<int>(type: "integer", nullable: false),
                    RunnerUpCount = table.Column<int>(type: "integer", nullable: false),
                    PlayedGames = table.Column<int>(type: "integer", nullable: false),
                    WonGames = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentRecords", x => new { x.UserId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_TournamentRecords_RoomUsers_UserId_RoomId",
                        columns: x => new { x.UserId, x.RoomId },
                        principalTable: "RoomUsers",
                        principalColumns: new[] { "Id", "RoomId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedPolls_RoomId",
                table: "SavedPolls",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Badges_RoomId",
                table: "Badges",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AddedCommands_RoomId",
                table: "AddedCommands",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddedCommands_Rooms_RoomId",
                table: "AddedCommands",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BadgeHoldings_RoomUsers_UserId_RoomId",
                table: "BadgeHoldings",
                columns: new[] { "UserId", "RoomId" },
                principalTable: "RoomUsers",
                principalColumns: new[] { "Id", "RoomId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Badges_Rooms_RoomId",
                table: "Badges",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBotParameterValues_Rooms_RoomId",
                table: "RoomBotParameterValues",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomTeams_Rooms_RoomId",
                table: "RoomTeams",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPolls_Rooms_RoomId",
                table: "SavedPolls",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddedCommands_Rooms_RoomId",
                table: "AddedCommands");

            migrationBuilder.DropForeignKey(
                name: "FK_BadgeHoldings_RoomUsers_UserId_RoomId",
                table: "BadgeHoldings");

            migrationBuilder.DropForeignKey(
                name: "FK_Badges_Rooms_RoomId",
                table: "Badges");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomBotParameterValues_Rooms_RoomId",
                table: "RoomBotParameterValues");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomTeams_Rooms_RoomId",
                table: "RoomTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPolls_Rooms_RoomId",
                table: "SavedPolls");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "TournamentRecords");

            migrationBuilder.DropTable(
                name: "RoomUsers");

            migrationBuilder.DropIndex(
                name: "IX_SavedPolls_RoomId",
                table: "SavedPolls");

            migrationBuilder.DropIndex(
                name: "IX_Badges_RoomId",
                table: "Badges");

            migrationBuilder.DropIndex(
                name: "IX_AddedCommands_RoomId",
                table: "AddedCommands");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "SavedPolls",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomInfoId",
                table: "SavedPolls",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsTrophy",
                table: "Badges",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsTeamTournament",
                table: "Badges",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ArcadeLevels",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "AddedCommands",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "PollSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoomId = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Suggestion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollSuggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repeats",
                columns: table => new
                {
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Delay = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repeats", x => new { x.RoomId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "RoomInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    JoinPhrase = table.Column<string>(type: "text", nullable: true),
                    OnTime = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => new { x.Id, x.RoomId });
                });

            migrationBuilder.CreateTable(
                name: "UserPlayTimes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    PlayTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlayTimes", x => new { x.UserId, x.RoomId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedPolls_RoomInfoId",
                table: "SavedPolls",
                column: "RoomInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_BadgeHoldings_UserData_UserId_RoomId",
                table: "BadgeHoldings",
                columns: new[] { "UserId", "RoomId" },
                principalTable: "UserData",
                principalColumns: new[] { "Id", "RoomId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBotParameterValues_RoomInfo_RoomId",
                table: "RoomBotParameterValues",
                column: "RoomId",
                principalTable: "RoomInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomTeams_RoomInfo_RoomId",
                table: "RoomTeams",
                column: "RoomId",
                principalTable: "RoomInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPolls_RoomInfo_RoomInfoId",
                table: "SavedPolls",
                column: "RoomInfoId",
                principalTable: "RoomInfo",
                principalColumn: "Id");
        }
    }
}
