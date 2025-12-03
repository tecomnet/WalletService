using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.DOM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsuarioEstatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing data to a valid enum value (e.g., RegistroCompletado = 8)
            migrationBuilder.Sql("UPDATE Usuario SET Estatus = '8' WHERE Estatus = 'Activo'");
            // Handle other potential string values if necessary, or set a default
            migrationBuilder.Sql(
                "UPDATE Usuario SET Estatus = '1' WHERE Estatus NOT IN ('8')"); // Default to PreRegistro (1) if unknown

            migrationBuilder.AlterColumn<int>(
                name: "Estatus",
                table: "Usuario",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Estatus",
                table: "Usuario",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
