using Microsoft.EntityFrameworkCore;

namespace AceriaData.ConsoleApp;

public class OrdenFabricacion
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    // Soporte necesario para el reto resuelto del propio PDF:
    // permite ejecutar Include(o => o.Planchas).
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

public class AceriaDbContext : DbContext
{
    public DbSet<OrdenFabricacion> OrdenesFabricacion { get; set; } = null!;
    public DbSet<PlanchaAcero> PlanchasAcero { get; set; } = null!;
    public DbSet<Aleacion> Aleaciones { get; set; } = null!;

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
        // RESULTADO FINAL / RETO RESUELTO DEL PUNTO 1.3
        // Inserta una orden con dos planchas y la recupera con Include.
        // ============================================================
        using var context = new AceriaDbContext();
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-001",
            Cliente = "Constructora del Norte",
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
        context.SaveChanges();

        var ordenRecuperada = context.OrdenesFabricacion
            .Include(o => o.Planchas)
            .FirstOrDefault(o => o.Id == orden.Id);

        Console.WriteLine($"Orden: {ordenRecuperada!.NumeroOrden}");

        foreach (var plancha in ordenRecuperada.Planchas)
        {
            Console.WriteLine(
                $" Plancha Id {plancha.Id} | Espesor: {plancha.Espesor} | Ancho: {plancha.Ancho} | Largo: {plancha.Largo}");
        }

        // ============================================================
        // PASOS 4-5 - INSPECCIONAR PROVEEDOR Y MODELO
        // Comenta el bloque activo anterior y descomenta este para
        // repetir la primera inspección propuesta por el PDF.
        // ============================================================
        // using var context = new AceriaDbContext();
        // Console.WriteLine($"Proveedor: {context.Database.ProviderName}");
        // Console.WriteLine($"Entidades en el modelo: {context.Model.GetEntityTypes().Count()}");
        // foreach (var entidad in context.Model.GetEntityTypes())
        // {
        //     Console.WriteLine($" Entidad: {entidad.ClrType.Name} → Tabla: {entidad.GetTableName()}");
        // }

        // ============================================================
        // PASOS 6-7 - INSPECCIONAR EL CHANGE TRACKER
        // Resultado esperado: Detached, Added y Added.
        // ============================================================
        // using var context = new AceriaDbContext();
        // var orden = new OrdenFabricacion
        // {
        //     NumeroOrden = "OF-001",
        //     Cliente = "Constructora del Norte",
        //     FechaCreacion = DateTime.Now
        // };
        //
        // Console.WriteLine($"Estado antes de Add: {context.Entry(orden).State}");
        // context.OrdenesFabricacion.Add(orden);
        // Console.WriteLine($"Estado después de Add: {context.Entry(orden).State}");
        // orden.Cliente = "Constructora del Sur";
        // Console.WriteLine($"Estado después de modificar: {context.Entry(orden).State}");

        // ============================================================
        // PASO 8 - INSPECCIONAR EL MODELO COMPLETO
        // ============================================================
        // using var context = new AceriaDbContext();
        // foreach (var entidad in context.Model.GetEntityTypes())
        // {
        //     Console.WriteLine($"Entidad: {entidad.ClrType.Name} → Tabla: {entidad.GetTableName()}");
        //     foreach (var propiedad in entidad.GetProperties())
        //     {
        //         Console.WriteLine(
        //             $" Propiedad: {propiedad.Name} | Columna: {propiedad.GetColumnName()} | Tipo: {propiedad.ClrType.Name}");
        //     }
        // }

        // ============================================================
        // PASO 15 - INSERTAR UNA ÚNICA PLANCHA ASOCIADA
        // ============================================================
        // using var context = new AceriaDbContext();
        // context.Database.EnsureDeleted();
        // context.Database.Migrate();
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
        //
        // var plancha = new PlanchaAcero
        // {
        //     OrdenId = orden.Id,
        //     Espesor = 10.5,
        //     Ancho = 1500,
        //     Largo = 3000
        // };
        //
        // context.PlanchasAcero.Add(plancha);
        // context.SaveChanges();
        // Console.WriteLine(
        //     $"Plancha insertada con Id {plancha.Id} para la orden {plancha.OrdenId}.");
    }
}
