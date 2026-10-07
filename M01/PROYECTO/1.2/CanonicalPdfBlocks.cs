// ============================================================================
// M01 1.2 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 01 - Paso 2: Sustituir el contenido de Program.cs
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
// public class AceriaDbContext : DbContext
// {
//     public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
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
//         context.Database.EnsureCreated();
//         Console.WriteLine("Base de datos AceriaDB creada correctamente en LocalDB.");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 02 - Paso 5: Insertar una orden desde el programa
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//     context.Database.EnsureCreated();
//
//     var orden = new OrdenFabricacion
//     {
//         NumeroOrden = "OF-001",
//         Cliente = "Constructora del Norte",
//         FechaCreacion = DateTime.Now
//     };
//
//     context.OrdenesFabricacion.Add(orden);
//     context.SaveChanges();
//
//     Console.WriteLine($"Orden {orden.NumeroOrden} insertada con Id {orden.Id}.");
// }
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 03 - Paso 6: Consultar las órdenes
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//     context.Database.EnsureCreated();
//
//     var ordenes = context.OrdenesFabricacion.ToList();
//
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente} | Fecha: {orden.FechaCreacion:dd/MM/yyyy}");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 04 - Paso 8: Insertar una plancha asociada a una orden
// ----------------------------------------------------------------------------
// public class PlanchaAcero
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public double Espesor { get; set; }
//     public double Ancho { get; set; }
//     public double Largo { get; set; }
//     public OrdenFabricacion Orden { get; set; } = null!;
// }
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 05 - Paso 9: Añadir el DbSet de planchas
// ----------------------------------------------------------------------------
// public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 06 - Paso 10: Insertar una plancha asociada
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
//
//     var orden = new OrdenFabricacion
//     {
//         NumeroOrden = "OF-001",
//         Cliente = "Constructora del Norte",
//         FechaCreacion = DateTime.Now
//     };
//
//     context.OrdenesFabricacion.Add(orden);
//     context.SaveChanges();
//
//     var plancha = new PlanchaAcero
//     {
//         OrdenId = orden.Id,
//         Espesor = 10.5,
//         Ancho = 1500,
//         Largo = 3000
//     };
//
//     context.PlanchasAcero.Add(plancha);
//     context.SaveChanges();
//
//     Console.WriteLine($"Plancha insertada con Id {plancha.Id} para la orden {plancha.OrdenId}.");
// }
// ============================================================================

// CANONICAL PDF M01 1.2 - BLOCK 07 - Paso 1: Modificar Program.cs:
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
//
//     var orden = new OrdenFabricacion
//     {
//         NumeroOrden = "OF-001",
//         Cliente = "Constructora del Norte",
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
//     context.SaveChanges();
//
//     var ordenRecuperada = context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .FirstOrDefault(o => o.Id == orden.Id);
//
//     Console.WriteLine($"Orden: {ordenRecuperada!.NumeroOrden}");
//     foreach (var plancha in ordenRecuperada.Planchas)
//     {
//         Console.WriteLine($"  Plancha Id {plancha.Id} | Espesor: {plancha.Espesor} | Ancho: {plancha.Ancho} | Largo: {plancha.Largo}");
//     }
// }
// ============================================================================

