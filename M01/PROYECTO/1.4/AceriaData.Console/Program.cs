using Microsoft.EntityFrameworkCore;

namespace AceriaData.ConsoleApp;

public class OrdenFabricacion
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public List<PlanchaAcero> Planchas { get; set; } = new();
}

public class PlanchaAcero
{
    public int Id { get; set; }
    public int OrdenId { get; set; }
    public double Espesor { get; set; }
    public double Ancho { get; set; }
    public double Largo { get; set; }
    public OrdenFabricacion Orden { get; set; } = null!;
}

public class Aleacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public double PorcentajeCarbono { get; set; }
    public double PorcentajeManganeso { get; set; }
}

public class EstadoOrden
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class AceriaDbContext : DbContext
{
    public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
    public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
    public DbSet<Aleacion> Aleaciones { get; set; } = null!;
    public DbSet<EstadoOrden> EstadosOrden { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;");
    }
}

public class Program
{
    public static void Main()
    {
        // ============================================================
        // RETO RESUELTO / ESTADO FINAL DEL PUNTO 1.4
        // Inspeccionar el modelo completo después de AddEstadoOrden.
        // ============================================================
        using var context = new AceriaDbContext();

        foreach (var entidad in context.Model.GetEntityTypes())
        {
            Console.WriteLine(
                $"Entidad: {entidad.ClrType.Name} → Tabla: {entidad.GetTableName()}");

            foreach (var propiedad in entidad.GetProperties())
            {
                Console.WriteLine(
                    $" Propiedad: {propiedad.Name} | Columna: {propiedad.GetColumnName()} | Tipo: {propiedad.ClrType.Name}");
            }
        }

        // ============================================================
        // PASOS 4-5 - DATABASE Y MODEL
        // Comenta el bloque activo y descomenta este para repetirlo.
        // ============================================================
        // using var context = new AceriaDbContext();
        // Console.WriteLine($"Proveedor: {context.Database.ProviderName}");
        // Console.WriteLine($"Entidades: {context.Model.GetEntityTypes().Count()}");
        // foreach (var entidad in context.Model.GetEntityTypes())
        // {
        //     Console.WriteLine($" {entidad.ClrType.Name} → {entidad.GetTableName()}");
        // }

        // ============================================================
        // PASOS 6-7 - SaveChanges DEVUELVE FILAS AFECTADAS
        // ============================================================
        // using var context = new AceriaDbContext();
        // context.Database.EnsureDeleted();
        // context.Database.Migrate();
        //
        // var aleacion = new Aleacion
        // {
        //     Nombre = "AISI 1045",
        //     PorcentajeCarbono = 0.45,
        //     PorcentajeManganeso = 0.75
        // };
        //
        // context.Aleaciones.Add(aleacion);
        // var filas = context.SaveChanges();
        //
        // Console.WriteLine($"Filas afectadas: {filas}");
        // Console.WriteLine($"Aleación insertada con Id {aleacion.Id}");

        // ============================================================
        // PASO 8 - CHANGE TRACKER ANTES Y DESPUÉS DE SaveChanges
        // Resultado esperado: Added -> Unchanged.
        // ============================================================
        // using var context = new AceriaDbContext();
        // context.Database.EnsureDeleted();
        // context.Database.Migrate();
        //
        // var aleacion = new Aleacion
        // {
        //     Nombre = "AISI 1045",
        //     PorcentajeCarbono = 0.45,
        //     PorcentajeManganeso = 0.75
        // };
        //
        // context.Aleaciones.Add(aleacion);
        //
        // Console.WriteLine("Antes de SaveChanges:");
        // foreach (var entrada in context.ChangeTracker.Entries())
        // {
        //     Console.WriteLine($" {entrada.Entity.GetType().Name}: {entrada.State}");
        // }
        //
        // context.SaveChanges();
        //
        // Console.WriteLine("Después de SaveChanges:");
        // foreach (var entrada in context.ChangeTracker.Entries())
        // {
        //     Console.WriteLine($" {entrada.Entity.GetType().Name}: {entrada.State}");
        // }
    }
}
