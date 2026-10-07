// ============================================================================
// M01 1.4 - BLOQUES C# CANÓNICOS DEL PDF
// Fuente: M01/PRACTICA/M01_PRACTICA_CANONICA.md
// Todos los fragmentos están comentados a propósito para uso pedagógico.
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 01 - Paso 2: Revisar la entidad Aleacion incorporada en 1.3
// ----------------------------------------------------------------------------
// public class Aleacion
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public double PorcentajeCarbono { get; set; }
//     public double PorcentajeManganeso { get; set; }
// }
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 02 - Paso 3: Verificar el DbSet de Aleacion en AceriaDbContext
// ----------------------------------------------------------------------------
// public class AceriaDbContext : DbContext
// {
//     public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
//     public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
//     public DbSet<Aleacion> Aleaciones { get; set; } = null!;
//
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         optionsBuilder.UseSqlServer(
//             "Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 03 - Paso 4: Inspeccionar las propiedades del DbContext
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//
//     Console.WriteLine($"Proveedor: {context.Database.ProviderName}");
//     Console.WriteLine($"Entidades: {context.Model.GetEntityTypes().Count()}");
//
//     foreach (var entidad in context.Model.GetEntityTypes())
//     {
//         Console.WriteLine($"  {entidad.ClrType.Name} → {entidad.GetTableName()}");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 04 - Paso 6: Observar SaveChanges en acción
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var aleacion = new Aleacion
//     {
//         Nombre = "AISI 1045",
//         PorcentajeCarbono = 0.45,
//         PorcentajeManganeso = 0.75
//     };
//
//     context.Aleaciones.Add(aleacion);
//     var filas = context.SaveChanges();
//
//     Console.WriteLine($"Filas afectadas: {filas}");
//     Console.WriteLine($"Aleación insertada con Id {aleacion.Id}");
// }
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 05 - Paso 8: Observar el Change Tracker antes y después de SaveChanges
// ----------------------------------------------------------------------------
// public static void Main()
// {
//     using var context = new AceriaDbContext();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var aleacion = new Aleacion
//     {
//         Nombre = "AISI 1045",
//         PorcentajeCarbono = 0.45,
//         PorcentajeManganeso = 0.75
//     };
//
//     context.Aleaciones.Add(aleacion);
//
//     Console.WriteLine("Antes de SaveChanges:");
//     foreach (var entrada in context.ChangeTracker.Entries())
//     {
//         Console.WriteLine($"  {entrada.Entity.GetType().Name}: {entrada.State}");
//     }
//
//     context.SaveChanges();
//
//     Console.WriteLine("Después de SaveChanges:");
//     foreach (var entrada in context.ChangeTracker.Entries())
//     {
//         Console.WriteLine($"  {entrada.Entity.GetType().Name}: {entrada.State}");
//     }
// }
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 06 - Paso 9: Añadir la entidad EstadoOrden
// ----------------------------------------------------------------------------
// public class EstadoOrden
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public string Descripcion { get; set; } = string.Empty;
// }
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 07 - Paso 9: Añadir la entidad EstadoOrden
// ----------------------------------------------------------------------------
// public DbSet<EstadoOrden> EstadosOrden { get; set; } = null!;
// ============================================================================

// CANONICAL PDF M01 1.4 - BLOCK 08 - Paso 1: Consultar el modelo completo:
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

