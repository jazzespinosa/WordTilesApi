using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "game_id",
                table: "GuessLogs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<ulong>(
                name: "transaction_id",
                table: "GuessLogs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<ulong>(
                name: "game_id",
                table: "GamesData",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "game_id",
                table: "GuessLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "transaction_id",
                table: "GuessLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<Guid>(
                name: "game_id",
                table: "GamesData",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
