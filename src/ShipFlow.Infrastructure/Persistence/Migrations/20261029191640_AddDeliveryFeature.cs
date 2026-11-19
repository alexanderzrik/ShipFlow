using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Packages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Packages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Packages");
        }
    }
}
