using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Cliente_Guid",
                table: "Cliente",
                column: "Guid");

            migrationBuilder.CreateTable(
                name: "CuentaWallet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SaldoActual = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    CuentaCLABE = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    ConcurrencyToken = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestCaseID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaWallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentaWallet_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraTransaccion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBilletera = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RefExternaId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraTransaccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BitacoraTransaccion_CuentaWallet_IdBilletera",
                        column: x => x.IdBilletera,
                        principalTable: "CuentaWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesPagoServicio",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTransaccion = table.Column<long>(type: "bigint", nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    NumeroReferencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoAutorizacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPagoServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesPagoServicio_BitacoraTransaccion_IdTransaccion",
                        column: x => x.IdTransaccion,
                        principalTable: "BitacoraTransaccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraTransaccion_IdBilletera",
                table: "BitacoraTransaccion",
                column: "IdBilletera");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaWallet_IdCliente",
                table: "CuentaWallet",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPagoServicio_IdTransaccion",
                table: "DetallesPagoServicio",
                column: "IdTransaccion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesPagoServicio");

            migrationBuilder.DropTable(
                name: "BitacoraTransaccion");

            migrationBuilder.DropTable(
                name: "CuentaWallet");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Cliente_Guid",
                table: "Cliente");
        }
    }
}
