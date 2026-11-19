using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryFeeCurrency",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryFeeCurrency",
                table: "BillOfLadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    RateToSyp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_Currency",
                table: "CurrencyRates",
                column: "Currency",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropColumn(
                name: "DeliveryFeeCurrency",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DeliveryFeeCurrency",
                table: "BillOfLadings");
        }
    }
}
