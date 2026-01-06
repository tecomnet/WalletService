using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class ProveedorAddCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Categoria",
                table: "Proveedor",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Proveedor");
        }
    }
}
