using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReworkRoomParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommandAutocorrectEnabled",
                table: "RoomParameters");

            migrationBuilder.DropColumn(
                name: "IsShowingErrorMessages",
                table: "RoomParameters");

            migrationBuilder.DropColumn(
                name: "IsShowingTeamLinksPreviews",
                table: "RoomParameters");

            migrationBuilder.DropColumn(
                name: "Locale",
                table: "RoomParameters");

            migrationBuilder.CreateTable(
                name: "RoomBotParameterValue",
                columns: table => new
                {
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    ParameterId = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomBotParameterValue", x => new { x.RoomId, x.ParameterId });
                    table.ForeignKey(
                        name: "FK_RoomBotParameterValue_RoomParameters_RoomId",
                        column: x => x.RoomId,
                        principalTable: "RoomParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomBotParameterValue");

            migrationBuilder.AddColumn<bool>(
                name: "IsCommandAutocorrectEnabled",
                table: "RoomParameters",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowingErrorMessages",
                table: "RoomParameters",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsShowingTeamLinksPreviews",
                table: "RoomParameters",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "RoomParameters",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
