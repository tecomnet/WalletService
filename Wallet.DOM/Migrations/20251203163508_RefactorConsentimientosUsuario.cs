using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorConsentimientosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "ConsentimientosUsuario",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "ConsentimientosUsuario",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTimestamp",
                table: "ConsentimientosUsuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreationUser",
                table: "ConsentimientosUsuario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "ConsentimientosUsuario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ConsentimientosUsuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTimestamp",
                table: "ConsentimientosUsuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationUser",
                table: "ConsentimientosUsuario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TestCaseID",
                table: "ConsentimientosUsuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "CreationTimestamp",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "CreationUser",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "ModificationTimestamp",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "ModificationUser",
                table: "ConsentimientosUsuario");

            migrationBuilder.DropColumn(
                name: "TestCaseID",
                table: "ConsentimientosUsuario");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "ConsentimientosUsuario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
