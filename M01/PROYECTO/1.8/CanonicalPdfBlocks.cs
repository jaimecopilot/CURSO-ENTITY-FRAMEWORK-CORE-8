// ============================================================================
// M01 1.8 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.8 - BLOCK 01 - Paso 2: Sustituir el contenido de Program.cs
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
//         InsertarOrdenesIniciales();
//
//         DemostrarAdd();
//         DemostrarAddRange();
//         DemostrarUpdate();
//         DemostrarAttach();
//         DemostrarEntry();
//         DemostrarRemove();
//         DemostrarEntryProperty();
//     }
//
//     public static void InsertarOrdenesIniciales()
//     {
//         using var context = AceriaDbContextFactory.Create();
//         var ordenes = new List<OrdenFabricacion>
//         {
//             new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", FechaCreacion = DateTime.Now },
//             new OrdenFabricacion { NumeroOrden = "OF-002", Cliente = "Constructora del Sur", FechaCreacion = DateTime.Now },
//             new OrdenFabricacion { NumeroOrden = "OF-003", Cliente = "Constructora del Este", FechaCreacion = DateTime.Now }
//         };
//         context.OrdenesFabricacion.AddRange(ordenes);
//         context.SaveChanges();
//     }
//
//     public static void DemostrarAdd()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var orden = new OrdenFabricacion
//         {
//             NumeroOrden = "OF-004",
//             Cliente = "Constructora del Oeste",
//             FechaCreacion = DateTime.Now
//         };
//
//         Console.WriteLine($"Estado antes de Add: {context.Entry(orden).State}");
//         context.OrdenesFabricacion.Add(orden);
//         Console.WriteLine($"Estado tras Add: {context.Entry(orden).State}");
//         context.SaveChanges();
//         Console.WriteLine($"Estado tras SaveChanges: {context.Entry(orden).State}");
//     }
//
//     public static void DemostrarAddRange()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var ordenes = new List<OrdenFabricacion>
//         {
//             new OrdenFabricacion { NumeroOrden = "OF-005", Cliente = "Constructora del Norte", FechaCreacion = DateTime.Now },
//             new OrdenFabricacion { NumeroOrden = "OF-006", Cliente = "Constructora del Sur", FechaCreacion = DateTime.Now }
//         };
//
//         context.OrdenesFabricacion.AddRange(ordenes);
//         Console.WriteLine($"Estado tras AddRange: {context.ChangeTracker.Entries().Count()} entidades rastreadas");
//         context.SaveChanges();
//         Console.WriteLine($"Órdenes insertadas: {ordenes.Count}");
//     }
//
//     public static void DemostrarUpdate()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var orden = new OrdenFabricacion
//         {
//             Id = 1,
//             NumeroOrden = "OF-001",
//             Cliente = "Constructora del Norte Actualizada con Update",
//             FechaCreacion = DateTime.Now
//         };
//
//         context.OrdenesFabricacion.Update(orden);
//         Console.WriteLine($"Estado tras Update: {context.Entry(orden).State}");
//         Console.WriteLine($"SQL generado: {context.OrdenesFabricacion.Where(o => o.Id == 1).ToQueryString()}");
//         context.SaveChanges();
//         Console.WriteLine($"Estado tras SaveChanges: {context.Entry(orden).State}");
//     }
//
//     public static void DemostrarAttach()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var orden = new OrdenFabricacion
//         {
//             Id = 2,
//             NumeroOrden = "OF-002",
//             Cliente = "Constructora del Sur",
//             FechaCreacion = DateTime.Now
//         };
//
//         context.OrdenesFabricacion.Attach(orden);
//         Console.WriteLine($"Estado tras Attach: {context.Entry(orden).State}");
//
//         orden.Cliente = "Constructora del Sur Actualizada con Attach";
//         context.ChangeTracker.DetectChanges();
//         Console.WriteLine($"Estado tras modificar: {context.Entry(orden).State}");
//         context.SaveChanges();
//         Console.WriteLine($"Estado tras SaveChanges: {context.Entry(orden).State}");
//     }
//
//     public static void DemostrarEntry()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var orden = new OrdenFabricacion
//         {
//             Id = 3,
//             NumeroOrden = "OF-003",
//             Cliente = "Constructora del Este",
//             FechaCreacion = DateTime.Now
//         };
//
//         var entry = context.Entry(orden);
//         Console.WriteLine($"Estado inicial: {entry.State}");
//
//         entry.State = EntityState.Modified;
//         Console.WriteLine($"Estado tras cambiar a Modified: {entry.State}");
//
//         context.SaveChanges();
//         Console.WriteLine($"Estado tras SaveChanges: {entry.State}");
//     }
//
//     public static void DemostrarRemove()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var orden = context.OrdenesFabricacion.FirstOrDefault(o => o.NumeroOrden == "OF-004")!;
//         Console.WriteLine($"Estado antes de Remove: {context.Entry(orden).State}");
//
//         context.OrdenesFabricacion.Remove(orden);
//         Console.WriteLine($"Estado tras Remove: {context.Entry(orden).State}");
//
//         context.SaveChanges();
//         Console.WriteLine($"Estado tras SaveChanges: {context.Entry(orden).State}");
//     }
//
//     public static void DemostrarEntryProperty()
//     {
//         using var context = AceriaDbContextFactory.Create();
//
//         var orden = context.OrdenesFabricacion.FirstOrDefault(o => o.NumeroOrden == "OF-005")!;
//         var entry = context.Entry(orden);
//
//         Console.WriteLine($"Estado inicial: {entry.State}");
//         Console.WriteLine($"Cliente original: {entry.Property(o => o.Cliente).OriginalValue}");
//
//         entry.Property(o => o.Cliente).CurrentValue = "Constructora del Norte Modificada con Entry";
//         entry.Property(o => o.Cliente).IsModified = true;
//
//         Console.WriteLine($"Estado tras modificar propiedad: {entry.State}");
//         Console.WriteLine($"Cliente actual: {entry.Property(o => o.Cliente).CurrentValue}");
//
//         context.SaveChanges();
//         Console.WriteLine($"Estado tras SaveChanges: {entry.State}");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.8 - BLOCK 02 - Paso 4: Observar la diferencia entre Update y Attach
// ----------------------------------------------------------------------------
// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// {
//     optionsBuilder
//         .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;")
//         .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
// }
// ============================================================================

// CANONICAL PDF M01 1.8 - BLOCK 03 - Paso 6: Diagnosticar un error común
// ----------------------------------------------------------------------------
// context.OrdenesFabricacion.Add(orden);
// context.OrdenesFabricacion.Add(orden);
// ============================================================================

// CANONICAL PDF M01 1.8 - BLOCK 04 - Paso 1: Añadir el método ActualizarSoloCliente:
// ----------------------------------------------------------------------------
// public static void ActualizarSoloCliente(int id, string nuevoCliente)
// {
//     using var context = AceriaDbContextFactory.Create();
//
//     var orden = new OrdenFabricacion { Id = id };
//     context.OrdenesFabricacion.Attach(orden);
//
//     orden.Cliente = nuevoCliente;
//     context.Entry(orden).Property(o => o.Cliente).IsModified = true;
//
//     context.SaveChanges();
//     Console.WriteLine($"Cliente actualizado para la orden {id}");
// }
// ============================================================================

// CANONICAL PDF M01 1.8 - BLOCK 05 - Paso 2: Llamar al método desde Main:
// ----------------------------------------------------------------------------
// ActualizarSoloCliente(5, "Constructora del Norte Modificada Solo Cliente");
// ============================================================================

