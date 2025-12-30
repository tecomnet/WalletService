using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTarjetaVinculadaCardNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenPasarela",
                table: "TarjetaVinculada");

            migrationBuilder.AddColumn<string>(
                name: "NumeroTarjeta",
                table: "TarjetaVinculada",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroTarjeta",
                table: "TarjetaVinculada");

            migrationBuilder.AddColumn<string>(
                name: "TokenPasarela",
                table: "TarjetaVinculada",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
