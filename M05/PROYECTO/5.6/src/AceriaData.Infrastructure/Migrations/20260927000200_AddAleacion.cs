using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using AceriaData.Infrastructure.Persistence;

namespace AceriaData.Infrastructure.Migrations;

[DbContext(typeof(AceriaDbContext))]
[Migration("20260927000200_AddAleacion")]
public sealed class AddAleacion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Aleaciones",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PorcentajeCarbono = table.Column<double>(type: "float", nullable: false),
                PorcentajeManganeso = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Aleaciones", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "Aleaciones");
}

