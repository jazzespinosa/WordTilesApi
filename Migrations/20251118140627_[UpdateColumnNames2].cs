using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNames2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "ValidWords",
                newName: "word_value");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "ValidWords",
                newName: "word_length");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "GamesData",
                newName: "word_value");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "GamesData",
                newName: "word_length");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "word_value",
                table: "ValidWords",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "word_length",
                table: "ValidWords",
                newName: "length");

            migrationBuilder.RenameColumn(
                name: "word_value",
                table: "GamesData",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "word_length",
                table: "GamesData",
                newName: "length");
        }
    }
}
