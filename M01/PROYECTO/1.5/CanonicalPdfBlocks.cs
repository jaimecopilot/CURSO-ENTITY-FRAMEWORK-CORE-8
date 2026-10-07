// ============================================================================
// M01 1.5 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 01 - Paso 2: Sustituir el contenido de Program.cs
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
//     public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
//     public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
//     public DbSet<Aleacion> Aleaciones { get; set; } = null!;
//     public DbSet<EstadoOrden> EstadosOrden { get; set; } = null!;
//
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         optionsBuilder.UseSqlServer(
//             "Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;");
//     }
// }
//
// public static class AceriaDbContextFactory
// {
//     public static AceriaDbContext Create()
//     {
//         return new AceriaDbContext();
//     }
// }
//
// public class Program
// {
//     public static void Main()
//     {
//         using (var context = AceriaDbContextFactory.Create())
//         {
//             context.Database.EnsureDeleted();
//             context.Database.Migrate();
//         }
//
//         InsertarOrden("OF-001", "Constructora del Norte");
//         InsertarOrden("OF-002", "Constructora del Sur");
//
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
//         var ordenes = context.OrdenesFabricacion.ToList();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
//         }
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 02 - Paso 4: Observar el Change Tracker con unidades de trabajo
// ----------------------------------------------------------------------------
// public static void InsertarOrden(string numero, string cliente)
// {
//     using var context = AceriaDbContextFactory.Create();
//     var orden = new OrdenFabricacion
//     {
//         NumeroOrden = numero,
//         Cliente = cliente,
//         FechaCreacion = DateTime.Now
//     };
//     context.OrdenesFabricacion.Add(orden);
//
//     Console.WriteLine($"Entidades rastreadas antes de SaveChanges: {context.ChangeTracker.Entries().Count()}");
//
//     context.SaveChanges();
//
//     Console.WriteLine($"Entidades rastreadas después de SaveChanges: {context.ChangeTracker.Entries().Count()}");
// }
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 03 - Paso 6: Demostrar el problema del DbContext compartido
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using (var context = AceriaDbContextFactory.Create())
//     {
//         context.Database.EnsureDeleted();
//         context.Database.Migrate();
//     }
//
//     using var contextCompartido = AceriaDbContextFactory.Create();
//
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", FechaCreacion = DateTime.Now };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-002", Cliente = "Constructora del Sur", FechaCreacion = DateTime.Now };
//
//     contextCompartido.OrdenesFabricacion.Add(orden1);
//     contextCompartido.OrdenesFabricacion.Add(orden2);
//     contextCompartido.SaveChanges();
//
//     Console.WriteLine($"Entidades en el Change Tracker: {contextCompartido.ChangeTracker.Entries().Count()}");
//
//     var ordenes = contextCompartido.OrdenesFabricacion.ToList();
//     Console.WriteLine($"Órdenes recuperadas: {ordenes.Count}");
// }
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 04 - Paso 8: Verificar EstadoOrden heredado del punto 1.4
// ----------------------------------------------------------------------------
// public class EstadoOrden
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public string Descripcion { get; set; } = string.Empty;
// }
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 05 - Paso 9: Verificar el DbSet
// ----------------------------------------------------------------------------
// public DbSet<EstadoOrden> EstadosOrden { get; set; } = null!;
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 06 - Paso 1: Añadir el método InsertarOrdenConPlanchas:
// ----------------------------------------------------------------------------
// public static void InsertarOrdenConPlanchas()
// {
//     using var context = AceriaDbContextFactory.Create();
//
//     var orden = new OrdenFabricacion
//     {
//         NumeroOrden = "OF-003",
//         Cliente = "Constructora del Este",
//         FechaCreacion = DateTime.Now
//     };
//
//     context.OrdenesFabricacion.Add(orden);
//     context.SaveChanges();
//
//     var plancha1 = new PlanchaAcero { OrdenId = orden.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000 };
//     var plancha2 = new PlanchaAcero { OrdenId = orden.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500 };
//
//     context.PlanchasAcero.AddRange(plancha1, plancha2);
//     var filas = context.SaveChanges();
//
//     Console.WriteLine($"Orden {orden.NumeroOrden} insertada con Id {orden.Id}");
//     Console.WriteLine($"Planchas insertadas: {filas}");
// }
// ============================================================================

// CANONICAL PDF M01 1.5 - BLOCK 07 - Paso 2: Llamar al método desde Main:
// ----------------------------------------------------------------------------
// InsertarOrdenConPlanchas();
// ============================================================================

