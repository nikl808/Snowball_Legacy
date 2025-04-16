using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Snowball_Legacy.Server.Migrations
{
    /// <inheritdoc />
    public partial class GameInfoNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameInfo_GamePicture_TitlePictureId",
                table: "GameInfo");

            migrationBuilder.DropTable(
                name: "GamePicture");

            migrationBuilder.DropIndex(
                name: "IX_GameInfo_TitlePictureId",
                table: "GameInfo");

            migrationBuilder.DropColumn(
                name: "TitlePictureId",
                table: "GameInfo");

            migrationBuilder.CreateTable(
                name: "GameScreenshot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameInfoId = table.Column<int>(type: "integer", nullable: false),
                    Picture = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScreenshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScreenshot_GameInfo_GameInfoId",
                        column: x => x.GameInfoId,
                        principalTable: "GameInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameTitlePicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameInfoId = table.Column<int>(type: "integer", nullable: false),
                    Picture = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTitlePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTitlePicture_GameInfo_GameInfoId",
                        column: x => x.GameInfoId,
                        principalTable: "GameInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameScreenshot_GameInfoId",
                table: "GameScreenshot",
                column: "GameInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTitlePicture_GameInfoId",
                table: "GameTitlePicture",
                column: "GameInfoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameScreenshot");

            migrationBuilder.DropTable(
                name: "GameTitlePicture");

            migrationBuilder.AddColumn<int>(
                name: "TitlePictureId",
                table: "GameInfo",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GamePicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameInfoId = table.Column<int>(type: "integer", nullable: true),
                    Picture = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamePicture_GameInfo_GameInfoId",
                        column: x => x.GameInfoId,
                        principalTable: "GameInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameInfo_TitlePictureId",
                table: "GameInfo",
                column: "TitlePictureId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePicture_GameInfoId",
                table: "GamePicture",
                column: "GameInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameInfo_GamePicture_TitlePictureId",
                table: "GameInfo",
                column: "TitlePictureId",
                principalTable: "GamePicture",
                principalColumn: "Id");
        }
    }
}
