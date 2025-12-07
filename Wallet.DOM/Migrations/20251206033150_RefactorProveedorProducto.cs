using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProveedorProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Empresa_EmpresaId",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicioFavorito_ProveedorServicio_ProveedorServicioId",
                table: "ServicioFavorito");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Empresa_EmpresaId",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "ProductoProveedor");

            migrationBuilder.DropTable(
                name: "ProveedorServicio");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_EmpresaId",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Usuario");

            migrationBuilder.RenameColumn(
                name: "ProveedorServicioId",
                table: "ServicioFavorito",
                newName: "ProveedorId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicioFavorito_ProveedorServicioId",
                table: "ServicioFavorito",
                newName: "IX_ServicioFavorito_ProveedorId");

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "Cliente",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Broker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Broker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BrokerId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Proveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proveedor_Broker_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "Broker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Producto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producto_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpresaProducto",
                columns: table => new
                {
                    EmpresasId = table.Column<int>(type: "int", nullable: false),
                    ProductosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaProducto", x => new { x.EmpresasId, x.ProductosId });
                    table.ForeignKey(
                        name: "FK_EmpresaProducto_Empresa_EmpresasId",
                        column: x => x.EmpresasId,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmpresaProducto_Producto_ProductosId",
                        column: x => x.ProductosId,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaProducto_ProductosId",
                table: "EmpresaProducto",
                column: "ProductosId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ProveedorId",
                table: "Producto",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_BrokerId",
                table: "Proveedor",
                column: "BrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Empresa_EmpresaId",
                table: "Cliente",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicioFavorito_Proveedor_ProveedorId",
                table: "ServicioFavorito",
                column: "ProveedorId",
                principalTable: "Proveedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Empresa_EmpresaId",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicioFavorito_Proveedor_ProveedorId",
                table: "ServicioFavorito");

            migrationBuilder.DropTable(
                name: "EmpresaProducto");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Broker");

            migrationBuilder.RenameColumn(
                name: "ProveedorId",
                table: "ServicioFavorito",
                newName: "ProveedorServicioId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicioFavorito_ProveedorId",
                table: "ServicioFavorito",
                newName: "IX_ServicioFavorito_ProveedorServicioId");

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Usuario",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "Cliente",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ProveedorServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyToken = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TestCaseID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UrlIcono = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
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
                    ConcurrencyToken = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TestCaseID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EmpresaId",
                table: "Usuario",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoProveedor_ProveedorServicioId",
                table: "ProductoProveedor",
                column: "ProveedorServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Empresa_EmpresaId",
                table: "Cliente",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicioFavorito_ProveedorServicio_ProveedorServicioId",
                table: "ServicioFavorito",
                column: "ProveedorServicioId",
                principalTable: "ProveedorServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Empresa_EmpresaId",
                table: "Usuario",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");
        }
    }
}
