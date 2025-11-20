using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Firmeza.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeTuMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Sales",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Sales",
                newName: "SaleDate");

            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                table: "Sales",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Sales",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "SaleDate",
                table: "Sales",
                newName: "Date");

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Sales",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
