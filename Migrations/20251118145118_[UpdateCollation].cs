using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordledDictionaryApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "word_value",
                table: "ValidWords",
                type: "string",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "string");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "word_value",
                table: "ValidWords",
                type: "string",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "string",
                oldCollation: "NOCASE");
        }
    }
}
