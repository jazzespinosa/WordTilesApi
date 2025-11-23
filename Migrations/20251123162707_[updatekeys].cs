using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class updatekeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameDataGameId",
                table: "GuessLogs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuessLogs_game_id",
                table: "GuessLogs",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_GuessLogs_GameDataGameId",
                table: "GuessLogs",
                column: "GameDataGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuessLogs_GamesData_GameDataGameId",
                table: "GuessLogs",
                column: "GameDataGameId",
                principalTable: "GamesData",
                principalColumn: "game_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuessLogs_GamesData_game_id",
                table: "GuessLogs",
                column: "game_id",
                principalTable: "GamesData",
                principalColumn: "game_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuessLogs_GamesData_GameDataGameId",
                table: "GuessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_GuessLogs_GamesData_game_id",
                table: "GuessLogs");

            migrationBuilder.DropIndex(
                name: "IX_GuessLogs_game_id",
                table: "GuessLogs");

            migrationBuilder.DropIndex(
                name: "IX_GuessLogs_GameDataGameId",
                table: "GuessLogs");

            migrationBuilder.DropColumn(
                name: "GameDataGameId",
                table: "GuessLogs");
        }
    }
}
