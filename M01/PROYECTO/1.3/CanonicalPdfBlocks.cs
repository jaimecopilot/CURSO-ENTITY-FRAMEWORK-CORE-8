// ============================================================================
// M01 1.3 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.3 - BLOCK 01 - Paso 4: Inspeccionar el DbContext y el proveedor
// ----------------------------------------------------------------------------
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.ConsoleApp;
//
// public class OrdenFabricacion
// {
//     public int Id { get; set; }
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public DateTime FechaCreacion { get; set; }
// }
//
// public class PlanchaAcero
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public double Espesor { get; set; }
//     public double Ancho { get; set; }
//     public double Largo { get; set; }
//     public OrdenFabricacion Orden { get; set; } = null!;
// }
//
// public class AceriaDbContext : DbContext
// {
//     public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
//     public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
//
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         optionsBuilder.UseSqlServer(
//             "Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;");
//     }
// }
//
// public class Program
// {
//     public static void Main()
//     {
//         using var context = new AceriaDbContext();
//
//         Console.WriteLine($"Proveedor: {context.Database.ProviderName}");
//         Console.WriteLine($"Entidades en el modelo: {context.Model.GetEntityTypes().Count()}");
//
//         foreach (var entidad in context.Model.GetEntityTypes())
//         {
//             Console.WriteLine($"  Entidad: {entidad.ClrType.Name} → Tabla: {entidad.GetTableName()}");
//         }
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.3 - BLOCK 02 - Paso 6: Inspeccionar el Change Tracker
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//
//     var orden = new OrdenFabricacion
//     {
//         NumeroOrden = "OF-001",
//         Cliente = "Constructora del Norte",
//         FechaCreacion = DateTime.Now
//     };
//
//     Console.WriteLine($"Estado antes de Add: {context.Entry(orden).State}");
//
//     context.OrdenesFabricacion.Add(orden);
//
//     Console.WriteLine($"Estado después de Add: {context.Entry(orden).State}");
//
//     orden.Cliente = "Constructora del Sur";
//
//     Console.WriteLine($"Estado después de modificar: {context.Entry(orden).State}");
// }
// ============================================================================

// CANONICAL PDF M01 1.3 - BLOCK 03 - Paso 8: Inspeccionar el modelo completo
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//
//     foreach (var entidad in context.Model.GetEntityTypes())
//     {
//         Console.WriteLine($"Entidad: {entidad.ClrType.Name} → Tabla: {entidad.GetTableName()}");
//
//         foreach (var propiedad in entidad.GetProperties())
//         {
//             Console.WriteLine($"  Propiedad: {propiedad.Name} | Columna: {propiedad.GetColumnName()} | Tipo: {propiedad.ClrType.Name}");
//         }
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.3 - BLOCK 04 - Paso 10: Revisar el archivo de migración
// ----------------------------------------------------------------------------
// public partial class InitialCreate : Migration
// {
//     protected override void Up(MigrationBuilder migrationBuilder)
//     {
//         migrationBuilder.CreateTable(
//             name: "OrdenesFabricacion",
//             columns: table => new
//             {
//                 Id = table.Column<int>(type: "int", nullable: false)
//                     .Annotation("SqlServer:Identity", "1, 1"),
//                 NumeroOrden = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                 Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                 FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
//             },
//             constraints: table =>
//             {
//                 table.PrimaryKey("PK_OrdenesFabricacion", x => x.Id);
//             });
//
//         migrationBuilder.CreateTable(
//             name: "PlanchasAcero",
//             columns: table => new
//             {
//                 Id = table.Column<int>(type: "int", nullable: false)
//                     .Annotation("SqlServer:Identity", "1, 1"),
//                 OrdenId = table.Column<int>(type: "int", nullable: false),
//                 Espesor = table.Column<double>(type: "float", nullable: false),
//                 Ancho = table.Column<double>(type: "float", nullable: false),
//                 Largo = table.Column<double>(type: "float", nullable: false)
//             },
//             constraints: table =>
//             {
//                 table.PrimaryKey("PK_PlanchasAcero", x => x.Id);
//                 table.ForeignKey(
//                     name: "FK_PlanchasAcero_OrdenesFabricacion_OrdenId",
//                     column: x => x.OrdenId,
//                     principalTable: "OrdenesFabricacion",
//                     principalColumn: "Id",
//                     onDelete: ReferentialAction.Cascade);
//             });
//     }
//
//     protected override void Down(MigrationBuilder migrationBuilder)
//     {
//         migrationBuilder.DropTable(name: "PlanchasAcero");
//         migrationBuilder.DropTable(name: "OrdenesFabricacion");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.3 - BLOCK 05 - Paso 1: Añadir la entidad Aleacion:
// ----------------------------------------------------------------------------
// public class Aleacion
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public double PorcentajeCarbono { get; set; }
//     public double PorcentajeManganeso { get; set; }
// }
// ============================================================================

// CANONICAL PDF M01 1.3 - BLOCK 06 - Paso 2: Añadir el DbSet al AceriaDbContext:
// ----------------------------------------------------------------------------
// public DbSet<Aleacion> Aleaciones { get; set; } = null!;
// ============================================================================

