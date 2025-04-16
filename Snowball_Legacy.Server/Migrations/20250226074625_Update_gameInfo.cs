using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snowball_Legacy.Server.Migrations
{
    /// <inheritdoc />
    public partial class Update_gameInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Developer",
                table: "GameInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdditionalFiles",
                table: "GameInfo",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Developer",
                table: "GameInfo");

            migrationBuilder.DropColumn(
                name: "IsAdditionalFiles",
                table: "GameInfo");
        }
    }
}
