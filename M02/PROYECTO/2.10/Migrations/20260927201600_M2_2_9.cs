using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceriaData.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class M2_2_9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlanchasAcero_OrdenId",
                table: "PlanchasAcero");

            migrationBuilder.CreateIndex(
                name: "IX_PlanchasAcero_Espesor_Activas",
                table: "PlanchasAcero",
                column: "Espesor",
                filter: "[Activa] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PlanchasAcero_OrdenId_Activa",
                table: "PlanchasAcero",
                columns: new[] { "OrdenId", "Activa" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_PlanchasAcero_Ancho",
                table: "PlanchasAcero",
                sql: "[Ancho] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PlanchasAcero_Espesor",
                table: "PlanchasAcero",
                sql: "[Espesor] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PlanchasAcero_Largo",
                table: "PlanchasAcero",
                sql: "[Largo] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PlanchasAcero_Peso",
                table: "PlanchasAcero",
                sql: "[Peso] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesFabricacion_Cliente",
                table: "OrdenesFabricacion",
                column: "Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesFabricacion_Cliente_FechaCreacion",
                table: "OrdenesFabricacion",
                columns: new[] { "Cliente", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesFabricacion_Estado_Incluye",
                table: "OrdenesFabricacion",
                column: "Estado")
                .Annotation("SqlServer:Include", new[] { "NumeroOrden", "Cliente", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesFabricacion_FechaEntrega_Pendientes",
                table: "OrdenesFabricacion",
                column: "FechaEntrega",
                filter: "[Estado] = 'Pendiente'");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesAleaciones_EstadoRelacion_Activas",
                table: "OrdenesAleaciones",
                column: "EstadoRelacion",
                filter: "[EstadoRelacion] = 'Activa'");

            migrationBuilder.CreateIndex(
                name: "IX_CertificadosCalidad_FechaEmision",
                table: "CertificadosCalidad",
                column: "FechaEmision");

            migrationBuilder.CreateIndex(
                name: "IX_Aleaciones_Nombre",
                table: "Aleaciones",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_Aleaciones_Porcentajes",
                table: "Aleaciones",
                columns: new[] { "PorcentajeCarbono", "PorcentajeManganeso" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Aleaciones_PorcentajeCarbono",
                table: "Aleaciones",
                sql: "[PorcentajeCarbono] >= 0 AND [PorcentajeCarbono] <= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Aleaciones_PorcentajeManganeso",
                table: "Aleaciones",
                sql: "[PorcentajeManganeso] >= 0 AND [PorcentajeManganeso] <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlanchasAcero_Espesor_Activas",
                table: "PlanchasAcero");

            migrationBuilder.DropIndex(
                name: "IX_PlanchasAcero_OrdenId_Activa",
                table: "PlanchasAcero");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PlanchasAcero_Ancho",
                table: "PlanchasAcero");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PlanchasAcero_Espesor",
                table: "PlanchasAcero");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PlanchasAcero_Largo",
                table: "PlanchasAcero");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PlanchasAcero_Peso",
                table: "PlanchasAcero");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesFabricacion_Cliente",
                table: "OrdenesFabricacion");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesFabricacion_Cliente_FechaCreacion",
                table: "OrdenesFabricacion");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesFabricacion_Estado_Incluye",
                table: "OrdenesFabricacion");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesFabricacion_FechaEntrega_Pendientes",
                table: "OrdenesFabricacion");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesAleaciones_EstadoRelacion_Activas",
                table: "OrdenesAleaciones");

            migrationBuilder.DropIndex(
                name: "IX_CertificadosCalidad_FechaEmision",
                table: "CertificadosCalidad");

            migrationBuilder.DropIndex(
                name: "IX_Aleaciones_Nombre",
                table: "Aleaciones");

            migrationBuilder.DropIndex(
                name: "IX_Aleaciones_Porcentajes",
                table: "Aleaciones");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Aleaciones_PorcentajeCarbono",
                table: "Aleaciones");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Aleaciones_PorcentajeManganeso",
                table: "Aleaciones");

            migrationBuilder.CreateIndex(
                name: "IX_PlanchasAcero_OrdenId",
                table: "PlanchasAcero",
                column: "OrdenId");
        }
    }
}
