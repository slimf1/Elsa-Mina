using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameRoomInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomBotParameterValues_RoomParameters_RoomId",
                table: "RoomBotParameterValues");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomTeams_RoomParameters_RoomId",
                table: "RoomTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomParameters",
                table: "RoomParameters");

            migrationBuilder.RenameTable(
                name: "RoomParameters",
                newName: "RoomInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomInfo",
                table: "RoomInfo",
                column: "Id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomBotParameterValues_RoomInfo_RoomId",
                table: "RoomBotParameterValues");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomTeams_RoomInfo_RoomId",
                table: "RoomTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomInfo",
                table: "RoomInfo");

            migrationBuilder.RenameTable(
                name: "RoomInfo",
                newName: "RoomParameters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomParameters",
                table: "RoomParameters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBotParameterValues_RoomParameters_RoomId",
                table: "RoomBotParameterValues",
                column: "RoomId",
                principalTable: "RoomParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomTeams_RoomParameters_RoomId",
                table: "RoomTeams",
                column: "RoomId",
                principalTable: "RoomParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
