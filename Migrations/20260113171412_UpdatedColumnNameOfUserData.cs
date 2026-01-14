using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordTilesApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedColumnNameOfUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserData",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "UserData",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserData",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "UserData",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "UserData",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserData",
                newName: "Id");
        }
    }
}
