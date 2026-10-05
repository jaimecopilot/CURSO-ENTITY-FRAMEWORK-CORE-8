using Microsoft.EntityFrameworkCore;

namespace AceriaData.ConsoleApp;

public class OrdenFabricacion
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public class AceriaDbContext : DbContext
{
    public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;");
    }
}

public class Program
{
    public static void Main()
    {
        // ============================================================
        // ESTADO FINAL DEL PUNTO 1.2 - TRANSICIÓN A MIGRACIONES
        // El PDF indica eliminar la base creada con EnsureCreated antes
        // de comenzar 1.3 para no mezclar EnsureCreated con migraciones.
        // ============================================================
        using var context = new AceriaDbContext();
        context.Database.EnsureDeleted();

        // ============================================================
        // PASOS 2-3 - CREAR LA BASE CON EnsureCreated
        // Para probarlos, comenta el bloque activo anterior y
        // descomenta este bloque.
        // ============================================================
        // using var context = new AceriaDbContext();
        // context.Database.EnsureCreated();
        // Console.WriteLine("Base de datos AceriaDB creada correctamente en LocalDB.");

        // ============================================================
        // PASO 5 - INSERTAR UNA ORDEN
        // Para probarlo desde una base limpia, comenta el bloque activo
        // y descomenta este bloque.
        // ============================================================
        // using var context = new AceriaDbContext();
        // context.Database.EnsureCreated();
        //
        // var orden = new OrdenFabricacion
        // {
        //     NumeroOrden = "OF-001",
        //     Cliente = "Constructora del Norte",
        //     FechaCreacion = DateTime.Now
        // };
        //
        // context.OrdenesFabricacion.Add(orden);
        // context.SaveChanges();
        // Console.WriteLine($"Orden {orden.NumeroOrden} insertada con Id {orden.Id}.");

        // ============================================================
        // PASO 6 - CONSULTAR LAS ÓRDENES
        // Ejecuta antes el bloque de inserción para disponer de datos.
        // ============================================================
        // using var context = new AceriaDbContext();
        // context.Database.EnsureCreated();
        //
        // var ordenes = context.OrdenesFabricacion.ToList();
        //
        // foreach (var orden in ordenes)
        // {
        //     Console.WriteLine(
        //         $"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente} | Fecha: {orden.FechaCreacion:dd/MM/yyyy}");
        // }

        // ============================================================
        // PASO 7 - DIAGNOSTICAR EL ERROR SIN EnsureCreated
        // El PDF propone retirar EnsureCreated para observar el fallo
        // cuando la tabla todavía no existe.
        // ============================================================
        // using var context = new AceriaDbContext();
        // var ordenes = context.OrdenesFabricacion.ToList();
    }
}
