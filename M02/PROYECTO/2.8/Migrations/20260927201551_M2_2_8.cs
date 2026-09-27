using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceriaData.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class M2_2_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrdenesFabricacion_NumeroOrden",
                table: "OrdenesFabricacion");

            migrationBuilder.DropIndex(
                name: "IX_Aleaciones_Codigo",
                table: "Aleaciones");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_OrdenesFabricacion_NumeroOrden",
                table: "OrdenesFabricacion",
                column: "NumeroOrden");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CertificadosCalidad_NumeroCertificado",
                table: "CertificadosCalidad",
                column: "NumeroCertificado");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Aleaciones_Codigo",
                table: "Aleaciones",
                column: "Codigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_OrdenesFabricacion_NumeroOrden",
                table: "OrdenesFabricacion");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CertificadosCalidad_NumeroCertificado",
                table: "CertificadosCalidad");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Aleaciones_Codigo",
                table: "Aleaciones");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesFabricacion_NumeroOrden",
                table: "OrdenesFabricacion",
                column: "NumeroOrden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aleaciones_Codigo",
                table: "Aleaciones",
                column: "Codigo",
                unique: true);
        }
    }
}
