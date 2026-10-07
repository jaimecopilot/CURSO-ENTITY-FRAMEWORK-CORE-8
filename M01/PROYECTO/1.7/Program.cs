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
    public static void Main()
    {
        using (var context = AceriaDbContextFactory.Create())
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        InsertarOrden("OF-001", "Constructora del Norte");
        InsertarOrden("OF-002", "Constructora del Sur");

        ObservarEstados();
        ObservarDeteccionDeCambios();
        ObservarValoresOriginales();
        ObservarEntidadesRastreadas();
        DemostrarUpdate();
        DemostrarAttach();
        DemostrarAsNoTracking();
        DemostrarClear();
    
        ReportarCambios();
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
    }

    public static void ObservarEstados()
    {
        using var context = AceriaDbContextFactory.Create();

        var nueva = new OrdenFabricacion { NumeroOrden = "OF-003", Cliente = "Constructora del Este", FechaCreacion = DateTime.Now };
        Console.WriteLine($"Estado de entidad nueva: {context.Entry(nueva).State}");

        context.OrdenesFabricacion.Add(nueva);
        Console.WriteLine($"Estado tras Add: {context.Entry(nueva).State}");

        context.SaveChanges();
        Console.WriteLine($"Estado tras SaveChanges: {context.Entry(nueva).State}");

        context.OrdenesFabricacion.Remove(nueva);
        Console.WriteLine($"Estado tras Remove: {context.Entry(nueva).State}");
    }

    public static void ObservarDeteccionDeCambios()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = context.OrdenesFabricacion.FirstOrDefault(o => o.NumeroOrden == "OF-001")!;
        Console.WriteLine($"Estado inicial: {context.Entry(orden).State}");

        orden.Cliente = "Constructora del Oeste";
        context.ChangeTracker.DetectChanges();
        Console.WriteLine($"Estado tras modificar Cliente: {context.Entry(orden).State}");
    }

    public static void ObservarValoresOriginales()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = context.OrdenesFabricacion.FirstOrDefault(o => o.NumeroOrden == "OF-001")!;
        orden.Cliente = "Constructora del Oeste";

        var entry = context.Entry(orden);
        Console.WriteLine($"Cliente actual: {entry.CurrentValues["Cliente"]}");
        Console.WriteLine($"Cliente original: {entry.OriginalValues["Cliente"]}");
        Console.WriteLine($"¿Cliente modificado?: {entry.Property(o => o.Cliente).IsModified}");
    }

    public static void ObservarEntidadesRastreadas()
    {
        using var context = AceriaDbContextFactory.Create();

        context.OrdenesFabricacion.ToList();
        context.Aleaciones.ToList();

        Console.WriteLine("Entidades rastreadas:");
        foreach (var entry in context.ChangeTracker.Entries())
        {
            Console.WriteLine($"  {entry.Entity.GetType().Name}: {entry.State}");
        }
    }

    public static void DemostrarUpdate()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion
        {
            Id = 1,
            NumeroOrden = "OF-001",
            Cliente = "Constructora del Norte Actualizada",
            FechaCreacion = DateTime.Now
        };

        context.OrdenesFabricacion.Update(orden);
        Console.WriteLine($"Estado tras Update: {context.Entry(orden).State}");

        context.SaveChanges();
        Console.WriteLine($"Estado tras SaveChanges: {context.Entry(orden).State}");
    }

    public static void DemostrarAttach()
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = new OrdenFabricacion
        {
            Id = 2,
            NumeroOrden = "OF-002",
            Cliente = "Constructora del Sur",
            FechaCreacion = DateTime.Now
        };

        context.OrdenesFabricacion.Attach(orden);
        Console.WriteLine($"Estado tras Attach: {context.Entry(orden).State}");

        orden.Cliente = "Constructora del Sur Actualizada";
        context.ChangeTracker.DetectChanges();
        Console.WriteLine($"Estado tras modificar y detectar: {context.Entry(orden).State}");

        context.SaveChanges();
        Console.WriteLine($"Estado tras SaveChanges: {context.Entry(orden).State}");
    }

    public static void DemostrarAsNoTracking()
    {
        using var context = AceriaDbContextFactory.Create();

        var ordenes = context.OrdenesFabricacion.AsNoTracking().ToList();
        Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
        Console.WriteLine($"Entidades rastreadas: {context.ChangeTracker.Entries().Count()}");
    }

    public static void DemostrarClear()
    {
        using var context = AceriaDbContextFactory.Create();

        context.OrdenesFabricacion.ToList();
        Console.WriteLine($"Entidades rastreadas antes de Clear: {context.ChangeTracker.Entries().Count()}");

        context.ChangeTracker.Clear();
        Console.WriteLine($"Entidades rastreadas después de Clear: {context.ChangeTracker.Entries().Count()}");
    }


    public static void ReportarCambios()
    {
        using var context = AceriaDbContextFactory.Create();
    
        var ordenes = context.OrdenesFabricacion.ToList();
        Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
    
        foreach (var orden in ordenes)
        {
            if (orden.NumeroOrden == "OF-001")
            {
                orden.Cliente = "Constructora del Norte Modificada";
            }
            if (orden.NumeroOrden == "OF-002")
            {
                orden.Cliente = "Constructora del Sur Modificada";
            }
        }
    
        context.ChangeTracker.DetectChanges();
    
        var modificadas = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified)
            .ToList();
    
        Console.WriteLine($"Entidades modificadas: {modificadas.Count}");
    
        foreach (var entry in modificadas)
        {
            var orden = (OrdenFabricacion)entry.Entity;
            Console.WriteLine($"  {orden.NumeroOrden}: {entry.OriginalValues["Cliente"]} → {entry.CurrentValues["Cliente"]}");
        }
    
        context.SaveChanges();
        Console.WriteLine("Cambios guardados.");
    }
}
