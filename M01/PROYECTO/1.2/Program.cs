using Microsoft.EntityFrameworkCore;

namespace AceriaData.ConsoleApp;

public class OrdenFabricacion
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    // NOTA DEL EJERCICIO:
    // el reto 1.2 consulta Include(o => o.Planchas), por lo que la navegación
    // debe existir para que el código del propio PDF compile y sea ejecutable.
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

public class AceriaDbContext : DbContext
{
    public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
    public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;

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
        using var context = new AceriaDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-001",
            Cliente = "Constructora del Norte",
            FechaCreacion = DateTime.Now
        };

        context.OrdenesFabricacion.Add(orden);
        context.SaveChanges();

        var plancha1 = new PlanchaAcero { OrdenId = orden.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000 };
        var plancha2 = new PlanchaAcero { OrdenId = orden.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500 };

        context.PlanchasAcero.AddRange(plancha1, plancha2);
        context.SaveChanges();

        var ordenRecuperada = context.OrdenesFabricacion
            .Include(o => o.Planchas)
            .FirstOrDefault(o => o.Id == orden.Id);

        Console.WriteLine($"Orden: {ordenRecuperada!.NumeroOrden}");
        foreach (var plancha in ordenRecuperada.Planchas)
        {
            Console.WriteLine($"  Plancha Id {plancha.Id} | Espesor: {plancha.Espesor} | Ancho: {plancha.Ancho} | Largo: {plancha.Largo}");
        }
    }
}
