using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTarjetaVinculadaOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarjetaVinculada_Cliente_IdCliente",
                table: "TarjetaVinculada");

            migrationBuilder.RenameColumn(
                name: "IdCliente",
                table: "TarjetaVinculada",
                newName: "IdCuentaWallet");

            migrationBuilder.RenameIndex(
                name: "IX_TarjetaVinculada_IdCliente",
                table: "TarjetaVinculada",
                newName: "IX_TarjetaVinculada_IdCuentaWallet");

            migrationBuilder.AddForeignKey(
                name: "FK_TarjetaVinculada_CuentaWallet_IdCuentaWallet",
                table: "TarjetaVinculada",
                column: "IdCuentaWallet",
                principalTable: "CuentaWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarjetaVinculada_CuentaWallet_IdCuentaWallet",
                table: "TarjetaVinculada");

            migrationBuilder.RenameColumn(
                name: "IdCuentaWallet",
                table: "TarjetaVinculada",
                newName: "IdCliente");

            migrationBuilder.RenameIndex(
                name: "IX_TarjetaVinculada_IdCuentaWallet",
                table: "TarjetaVinculada",
                newName: "IX_TarjetaVinculada_IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_TarjetaVinculada_Cliente_IdCliente",
                table: "TarjetaVinculada",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
