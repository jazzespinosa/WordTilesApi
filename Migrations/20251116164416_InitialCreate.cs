using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GamesData",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Word_Value = table.Column<string>(type: "string", nullable: false),
                    Word_Length = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    MaxTurns = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesData", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "ValidWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Word_Value = table.Column<string>(type: "string", nullable: false),
                    Word_Length = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidWords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamesData");

            migrationBuilder.DropTable(
                name: "ValidWords");
        }
    }
}
