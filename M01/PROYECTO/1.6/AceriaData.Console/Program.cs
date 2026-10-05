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
        InsertarOrden("OF-003", "Constructora del Este");

        ListarOrdenes();

        var ordenActualizada = ActualizarCliente("OF-002", "Constructora del Oeste");
        Console.WriteLine($"Actualizada: {ordenActualizada}");

        var existe = ExisteOrden("OF-003");
        Console.WriteLine($"Existe OF-003: {existe}");

        var total = ContarOrdenes();
        Console.WriteLine($"Total de órdenes: {total}");

        var encontrada = BuscarPorId(1);
        Console.WriteLine($"Orden con Id 1: {encontrada?.NumeroOrden}");

        EliminarOrden("OF-001");
        ListarOrdenes();

        // Reto resuelto del punto 1.6.
        var insertada = InsertarPlancha("OF-002", 10.5, 1500, 3000);
        Console.WriteLine($"Plancha insertada: {insertada}");

        // Diagnóstico del paso 8: debe devolver false sin excepción.
        // var eliminada = EliminarOrden("OF-999");
        // Console.WriteLine($"OF-999 eliminada: {eliminada}");

        // ============================================================
        // PASO 6 - CACHÉ DE IDENTIDAD CON Find
        // Para probar esta variante, comenta el flujo activo de Main y
        // descomenta el bloque siguiente.
        // ============================================================
        /*
        using (var context = AceriaDbContextFactory.Create())
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        InsertarOrden("OF-001", "Constructora del Norte");

        using (var context = AceriaDbContextFactory.Create())
        {
            var orden1 = context.OrdenesFabricacion.Find(1);
            var orden2 = context.OrdenesFabricacion.Find(1);

            Console.WriteLine($"Misma instancia: {ReferenceEquals(orden1, orden2)}");
        }
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
    }

    public static void ListarOrdenes()
    {
        using var context = AceriaDbContextFactory.Create();
        var ordenes = context.OrdenesFabricacion
            .OrderBy(o => o.Id)
            .ToList();

        Console.WriteLine("--- Órdenes ---");

        foreach (var orden in ordenes)
        {
            Console.WriteLine(
                $"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
        }

        // ============================================================
        // PASO 4 - CONSULTA AsNoTracking
        // Sustituye la consulta anterior por esta versión para comprobar
        // que el Change Tracker queda vacío.
        // ============================================================
        /*
        var ordenes = context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .ToList();

        Console.WriteLine("--- Órdenes ---");

        foreach (var orden in ordenes)
        {
            Console.WriteLine(
                $"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
        }

        Console.WriteLine(
            $"Entidades rastreadas: {context.ChangeTracker.Entries().Count()}");
        */
    }

    public static bool ActualizarCliente(string numero, string nuevoCliente)
    {
        using var context = AceriaDbContextFactory.Create();
        var orden = context.OrdenesFabricacion
            .FirstOrDefault(o => o.NumeroOrden == numero);

        if (orden is null)
        {
            return false;
        }

        orden.Cliente = nuevoCliente;
        context.SaveChanges();
        return true;
    }

    public static bool ExisteOrden(string numero)
    {
        using var context = AceriaDbContextFactory.Create();
        return context.OrdenesFabricacion.Any(o => o.NumeroOrden == numero);
    }

    public static int ContarOrdenes()
    {
        using var context = AceriaDbContextFactory.Create();
        return context.OrdenesFabricacion.Count();
    }

    public static OrdenFabricacion? BuscarPorId(int id)
    {
        using var context = AceriaDbContextFactory.Create();
        return context.OrdenesFabricacion.Find(id);
    }

    public static bool EliminarOrden(string numero)
    {
        using var context = AceriaDbContextFactory.Create();
        var orden = context.OrdenesFabricacion
            .FirstOrDefault(o => o.NumeroOrden == numero);

        if (orden is null)
        {
            return false;
        }

        context.OrdenesFabricacion.Remove(orden);
        context.SaveChanges();
        return true;
    }

    public static bool InsertarPlancha(
        string numeroOrden,
        double espesor,
        double ancho,
        double largo)
    {
        using var context = AceriaDbContextFactory.Create();

        var orden = context.OrdenesFabricacion
            .FirstOrDefault(o => o.NumeroOrden == numeroOrden);

        if (orden is null)
        {
            return false;
        }

        var plancha = new PlanchaAcero
        {
            OrdenId = orden.Id,
            Espesor = espesor,
            Ancho = ancho,
            Largo = largo
        };

        context.PlanchasAcero.Add(plancha);
        context.SaveChanges();
        return true;
    }
}
