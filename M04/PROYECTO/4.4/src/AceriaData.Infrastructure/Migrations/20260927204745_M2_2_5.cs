using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceriaData.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class M2_2_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdenesAleaciones",
                columns: table => new
                {
                    OrdenFabricacionId = table.Column<int>(type: "int", nullable: false),
                    AleacionId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CantidadUtilizada = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    EstadoRelacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activa")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesAleaciones", x => new { x.OrdenFabricacionId, x.AleacionId });
                    table.ForeignKey(
                        name: "FK_OrdenesAleaciones_Aleaciones_AleacionId",
                        column: x => x.AleacionId,
                        principalTable: "Aleaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesAleaciones_OrdenesFabricacion_OrdenFabricacionId",
                        column: x => x.OrdenFabricacionId,
                        principalTable: "OrdenesFabricacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesAleaciones_AleacionId",
                table: "OrdenesAleaciones",
                column: "AleacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenesAleaciones");
        }
    }
}

