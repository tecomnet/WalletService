using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RelacionUnoUnoTransaccionDetallePagoServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraTransaccion_CuentaWallet_IdBilletera",
                table: "BitacoraTransaccion");

            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraTransaccion_DetallesPagoServicio_DetallesPagoServicioId",
                table: "BitacoraTransaccion");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_IdTransaccion",
                table: "DetallesPagoServicio");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPagoServicio_IdTransaccion",
                table: "DetallesPagoServicio");

            migrationBuilder.DropIndex(
                name: "IX_BitacoraTransaccion_DetallesPagoServicioId",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "IdProducto",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "DetallesPagoServicioId",
                table: "BitacoraTransaccion");

            migrationBuilder.RenameColumn(
                name: "IdTransaccion",
                table: "DetallesPagoServicio",
                newName: "BitacoraTransaccionId");

            migrationBuilder.RenameColumn(
                name: "IdBilletera",
                table: "BitacoraTransaccion",
                newName: "CuentaWalletId");

            migrationBuilder.RenameIndex(
                name: "IX_BitacoraTransaccion_IdBilletera",
                table: "BitacoraTransaccion",
                newName: "IX_BitacoraTransaccion_CuentaWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPagoServicio_BitacoraTransaccionId",
                table: "DetallesPagoServicio",
                column: "BitacoraTransaccionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BitacoraTransaccion_CuentaWallet_CuentaWalletId",
                table: "BitacoraTransaccion",
                column: "CuentaWalletId",
                principalTable: "CuentaWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_BitacoraTransaccionId",
                table: "DetallesPagoServicio",
                column: "BitacoraTransaccionId",
                principalTable: "BitacoraTransaccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraTransaccion_CuentaWallet_CuentaWalletId",
                table: "BitacoraTransaccion");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_BitacoraTransaccionId",
                table: "DetallesPagoServicio");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPagoServicio_BitacoraTransaccionId",
                table: "DetallesPagoServicio");

            migrationBuilder.RenameColumn(
                name: "BitacoraTransaccionId",
                table: "DetallesPagoServicio",
                newName: "IdTransaccion");

            migrationBuilder.RenameColumn(
                name: "CuentaWalletId",
                table: "BitacoraTransaccion",
                newName: "IdBilletera");

            migrationBuilder.RenameIndex(
                name: "IX_BitacoraTransaccion_CuentaWalletId",
                table: "BitacoraTransaccion",
                newName: "IX_BitacoraTransaccion_IdBilletera");

            migrationBuilder.AddColumn<int>(
                name: "IdProducto",
                table: "DetallesPagoServicio",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DetallesPagoServicioId",
                table: "BitacoraTransaccion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPagoServicio_IdTransaccion",
                table: "DetallesPagoServicio",
                column: "IdTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraTransaccion_DetallesPagoServicioId",
                table: "BitacoraTransaccion",
                column: "DetallesPagoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_BitacoraTransaccion_CuentaWallet_IdBilletera",
                table: "BitacoraTransaccion",
                column: "IdBilletera",
                principalTable: "CuentaWallet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BitacoraTransaccion_DetallesPagoServicio_DetallesPagoServicioId",
                table: "BitacoraTransaccion",
                column: "DetallesPagoServicioId",
                principalTable: "DetallesPagoServicio",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_IdTransaccion",
                table: "DetallesPagoServicio",
                column: "IdTransaccion",
                principalTable: "BitacoraTransaccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
