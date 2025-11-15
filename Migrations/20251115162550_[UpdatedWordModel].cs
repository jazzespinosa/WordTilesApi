using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedWordModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Word",
                table: "Entries");

            migrationBuilder.AddColumn<int>(
                name: "Word_Length",
                table: "Entries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Word_Value",
                table: "Entries",
                type: "string",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Word_Length",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "Word_Value",
                table: "Entries");

            migrationBuilder.AddColumn<string>(
                name: "Word",
                table: "Entries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
