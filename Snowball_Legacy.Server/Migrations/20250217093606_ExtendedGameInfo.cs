using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snowball_Legacy.Server.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedGameInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "GameInfo",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseDate",
                table: "GameInfo",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "GameInfo");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "GameInfo");
        }
    }
}
