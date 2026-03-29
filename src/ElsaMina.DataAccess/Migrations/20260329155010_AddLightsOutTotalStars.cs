using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLightsOutTotalStars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalStars",
                table: "LightsOutScores",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalStars",
                table: "LightsOutScores");
        }
    }
}
