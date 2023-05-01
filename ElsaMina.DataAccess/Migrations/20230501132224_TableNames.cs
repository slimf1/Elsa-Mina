using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BadgeHoldings_RoomSpecificUserData_BadgeHoldersId",
                table: "BadgeHoldings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomSpecificUserData",
                table: "RoomSpecificUserData");

            migrationBuilder.RenameTable(
                name: "RoomSpecificUserData",
                newName: "UserData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserData",
                table: "UserData",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BadgeHoldings_UserData_BadgeHoldersId",
                table: "BadgeHoldings",
                column: "BadgeHoldersId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BadgeHoldings_UserData_BadgeHoldersId",
                table: "BadgeHoldings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserData",
                table: "UserData");

            migrationBuilder.RenameTable(
                name: "UserData",
                newName: "RoomSpecificUserData");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomSpecificUserData",
                table: "RoomSpecificUserData",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BadgeHoldings_RoomSpecificUserData_BadgeHoldersId",
                table: "BadgeHoldings",
                column: "BadgeHoldersId",
                principalTable: "RoomSpecificUserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
