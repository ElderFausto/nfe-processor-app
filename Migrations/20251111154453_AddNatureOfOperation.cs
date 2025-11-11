using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nfe_processor_app.Migrations
{
    /// <inheritdoc />
    public partial class AddNatureOfOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductsValue",
                table: "Nfes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProductsValue",
                table: "Nfes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
