using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class RefactorWalletInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_IdTransaccion",
                table: "DetallesPagoServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallesPagoServicio",
                table: "DetallesPagoServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BitacoraTransaccion",
                table: "BitacoraTransaccion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "BitacoraTransaccion",
                newName: "ModificationTimestamp");

            migrationBuilder.AlterColumn<int>(
                name: "IdTransaccion",
                table: "DetallesPagoServicio",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                    name: "Id",
                    table: "DetallesPagoServicio",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "DetallesPagoServicio",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTimestamp",
                table: "DetallesPagoServicio",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreationUser",
                table: "DetallesPagoServicio",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "DetallesPagoServicio",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DetallesPagoServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationTimestamp",
                table: "DetallesPagoServicio",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationUser",
                table: "DetallesPagoServicio",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TestCaseID",
                table: "DetallesPagoServicio",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                    name: "Id",
                    table: "BitacoraTransaccion",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(long),
                    oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<byte[]>(
                name: "ConcurrencyToken",
                table: "BitacoraTransaccion",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTimestamp",
                table: "BitacoraTransaccion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreationUser",
                table: "BitacoraTransaccion",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "BitacoraTransaccion",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BitacoraTransaccion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationUser",
                table: "BitacoraTransaccion",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TestCaseID",
                table: "BitacoraTransaccion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallesPagoServicio",
                table: "DetallesPagoServicio",
                columns: new[] { "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BitacoraTransaccion",
                table: "BitacoraTransaccion",
                columns: new[] { "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_IdTransaccion",
                table: "DetallesPagoServicio",
                column: "IdTransaccion",
                principalTable: "BitacoraTransaccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesPagoServicio_BitacoraTransaccion_IdTransaccion",
                table: "DetallesPagoServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallesPagoServicio",
                table: "DetallesPagoServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BitacoraTransaccion",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "CreationTimestamp",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "CreationUser",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "ModificationTimestamp",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "ModificationUser",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "TestCaseID",
                table: "DetallesPagoServicio");

            migrationBuilder.DropColumn(
                name: "ConcurrencyToken",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "CreationTimestamp",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "CreationUser",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "ModificationUser",
                table: "BitacoraTransaccion");

            migrationBuilder.DropColumn(
                name: "TestCaseID",
                table: "BitacoraTransaccion");

            migrationBuilder.RenameColumn(
                name: "ModificationTimestamp",
                table: "BitacoraTransaccion",
                newName: "FechaCreacion");

            migrationBuilder.AlterColumn<long>(
                name: "IdTransaccion",
                table: "DetallesPagoServicio",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                    name: "Id",
                    table: "DetallesPagoServicio",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                    name: "Id",
                    table: "BitacoraTransaccion",
                    type: "bigint",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallesPagoServicio",
                table: "DetallesPagoServicio",
                columns: new[] { "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BitacoraTransaccion",
                table: "BitacoraTransaccion",
                columns: new[] { "Id" });

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
