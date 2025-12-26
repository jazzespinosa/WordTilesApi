using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordTilesApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    firebase_uid = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.Id);
                    table.UniqueConstraint("AK_UserData_player_id", x => x.player_id);
                });

            migrationBuilder.CreateTable(
                name: "ValidWords",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    word_value = table.Column<string>(type: "string", nullable: false, collation: "NOCASE"),
                    word_length = table.Column<int>(type: "int", nullable: false),
                    is_solution = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidWords", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "GamesData",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    word_value = table.Column<string>(type: "string", nullable: false),
                    word_length = table.Column<int>(type: "int", nullable: false),
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    max_turns = table.Column<int>(type: "INTEGER", nullable: false),
                    game_status = table.Column<string>(type: "string", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesData", x => x.game_id);
                    table.ForeignKey(
                        name: "FK_GamesData_UserData_player_id",
                        column: x => x.player_id,
                        principalTable: "UserData",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuessLogs",
                columns: table => new
                {
                    transaction_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    game_id = table.Column<int>(type: "INTEGER", nullable: false),
                    guess = table.Column<string>(type: "TEXT", nullable: false),
                    guess_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    is_correct = table.Column<bool>(type: "INTEGER", nullable: false),
                    turn = table.Column<int>(type: "INTEGER", nullable: false),
                    max_turns = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuessLogs", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_GuessLogs_GamesData_game_id",
                        column: x => x.game_id,
                        principalTable: "GamesData",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamesData_player_id",
                table: "GamesData",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuessLogs_game_id",
                table: "GuessLogs",
                column: "game_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuessLogs");

            migrationBuilder.DropTable(
                name: "ValidWords");

            migrationBuilder.DropTable(
                name: "GamesData");

            migrationBuilder.DropTable(
                name: "UserData");
        }
    }
}
