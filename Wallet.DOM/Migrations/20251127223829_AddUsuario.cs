using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispositivoMovilAutorizado_Cliente_ClienteId",
                table: "DispositivoMovilAutorizado");

            migrationBuilder.DropForeignKey(
                name: "FK_UbicacionGeolocalizacion_Cliente_ClienteId",
                table: "UbicacionGeolocalizacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Verificacion2FA_Cliente_ClienteId",
                table: "Verificacion2FA");

            migrationBuilder.DropIndex(
                name: "IX_DispositivoMovilAutorizado_ClienteId",
                table: "DispositivoMovilAutorizado");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "DispositivoMovilAutorizado");

            migrationBuilder.DropColumn(
                name: "CodigoPais",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "CorreoElectronico",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Cliente");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Verificacion2FA",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Verificacion2FA_ClienteId",
                table: "Verificacion2FA",
                newName: "IX_Verificacion2FA_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "UbicacionGeolocalizacion",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_UbicacionGeolocalizacion_ClienteId",
                table: "UbicacionGeolocalizacion",
                newName: "IX_UbicacionGeolocalizacion_UsuarioId");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "DispositivoMovilAutorizado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Cliente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPais = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Contrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispositivoMovilAutorizado_UsuarioId",
                table: "DispositivoMovilAutorizado",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EmpresaId",
                table: "Usuario",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Usuario_UsuarioId",
                table: "Cliente",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispositivoMovilAutorizado_Usuario_UsuarioId",
                table: "DispositivoMovilAutorizado",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UbicacionGeolocalizacion_Usuario_UsuarioId",
                table: "UbicacionGeolocalizacion",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verificacion2FA_Usuario_UsuarioId",
                table: "Verificacion2FA",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Usuario_UsuarioId",
                table: "Cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_DispositivoMovilAutorizado_Usuario_UsuarioId",
                table: "DispositivoMovilAutorizado");

            migrationBuilder.DropForeignKey(
                name: "FK_UbicacionGeolocalizacion_Usuario_UsuarioId",
                table: "UbicacionGeolocalizacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Verificacion2FA_Usuario_UsuarioId",
                table: "Verificacion2FA");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_DispositivoMovilAutorizado_UsuarioId",
                table: "DispositivoMovilAutorizado");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "DispositivoMovilAutorizado");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Cliente");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Verificacion2FA",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Verificacion2FA_UsuarioId",
                table: "Verificacion2FA",
                newName: "IX_Verificacion2FA_ClienteId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "UbicacionGeolocalizacion",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_UbicacionGeolocalizacion_UsuarioId",
                table: "UbicacionGeolocalizacion",
                newName: "IX_UbicacionGeolocalizacion_ClienteId");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "DispositivoMovilAutorizado",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPais",
                table: "Cliente",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "Cliente",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoElectronico",
                table: "Cliente",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Cliente",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DispositivoMovilAutorizado_ClienteId",
                table: "DispositivoMovilAutorizado",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_DispositivoMovilAutorizado_Cliente_ClienteId",
                table: "DispositivoMovilAutorizado",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UbicacionGeolocalizacion_Cliente_ClienteId",
                table: "UbicacionGeolocalizacion",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Verificacion2FA_Cliente_ClienteId",
                table: "Verificacion2FA",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
