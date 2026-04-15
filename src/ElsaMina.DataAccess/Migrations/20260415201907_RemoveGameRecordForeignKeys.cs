using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGameRecordForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FloodItScores_Users_UserId",
                table: "FloodItScores");

            migrationBuilder.DropForeignKey(
                name: "FK_LightsOutScores_Users_UserId",
                table: "LightsOutScores");

            migrationBuilder.DropForeignKey(
                name: "FK_TwentyFortyEightScores_Users_UserId",
                table: "TwentyFortyEightScores");

            migrationBuilder.DropForeignKey(
                name: "FK_VoltorbFlipLevels_Users_UserId",
                table: "VoltorbFlipLevels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_FloodItScores_Users_UserId",
                table: "FloodItScores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LightsOutScores_Users_UserId",
                table: "LightsOutScores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TwentyFortyEightScores_Users_UserId",
                table: "TwentyFortyEightScores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoltorbFlipLevels_Users_UserId",
                table: "VoltorbFlipLevels",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
