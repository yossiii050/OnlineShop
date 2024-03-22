using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class dd3434 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAddres_ZipCode",
                table: "Orders",
                newName: "ShipZipCode");

            migrationBuilder.RenameColumn(
                name: "UserAddres_Street",
                table: "Orders",
                newName: "ShipStreet");

            migrationBuilder.RenameColumn(
                name: "UserAddres_Country",
                table: "Orders",
                newName: "ShipCountry");

            migrationBuilder.RenameColumn(
                name: "UserAddres_City",
                table: "Orders",
                newName: "ShipCity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShipZipCode",
                table: "Orders",
                newName: "UserAddres_ZipCode");

            migrationBuilder.RenameColumn(
                name: "ShipStreet",
                table: "Orders",
                newName: "UserAddres_Street");

            migrationBuilder.RenameColumn(
                name: "ShipCountry",
                table: "Orders",
                newName: "UserAddres_Country");

            migrationBuilder.RenameColumn(
                name: "ShipCity",
                table: "Orders",
                newName: "UserAddres_City");
        }
    }
}
