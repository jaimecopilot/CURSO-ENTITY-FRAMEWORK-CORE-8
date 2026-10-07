// ============================================================================
// M01 1.10 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.10 - BLOCK 01 - Paso 3: Comprender la fábrica de tiempo de diseño
// ----------------------------------------------------------------------------
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
//
// namespace AceriaData.ConsoleApp;
//
// public sealed class AceriaDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AceriaDbContext>
// {
//     public AceriaDbContext CreateDbContext(string[] args)
//     {
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: false)
//             .AddEnvironmentVariables()
//             .Build();
//
//         var connectionString = configuration.GetConnectionString("AceriaDB")
//             ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'AceriaDB'.");
//
//         return new AceriaDbContext(connectionString);
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.10 - BLOCK 02 - Paso 6: Inspeccionar Up y Down
// ----------------------------------------------------------------------------
// protected override void Up(MigrationBuilder migrationBuilder)
// {
//     migrationBuilder.CreateTable(
//         name: "EstadosOrden",
//         columns: table => new
//         {
//             Id = table.Column<int>(type: "int", nullable: false)
//                 .Annotation("SqlServer:Identity", "1, 1"),
//             Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
//             Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
//         },
//         constraints: table => table.PrimaryKey("PK_EstadosOrden", x => x.Id));
// }
//
// protected override void Down(MigrationBuilder migrationBuilder) =>
//     migrationBuilder.DropTable(name: "EstadosOrden");
// ============================================================================

// CANONICAL PDF M01 1.10 - BLOCK 03 - Paso 11: Relacionar las migraciones con Database.Migrate y el logging
// ----------------------------------------------------------------------------
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
//
// namespace AceriaData.ConsoleApp;
//
// public class OrdenFabricacion
// {
//     public int Id { get; set; }
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public DateTime FechaCreacion { get; set; }
//     public List<PlanchaAcero> Planchas { get; set; } = new();
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
// public class Aleacion
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public double PorcentajeCarbono { get; set; }
//     public double PorcentajeManganeso { get; set; }
// }
//
// public class EstadoOrden
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public string Descripcion { get; set; } = string.Empty;
// }
//
// public class AceriaDbContext : DbContext
// {
//     private readonly string _connectionString;
//
//     public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
//     public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
//     public DbSet<Aleacion> Aleaciones { get; set; } = null!;
//     public DbSet<EstadoOrden> EstadosOrden { get; set; } = null!;
//
//     public AceriaDbContext(string connectionString)
//     {
//         _connectionString = connectionString;
//     }
//
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         if (!optionsBuilder.IsConfigured)
//         {
//             optionsBuilder
//                 .UseSqlServer(_connectionString, sqlOptions =>
//                 {
//                     sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//                     sqlOptions.CommandTimeout(60);
//                 })
//                 .LogTo(
//                     Console.WriteLine,
//                     new[] { "Microsoft.EntityFrameworkCore.Database.Command" },
//                     LogLevel.Information)
//                 .EnableSensitiveDataLogging()
//                 .EnableDetailedErrors();
//         }
//     }
// }
//
// public static class AceriaDbContextFactory
// {
//     private static string? _connectionString;
//
//     public static void Initialize(string connectionString)
//     {
//         _connectionString = connectionString;
//     }
//
//     public static AceriaDbContext Create()
//     {
//         if (_connectionString is null)
//         {
//             throw new InvalidOperationException("La fábrica no ha sido inicializada. Llama a Initialize primero.");
//         }
//         return new AceriaDbContext(_connectionString);
//     }
// }
//
// public class Program
// {
//     public static void Main()
//     {
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//             .AddEnvironmentVariables()
//             .Build();
//
//         var connectionString = configuration.GetConnectionString("AceriaDB")
//             ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'AceriaDB'.");
//
//         AceriaDbContextFactory.Initialize(connectionString);
//
//         using (var context = AceriaDbContextFactory.Create())
//         {
//             context.Database.EnsureDeleted();
//             context.Database.Migrate();
//         }
//
//         InsertarOrden("OF-001", "Constructora del Norte");
//         InsertarOrden("OF-002", "Constructora del Sur");
//         ListarOrdenes();
//     }
//
//     public static void InsertarOrden(string numero, string cliente)
//     {
//         using var context = AceriaDbContextFactory.Create();
//         var orden = new OrdenFabricacion
//         {
//             NumeroOrden = numero,
//             Cliente = cliente,
//             FechaCreacion = DateTime.Now
//         };
//         context.OrdenesFabricacion.Add(orden);
//         context.SaveChanges();
//     }
//
//     public static void ListarOrdenes()
//     {
//         using var context = AceriaDbContextFactory.Create();
//         var ordenes = context.OrdenesFabricacion.OrderBy(o => o.Id).ToList();
//         Console.WriteLine("--- Órdenes ---");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
//         }
//     }
// }
// ============================================================================

