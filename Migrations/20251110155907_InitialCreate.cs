using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nfe_processor_app.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nfes",
                columns: table => new
                {
                    AccessKey = table.Column<string>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IssuerName = table.Column<string>(type: "TEXT", nullable: false),
                    IssuerCNPJ = table.Column<string>(type: "TEXT", nullable: false),
                    RecipientName = table.Column<string>(type: "TEXT", nullable: false),
                    RecipientCNPJ = table.Column<string>(type: "TEXT", nullable: false),
                    TotalValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    IcmsValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    IpiValue = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nfes", x => x.AccessKey);
                });

            migrationBuilder.CreateTable(
                name: "NfeProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    NfeAccessKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NfeProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NfeProducts_Nfes_NfeAccessKey",
                        column: x => x.NfeAccessKey,
                        principalTable: "Nfes",
                        principalColumn: "AccessKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NfeProducts_NfeAccessKey",
                table: "NfeProducts",
                column: "NfeAccessKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NfeProducts");

            migrationBuilder.DropTable(
                name: "Nfes");
        }
    }
}
