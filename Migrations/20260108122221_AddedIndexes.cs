using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordTilesApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_GuessLogs_game_id",
                table: "GuessLogs",
                newName: "IX_GuessLog_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_GamesData_player_id",
                table: "GamesData",
                newName: "IX_GamesData_PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_GuessLog_GameId",
                table: "GuessLogs",
                newName: "IX_GuessLogs_game_id");

            migrationBuilder.RenameIndex(
                name: "IX_GamesData_PlayerId",
                table: "GamesData",
                newName: "IX_GamesData_player_id");
        }
    }
}
