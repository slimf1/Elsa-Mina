using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LessRestrictiveFkForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_Users_Id",
                table: "RoomUsers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
