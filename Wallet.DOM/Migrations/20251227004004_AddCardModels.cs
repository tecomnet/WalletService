using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class AddCardModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarjetaEmitida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCuentaWallet = table.Column<int>(type: "int", nullable: false),
                    TokenProcesador = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PanEnmascarado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloqueoTemporal = table.Column<bool>(type: "bit", nullable: false),
                    LimiteDiario = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    ComprasEnLineaHabilitadas = table.Column<bool>(type: "bit", nullable: false),
                    RetirosCajeroHabilitados = table.Column<bool>(type: "bit", nullable: false),
                    MotivoCancelacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EstadoEntrega = table.Column<int>(type: "int", nullable: true),
                    NumeroGuia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Paqueteria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NombreImpreso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_TarjetaEmitida", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarjetaEmitida_CuentaWallet_IdCuentaWallet",
                        column: x => x.IdCuentaWallet,
                        principalTable: "CuentaWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarjetaVinculada",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    TokenPasarela = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GatewayCustomerId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PanEnmascarado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Marca = table.Column<int>(type: "int", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsFavorita = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_TarjetaVinculada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarjetaVinculada_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarjetaEmitida_IdCuentaWallet",
                table: "TarjetaEmitida",
                column: "IdCuentaWallet");

            migrationBuilder.CreateIndex(
                name: "IX_TarjetaVinculada_IdCliente",
                table: "TarjetaVinculada",
                column: "IdCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarjetaEmitida");

            migrationBuilder.DropTable(
                name: "TarjetaVinculada");
        }
    }
}
