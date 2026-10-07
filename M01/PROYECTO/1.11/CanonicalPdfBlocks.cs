// ============================================================================
// M01 1.11 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.11 - BLOCK 01 - Paso 4: VARIANTE CONCEPTUAL — NO EJECUTAR EN M01 — Sustituir el contenido de Program.cs
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
//     private readonly string _provider;
//
//     public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
//     public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
//     public DbSet<Aleacion> Aleaciones { get; set; } = null!;
//     public DbSet<EstadoOrden> EstadosOrden { get; set; } = null!;
//
//     public AceriaDbContext(string connectionString, string provider)
//     {
//         _connectionString = connectionString;
//         _provider = provider;
//     }
//
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         if (!optionsBuilder.IsConfigured)
//         {
//             switch (_provider)
//             {
//                 case "Sqlite":
//                     optionsBuilder
//                         .UseSqlite(_connectionString)
//                         .LogTo(
//                             Console.WriteLine,
//                             new[] { "Microsoft.EntityFrameworkCore.Database.Command" },
//                             LogLevel.Information)
//                         .EnableSensitiveDataLogging()
//                         .EnableDetailedErrors();
//                     break;
//
//                 case "SqlServer":
//                 default:
//                     optionsBuilder
//                         .UseSqlServer(_connectionString, sqlOptions =>
//                         {
//                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//                             sqlOptions.CommandTimeout(60);
//                         })
//                         .LogTo(
//                             Console.WriteLine,
//                             new[] { "Microsoft.EntityFrameworkCore.Database.Command" },
//                             LogLevel.Information)
//                         .EnableSensitiveDataLogging()
//                         .EnableDetailedErrors();
//                     break;
//             }
//         }
//     }
// }
//
// public static class AceriaDbContextFactory
// {
//     private static string? _connectionString;
//     private static string? _provider;
//
//     public static void Initialize(string connectionString, string provider)
//     {
//         _connectionString = connectionString;
//         _provider = provider;
//     }
//
//     public static AceriaDbContext Create()
//     {
//         if (_connectionString is null || _provider is null)
//         {
//             throw new InvalidOperationException("La fábrica no ha sido inicializada. Llama a Initialize primero.");
//         }
//         return new AceriaDbContext(_connectionString, _provider);
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
//         var provider = configuration["Database:Provider"] ?? "SqlServer";
//
//         var connectionString = provider == "Sqlite"
//             ? "Data Source=aceria.db"
//             : configuration.GetConnectionString("AceriaDB")
//                 ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'AceriaDB'.");
//
//         AceriaDbContextFactory.Initialize(connectionString, provider);
//
//         Console.WriteLine($"Proveedor configurado: {provider}");
//         Console.WriteLine($"Cadena de conexión: {connectionString}");
//
//         using (var context = AceriaDbContextFactory.Create())
//         {
//             context.Database.EnsureDeleted();
//             context.Database.EnsureCreated();
//         }
//
//         InsertarOrden("OF-001", "Constructora del Norte");
//         InsertarOrden("OF-002", "Constructora del Sur");
//         ListarOrdenes();
//
//         MostrarInformacionDelProveedor();
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
//
//     public static void MostrarInformacionDelProveedor()
//     {
//         using var context = AceriaDbContextFactory.Create();
//         Console.WriteLine($"Proveedor activo: {context.Database.ProviderName}");
//         Console.WriteLine($"Puede conectar: {context.Database.CanConnect()}");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.11 - BLOCK 02 - Paso 2: Añadir el método ProbarConSqliteEnMemoria:
// ----------------------------------------------------------------------------
// public static void ProbarConSqliteEnMemoria()
// {
//     var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
//     connection.Open();
//
//     var options = new DbContextOptionsBuilder<AceriaDbContext>()
//         .UseSqlite(connection)
//         .Options;
//
//     using (var context = new AceriaDbContext("Data Source=:memory:", "Sqlite"))
//     {
//         context.Database.EnsureCreated();
//
//         var orden = new OrdenFabricacion { NumeroOrden = "TEST-001", Cliente = "Cliente de Prueba", FechaCreacion = DateTime.Now };
//         context.OrdenesFabricacion.Add(orden);
//         context.SaveChanges();
//
//         var ordenes = context.OrdenesFabricacion.ToList();
//         Console.WriteLine($"Órdenes en memoria: {ordenes.Count}");
//         Console.WriteLine($"Primera orden: {ordenes[0].NumeroOrden}");
//     }
//
//     connection.Close();
// }
// ============================================================================

// CANONICAL PDF M01 1.11 - BLOCK 03 - Paso 3: Llamar al método desde Main:
// ----------------------------------------------------------------------------
// ProbarConSqliteEnMemoria();
// ============================================================================

