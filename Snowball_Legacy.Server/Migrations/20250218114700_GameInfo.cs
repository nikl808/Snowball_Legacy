using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snowball_Legacy.Server.Migrations
{
    /// <inheritdoc />
    public partial class GameInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId",
                table: "GamePicture");

            migrationBuilder.DropForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId1",
                table: "GamePicture");

            migrationBuilder.DropIndex(
                name: "IX_GamePicture_GameInfoId1",
                table: "GamePicture");

            migrationBuilder.DropColumn(
                name: "GameInfoId1",
                table: "GamePicture");

            migrationBuilder.AlterColumn<int>(
                name: "GameInfoId",
                table: "GamePicture",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TitlePictureId",
                table: "GameInfo",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_TitlePictureId",
                table: "GameInfo",
                column: "TitlePictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameInfo_GamePicture_TitlePictureId",
                table: "GameInfo",
                column: "TitlePictureId",
                principalTable: "GamePicture",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId",
                table: "GamePicture",
                column: "GameInfoId",
                principalTable: "GameInfo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameInfo_GamePicture_TitlePictureId",
                table: "GameInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId",
                table: "GamePicture");

            migrationBuilder.DropIndex(
                name: "IX_GameInfo_TitlePictureId",
                table: "GameInfo");

            migrationBuilder.DropColumn(
                name: "TitlePictureId",
                table: "GameInfo");

            migrationBuilder.AlterColumn<int>(
                name: "GameInfoId",
                table: "GamePicture",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId",
                table: "GamePicture",
                column: "GameInfoId",
                principalTable: "GameInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePicture_GameInfo_GameInfoId1",
                table: "GamePicture",
                column: "GameInfoId1",
                principalTable: "GameInfo",
                principalColumn: "Id");
        }
    }
}
