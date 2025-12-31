using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReworkUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastOnline",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastSeenRoomId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers");

            migrationBuilder.DropColumn(
                name: "LastOnline",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastSeenRoomId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
