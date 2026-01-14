using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordTilesApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedTurnsTakenToGameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "GamesData",
                newName: "end_time");

            migrationBuilder.AddColumn<int>(
                name: "turns_taken",
                table: "GamesData",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "turns_taken",
                table: "GamesData");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "GamesData",
                newName: "EndTime");
        }
    }
}
