using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProveedorServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    UrlIcono = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_ProveedorServicio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductoProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorServicioId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_ProductoProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoProveedor_ProveedorServicio_ProveedorServicioId",
                        column: x => x.ProveedorServicioId,
                        principalTable: "ProveedorServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicioFavorito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ProveedorServicioId = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroReferencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_ServicioFavorito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicioFavorito_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicioFavorito_ProveedorServicio_ProveedorServicioId",
                        column: x => x.ProveedorServicioId,
                        principalTable: "ProveedorServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedor_ProveedorServicioId",
                table: "ProductoProveedor",
                column: "ProveedorServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicioFavorito_ClienteId",
                table: "ServicioFavorito",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicioFavorito_ProveedorServicioId",
                table: "ServicioFavorito",
                column: "ProveedorServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoProveedor");

            migrationBuilder.DropTable(
                name: "ServicioFavorito");

            migrationBuilder.DropTable(
                name: "ProveedorServicio");
        }
    }
}
