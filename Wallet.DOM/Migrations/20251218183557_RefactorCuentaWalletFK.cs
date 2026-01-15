using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCuentaWalletFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentaWallet_Cliente_IdCliente",
                table: "CuentaWallet");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Cliente_Guid",
                table: "Cliente");

            migrationBuilder.DropIndex(
                name: "IX_CuentaWallet_IdCliente",
                table: "CuentaWallet");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "CuentaWallet");

            migrationBuilder.AddColumn<int>(
                name: "IdCliente",
                table: "CuentaWallet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentaWallet_Cliente_IdCliente",
                table: "CuentaWallet",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_CuentaWallet_IdCliente",
                table: "CuentaWallet",
                column: "IdCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentaWallet_Cliente_IdCliente",
                table: "CuentaWallet");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdCliente",
                table: "CuentaWallet",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Cliente_Guid",
                table: "Cliente",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentaWallet_Cliente_IdCliente",
                table: "CuentaWallet",
                column: "IdCliente",
                principalTable: "Cliente",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
