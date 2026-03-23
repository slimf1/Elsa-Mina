using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddVoltorbFlipMaxLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxLevel",
                table: "VoltorbFlipLevels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"VoltorbFlipLevels\" SET \"MaxLevel\" = \"Level\"");

            migrationBuilder.CreateTable(
                name: "EventRoleMappings",
                columns: table => new
                {
                    EventName = table.Column<string>(type: "text", nullable: false),
                    RoomId = table.Column<string>(type: "text", nullable: false),
                    DiscordRoleId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRoleMappings", x => new { x.EventName, x.RoomId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRoleMappings");

            migrationBuilder.DropColumn(
                name: "MaxLevel",
                table: "VoltorbFlipLevels");
        }
    }
}
