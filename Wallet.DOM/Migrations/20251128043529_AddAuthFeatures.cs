using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verificacion2FA_Usuario_UsuarioId",
                table: "Verificacion2FA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Verificacion2FA",
                table: "Verificacion2FA");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente");

            migrationBuilder.RenameTable(
                name: "Verificacion2FA",
                newName: "Verificacion2Fa");

            migrationBuilder.RenameIndex(
                name: "IX_Verificacion2FA_UsuarioId",
                table: "Verificacion2Fa",
                newName: "IX_Verificacion2Fa_UsuarioId");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Usuario",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Usuario",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verificacion2Fa",
                table: "Verificacion2Fa",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Verificacion2Fa_Usuario_UsuarioId",
                table: "Verificacion2Fa",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verificacion2Fa_Usuario_UsuarioId",
                table: "Verificacion2Fa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Verificacion2Fa",
                table: "Verificacion2Fa");

            migrationBuilder.DropIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Usuario");

            migrationBuilder.RenameTable(
                name: "Verificacion2Fa",
                newName: "Verificacion2FA");

            migrationBuilder.RenameIndex(
                name: "IX_Verificacion2Fa_UsuarioId",
                table: "Verificacion2FA",
                newName: "IX_Verificacion2FA_UsuarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Verificacion2FA",
                table: "Verificacion2FA",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_UsuarioId",
                table: "Cliente",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Verificacion2FA_Usuario_UsuarioId",
                table: "Verificacion2FA",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
