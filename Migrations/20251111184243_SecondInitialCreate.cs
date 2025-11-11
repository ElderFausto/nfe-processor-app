using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nfe_processor_app.Migrations
{
    /// <inheritdoc />
    public partial class SecondInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NatureOfOperation",
                table: "Nfes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NatureOfOperation",
                table: "Nfes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
