using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceriaData.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class M2_2_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PlanchasAcero",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PlanchasAcero",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OrdenesFabricacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrdenesFabricacion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EstadosOrden",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EstadosOrden",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Aleaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Aleaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PlanchasAcero");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PlanchasAcero");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OrdenesFabricacion");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrdenesFabricacion");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EstadosOrden");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EstadosOrden");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Aleaciones");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Aleaciones");
        }
    }
}
