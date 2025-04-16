using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snowball_Legacy.Server.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameInfo_GameId",
                table: "GameInfo");

            migrationBuilder.AddColumn<int>(
                name: "GameInfoId1",
                table: "GamePicture",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamePicture_GameInfoId1",
                table: "GamePicture",
                column: "GameInfoId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_GameId",
                table: "GameInfo",
                column: "GameId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId1",
                table: "GamePicture",
                column: "GameInfoId1",
                principalTable: "GameInfo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId1",
                table: "GamePicture");

            migrationBuilder.DropIndex(
                name: "IX_GamePicture_GameInfoId1",
                table: "GamePicture");

            migrationBuilder.DropIndex(
                name: "IX_GameInfo_GameId",
                table: "GameInfo");

            migrationBuilder.DropColumn(
                name: "GameInfoId1",
                table: "GamePicture");

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_GameId",
                table: "GameInfo",
                column: "GameId");
        }
    }
}
