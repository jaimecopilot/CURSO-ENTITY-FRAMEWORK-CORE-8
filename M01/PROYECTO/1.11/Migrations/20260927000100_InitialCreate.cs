using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using AceriaData.ConsoleApp;

namespace AceriaData.ConsoleApp.Migrations;

[DbContext(typeof(AceriaDbContext))]
[Migration("20260927000100_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OrdenesFabricacion",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                NumeroOrden = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_OrdenesFabricacion", x => x.Id));

        migrationBuilder.CreateTable(
            name: "PlanchasAcero",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrdenId = table.Column<int>(type: "int", nullable: false),
                Espesor = table.Column<double>(type: "float", nullable: false),
                Ancho = table.Column<double>(type: "float", nullable: false),
                Largo = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PlanchasAcero", x => x.Id);
                table.ForeignKey(
                    name: "FK_PlanchasAcero_OrdenesFabricacion_OrdenId",
                    column: x => x.OrdenId,
                    principalTable: "OrdenesFabricacion",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PlanchasAcero_OrdenId",
            table: "PlanchasAcero",
            column: "OrdenId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PlanchasAcero");
        migrationBuilder.DropTable(name: "OrdenesFabricacion");
    }
}
