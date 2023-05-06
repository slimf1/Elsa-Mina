using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomColumnToAddedCommand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AddedCommands",
                table: "AddedCommands");

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "AddedCommands",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddedCommands",
                table: "AddedCommands",
                columns: new[] { "Id", "RoomId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AddedCommands",
                table: "AddedCommands");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "AddedCommands");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AddedCommands",
                table: "AddedCommands",
                column: "Id");
        }
    }
}
