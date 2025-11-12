using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nfe_processor_app.Migrations
{
    /// <inheritdoc />
    public partial class AddNatureOfOperation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NatureOfOperation",
                table: "Nfes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NatureOfOperation",
                table: "Nfes");
        }
    }
}
