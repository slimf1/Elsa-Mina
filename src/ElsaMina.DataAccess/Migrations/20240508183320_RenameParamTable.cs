using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameParamTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomBotParameterValue_RoomParameters_RoomId",
                table: "RoomBotParameterValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomBotParameterValue",
                table: "RoomBotParameterValue");

            migrationBuilder.RenameTable(
                name: "RoomBotParameterValue",
                newName: "RoomBotParameterValues");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomBotParameterValues",
                table: "RoomBotParameterValues",
                columns: new[] { "RoomId", "ParameterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBotParameterValues_RoomParameters_RoomId",
                table: "RoomBotParameterValues",
                column: "RoomId",
                principalTable: "RoomParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomBotParameterValues_RoomParameters_RoomId",
                table: "RoomBotParameterValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomBotParameterValues",
                table: "RoomBotParameterValues");

            migrationBuilder.RenameTable(
                name: "RoomBotParameterValues",
                newName: "RoomBotParameterValue");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomBotParameterValue",
                table: "RoomBotParameterValue",
                columns: new[] { "RoomId", "ParameterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomBotParameterValue_RoomParameters_RoomId",
                table: "RoomBotParameterValue",
                column: "RoomId",
                principalTable: "RoomParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
