using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class ProductoEnDetallesPagoServicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdProveedor",
                table: "DetallesPagoServicio",
                newName: "IdProducto");

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "DetallesPagoServicio",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DetallesPagoServicioId",
                table: "BitacoraTransaccion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPagoServicio_ProductoId",
                table: "DetallesPagoServicio",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraTransaccion_DetallesPagoServicioId",
                table: "BitacoraTransaccion",
                column: "DetallesPagoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_BitacoraTransaccion_DetallesPagoServicio_DetallesPagoServicioId",
                table: "BitacoraTransaccion",
                column: "DetallesPagoServicioId",
                principalTable: "DetallesPagoServicio",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPagoServicio_Producto_ProductoId",
                table: "DetallesPagoServicio",
                column: "ProductoId",
                principalTable: "Producto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BitacoraTransaccion_DetallesPagoServicio_DetallesPagoServicioId",
                table: "BitacoraTransaccion");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPagoServicio_Producto_ProductoId",
                table: "DetallesPagoServicio");

            migrationBuilder.DropIndex(
                name: "IX_DetallesPagoServicio_ProductoId",
                table: "DetallesPagoServicio");

            migrationBuilder.DropIndex(
                name: "IX_BitacoraTransaccion_DetallesPagoServicioId",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "DetallesPagoServicioId",
                table: "BitacoraTransaccion");

            migrationBuilder.RenameColumn(
                name: "IdProducto",
                table: "DetallesPagoServicio",
                newName: "IdProveedor");
        }
    }
}
