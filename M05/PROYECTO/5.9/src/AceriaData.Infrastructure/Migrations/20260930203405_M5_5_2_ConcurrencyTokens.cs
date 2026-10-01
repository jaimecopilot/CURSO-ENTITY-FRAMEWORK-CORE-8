using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceriaData.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class M5_5_2_ConcurrencyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PlanchasAcero",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OrdenesFabricacion",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "EstadoDetalle",
                table: "DetallesOrden",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pendiente");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Aleaciones",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PlanchasAcero");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OrdenesFabricacion");

            migrationBuilder.DropColumn(
                name: "EstadoDetalle",
                table: "DetallesOrden");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Aleaciones");
        }
    }
}
