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
            "Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;");
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
    public static async Task Main()
    {
        using (var context = AceriaDbContextFactory.Create())
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        DemostrarValorDevuelto();
        DemostrarAtomicidadSaveChanges();
        DemostrarPropagacionDeClaves();
        DemostrarManejoDeErrores();
        await DemostrarSaveChangesAsync();
        DemostrarUnidadDeTrabajo();
    
        InsertarOrdenConPlanchas();
}

    public static void DemostrarValorDevuelto()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden1 = new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", FechaCreacion = DateTime.Now };
        var orden2 = new OrdenFabricacion { NumeroOrden = "OF-002", Cliente = "Constructora del Sur", FechaCreacion = DateTime.Now };

        context.OrdenesFabricacion.AddRange(orden1, orden2);
        var filas = context.SaveChanges();

        Console.WriteLine($"Filas afectadas en la inserción: {filas}");

        orden1.Cliente = "Constructora del Norte Modificada";
        var filasUpdate = context.SaveChanges();

        Console.WriteLine($"Filas afectadas en la actualización: {filasUpdate}");
    }

    public static void DemostrarAtomicidadSaveChanges()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion { NumeroOrden = "OF-003", Cliente = "Constructora del Este", FechaCreacion = DateTime.Now };
        context.OrdenesFabricacion.Add(orden);

        var planchaInvalida = new PlanchaAcero { OrdenId = 9999, Espesor = 10.5, Ancho = 1500, Largo = 3000 };
        context.PlanchasAcero.Add(planchaInvalida);

        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error capturado: {ex.InnerException?.Message}");
        }

        var totalOrdenes = context.OrdenesFabricacion.Count();
        Console.WriteLine($"Órdenes en la base de datos tras el error: {totalOrdenes}");
    }

    public static void DemostrarPropagacionDeClaves()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-004",
            Cliente = "Constructora del Oeste",
            FechaCreacion = DateTime.Now
        };

        context.OrdenesFabricacion.Add(orden);
        context.SaveChanges();

        Console.WriteLine($"Id generado para la orden: {orden.Id}");

        var plancha = new PlanchaAcero
        {
            OrdenId = orden.Id,
            Espesor = 10.5,
            Ancho = 1500,
            Largo = 3000
        };

        context.PlanchasAcero.Add(plancha);
        context.SaveChanges();

        Console.WriteLine($"Id generado para la plancha: {plancha.Id}");
        Console.WriteLine($"Clave foránea de la plancha: {plancha.OrdenId}");
    }

    public static void DemostrarManejoDeErrores()
    {
        using var context = AceriaDbContextFactory.Create();

        var planchaInvalida = new PlanchaAcero { OrdenId = 999999, Espesor = 8.0, Ancho = 1000, Largo = 2000 };
        context.PlanchasAcero.Add(planchaInvalida);

        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Error de base de datos: {ex.InnerException?.Message}");
        }
    }

    public static async Task DemostrarSaveChangesAsync()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion { NumeroOrden = "OF-005", Cliente = "Constructora Asíncrona", FechaCreacion = DateTime.Now };
        context.OrdenesFabricacion.Add(orden);

        var filas = await context.SaveChangesAsync();
        Console.WriteLine($"Filas afectadas con SaveChangesAsync: {filas}");
    }

    public static void DemostrarUnidadDeTrabajo()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-006",
            Cliente = "Constructora Unidad",
            FechaCreacion = DateTime.Now
        };

        var plancha1 = new PlanchaAcero { Espesor = 10.5, Ancho = 1500, Largo = 3000, Orden = orden };
        var plancha2 = new PlanchaAcero { Espesor = 12.0, Ancho = 1200, Largo = 2500, Orden = orden };

        context.OrdenesFabricacion.Add(orden);
        context.PlanchasAcero.AddRange(plancha1, plancha2);

        var filas = context.SaveChanges();

        Console.WriteLine($"Filas afectadas en la unidad de trabajo: {filas}");
        Console.WriteLine($"Orden Id: {orden.Id}, Plancha 1 Id: {plancha1.Id}, Plancha 2 Id: {plancha2.Id}");
    }


    public static void InsertarOrdenConPlanchas()
    {
        using var context = AceriaDbContextFactory.Create();
    
        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-007",
            Cliente = "Constructora con Planchas",
            FechaCreacion = DateTime.Now,
            Planchas = new List<PlanchaAcero>
            {
                new PlanchaAcero { Espesor = 10.5, Ancho = 1500, Largo = 3000 },
                new PlanchaAcero { Espesor = 12.0, Ancho = 1200, Largo = 2500 }
            }
        };
    
        context.OrdenesFabricacion.Add(orden);
        var filas = context.SaveChanges();
    
        Console.WriteLine($"Filas afectadas: {filas}");
        Console.WriteLine($"Orden Id: {orden.Id}");
        foreach (var plancha in orden.Planchas)
        {
            Console.WriteLine($"Plancha Id: {plancha.Id}, OrdenId: {plancha.OrdenId}");
        }
    }
}
