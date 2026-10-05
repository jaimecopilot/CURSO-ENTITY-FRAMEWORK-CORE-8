using Microsoft.EntityFrameworkCore;

namespace AceriaData.ConsoleApp;

public class OrdenFabricacion
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    // Se conserva la navegación incorporada en 1.3 para que los retos
    // acumulativos con Include continúen siendo ejecutables.
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

public static class AceriaDbContextFactory
{
    public static AceriaDbContext Create()
    {
        return new AceriaDbContext();
    }
}

public class Program
{
    public static void Main()
    {
        // ============================================================
        // ESTADO ACUMULATIVO PRINCIPAL DEL PUNTO 1.5
        // Cada operación usa un DbContext distinto y de vida corta.
        // ============================================================
        using (var context = AceriaDbContextFactory.Create())
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        InsertarOrden("OF-001", "Constructora del Norte");
        InsertarOrden("OF-002", "Constructora del Sur");
        ListarOrdenes();

        // Reto resuelto del PDF: una orden y dos planchas dentro
        // de una única unidad de trabajo.
        InsertarOrdenConPlanchas();

        // ============================================================
        // PASO 6 - PROBLEMA DEL DbContext COMPARTIDO
        // Para probar esta variante, comenta el contenido activo de Main
        // y descomenta el bloque completo siguiente.
        // ============================================================
        /*
        using (var context = AceriaDbContextFactory.Create())
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        using var contextCompartido = AceriaDbContextFactory.Create();

        var orden1 = new OrdenFabricacion
        {
            NumeroOrden = "OF-001",
            Cliente = "Constructora del Norte",
            FechaCreacion = DateTime.Now
        };

        var orden2 = new OrdenFabricacion
        {
            NumeroOrden = "OF-002",
            Cliente = "Constructora del Sur",
            FechaCreacion = DateTime.Now
        };

        contextCompartido.OrdenesFabricacion.Add(orden1);
        contextCompartido.OrdenesFabricacion.Add(orden2);
        contextCompartido.SaveChanges();

        Console.WriteLine(
            $"Entidades en el Change Tracker: {contextCompartido.ChangeTracker.Entries().Count()}");

        var ordenes = contextCompartido.OrdenesFabricacion.ToList();
        Console.WriteLine($"Órdenes recuperadas: {ordenes.Count}");
        */
    }

    public static void InsertarOrden(string numero, string cliente)
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = numero,
            Cliente = cliente,
            FechaCreacion = DateTime.Now
        };

        context.OrdenesFabricacion.Add(orden);
        context.SaveChanges();

        // ============================================================
        // PASO 4 - OBSERVAR EL CHANGE TRACKER
        // Sustituye las dos líneas anteriores Add/SaveChanges por este
        // bloque comentado para reproducir exactamente la variante.
        // ============================================================
        /*
        context.OrdenesFabricacion.Add(orden);

        Console.WriteLine(
            $"Entidades rastreadas antes de SaveChanges: {context.ChangeTracker.Entries().Count()}");

        context.SaveChanges();

        Console.WriteLine(
            $"Entidades rastreadas después de SaveChanges: {context.ChangeTracker.Entries().Count()}");
        */
    }

    public static void ListarOrdenes()
    {
        using var context = AceriaDbContextFactory.Create();
        var ordenes = context.OrdenesFabricacion.ToList();

        foreach (var orden in ordenes)
        {
            Console.WriteLine(
                $"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
        }
    }

    public static void InsertarOrdenConPlanchas()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-003",
            Cliente = "Constructora del Este",
            FechaCreacion = DateTime.Now
        };

        context.OrdenesFabricacion.Add(orden);
        context.SaveChanges();

        var plancha1 = new PlanchaAcero
        {
            OrdenId = orden.Id,
            Espesor = 10.5,
            Ancho = 1500,
            Largo = 3000
        };

        var plancha2 = new PlanchaAcero
        {
            OrdenId = orden.Id,
            Espesor = 12.0,
            Ancho = 1200,
            Largo = 2500
        };

        context.PlanchasAcero.AddRange(plancha1, plancha2);
        var filas = context.SaveChanges();

        Console.WriteLine($"Orden {orden.NumeroOrden} insertada con Id {orden.Id}");
        Console.WriteLine($"Planchas insertadas: {filas}");
    }
}
