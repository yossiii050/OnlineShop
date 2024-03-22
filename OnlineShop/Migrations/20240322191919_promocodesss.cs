using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class promocodesss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_promoCodes",
                table: "promoCodes");

            migrationBuilder.RenameTable(
                name: "promoCodes",
                newName: "PromoCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PromoCodes",
                table: "PromoCodes",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PromoCodes",
                table: "PromoCodes");

            migrationBuilder.RenameTable(
                name: "PromoCodes",
                newName: "promoCodes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_promoCodes",
                table: "promoCodes",
                column: "Id");
        }
    }
}
