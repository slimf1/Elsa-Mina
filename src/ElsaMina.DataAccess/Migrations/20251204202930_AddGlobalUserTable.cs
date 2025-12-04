using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badges_Rooms_RoomId",
                table: "Badges");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPolls_Rooms_RoomId",
                table: "SavedPolls");

            migrationBuilder.DropIndex(
                name: "IX_SavedPolls_RoomId",
                table: "SavedPolls");

            migrationBuilder.DropIndex(
                name: "IX_Badges_RoomId",
                table: "Badges");

            migrationBuilder.AddColumn<string>(
                name: "SavedRoomId",
                table: "SavedPolls",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavedRoomId",
                table: "Badges",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    RegisterDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedPolls_SavedRoomId",
                table: "SavedPolls",
                column: "SavedRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Badges_SavedRoomId",
                table: "Badges",
                column: "SavedRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Badges_Rooms_SavedRoomId",
                table: "Badges",
                column: "SavedRoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPolls_Rooms_SavedRoomId",
                table: "SavedPolls",
                column: "SavedRoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badges_Rooms_SavedRoomId",
                table: "Badges");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPolls_Rooms_SavedRoomId",
                table: "SavedPolls");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SavedPolls_SavedRoomId",
                table: "SavedPolls");

            migrationBuilder.DropIndex(
                name: "IX_Badges_SavedRoomId",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "SavedRoomId",
                table: "SavedPolls");

            migrationBuilder.DropColumn(
                name: "SavedRoomId",
                table: "Badges");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPolls_RoomId",
                table: "SavedPolls",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Badges_RoomId",
                table: "Badges",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Badges_Rooms_RoomId",
                table: "Badges",
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
    }
}
