using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ValidWords",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Word_Value",
                table: "ValidWords",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Word_Length",
                table: "ValidWords",
                newName: "length");

            migrationBuilder.RenameColumn(
                name: "Turn",
                table: "GuessLogs",
                newName: "turn");

            migrationBuilder.RenameColumn(
                name: "Guess",
                table: "GuessLogs",
                newName: "guess");

            migrationBuilder.RenameColumn(
                name: "IsCorrect",
                table: "GuessLogs",
                newName: "is_correct");

            migrationBuilder.RenameColumn(
                name: "GuessTime",
                table: "GuessLogs",
                newName: "guess_time");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "GuessLogs",
                newName: "game_id");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "GuessLogs",
                newName: "transaction_id");

            migrationBuilder.RenameColumn(
                name: "Word_Value",
                table: "GamesData",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Word_Length",
                table: "GamesData",
                newName: "length");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "GamesData",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "MaxTurns",
                table: "GamesData",
                newName: "max_turns");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "GamesData",
                newName: "game_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ValidWords",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "ValidWords",
                newName: "Word_Value");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "ValidWords",
                newName: "Word_Length");

            migrationBuilder.RenameColumn(
                name: "turn",
                table: "GuessLogs",
                newName: "Turn");

            migrationBuilder.RenameColumn(
                name: "guess",
                table: "GuessLogs",
                newName: "Guess");

            migrationBuilder.RenameColumn(
                name: "is_correct",
                table: "GuessLogs",
                newName: "IsCorrect");

            migrationBuilder.RenameColumn(
                name: "guess_time",
                table: "GuessLogs",
                newName: "GuessTime");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "GuessLogs",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "transaction_id",
                table: "GuessLogs",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "GamesData",
                newName: "Word_Value");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "GamesData",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "max_turns",
                table: "GamesData",
                newName: "MaxTurns");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "GamesData",
                newName: "Word_Length");

            migrationBuilder.RenameColumn(
                name: "game_id",
                table: "GamesData",
                newName: "GameId");
        }
    }
}
