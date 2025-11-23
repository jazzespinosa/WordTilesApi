using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class fixforeignkeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuessLogs_GamesData_GameDataGameId",
                table: "GuessLogs");

            migrationBuilder.DropIndex(
                name: "IX_GuessLogs_GameDataGameId",
                table: "GuessLogs");

            migrationBuilder.DropColumn(
                name: "GameDataGameId",
                table: "GuessLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameDataGameId",
                table: "GuessLogs",
                type: "INTEGER",
                nullable: true);

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
        }
    }
}
