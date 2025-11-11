using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nfe_processor_app.Migrations
{
    /// <inheritdoc />
    public partial class AddNfeOperationAndProductsValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NatureOfOperation",
                table: "Nfes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductsValue",
                table: "Nfes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NatureOfOperation",
                table: "Nfes");

            migrationBuilder.DropColumn(
                name: "ProductsValue",
                table: "Nfes");
        }
    }
}
