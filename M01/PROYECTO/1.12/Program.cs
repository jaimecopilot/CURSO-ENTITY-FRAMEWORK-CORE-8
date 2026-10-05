using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    public AceriaDbContext(DbContextOptions<AceriaDbContext> options) : base(options) { }

    public DbSet<OrdenFabricacion> OrdenesFabricacion => Set<OrdenFabricacion>();
    public DbSet<PlanchaAcero> PlanchasAcero => Set<PlanchaAcero>();
    public DbSet<Aleacion> Aleaciones => Set<Aleacion>();
    public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();
}

public interface IOrdenRepositorio
{
    List<OrdenFabricacion> ObtenerTodas();
    OrdenFabricacion? ObtenerPorId(int id);
    OrdenFabricacion? ObtenerPorNumero(string numeroOrden);
    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
    void Guardar();
}

public sealed class OrdenRepositorio : IOrdenRepositorio
{
    private readonly AceriaDbContext _context;

    public OrdenRepositorio(AceriaDbContext context) => _context = context;

    public List<OrdenFabricacion> ObtenerTodas() =>
        _context.OrdenesFabricacion.OrderBy(o => o.Id).ToList();

    public OrdenFabricacion? ObtenerPorId(int id) => _context.OrdenesFabricacion.Find(id);

    public OrdenFabricacion? ObtenerPorNumero(string numeroOrden) =>
        _context.OrdenesFabricacion.FirstOrDefault(o => o.NumeroOrden == numeroOrden);

    public void Agregar(OrdenFabricacion orden) => _context.OrdenesFabricacion.Add(orden);

    public void Eliminar(OrdenFabricacion orden) => _context.OrdenesFabricacion.Remove(orden);

    public void Guardar()
    {
        _context.SaveChanges();
    }
}

public interface IServicioOrdenes
{
    string ObtenerResumen(int ordenId);
}

public sealed class ServicioOrdenes : IServicioOrdenes
{
    private readonly IOrdenRepositorio _repositorio;
    private readonly AceriaDbContext _context;

    public ServicioOrdenes(IOrdenRepositorio repositorio, AceriaDbContext context)
    {
        _repositorio = repositorio;
        _context = context;
    }

    public string ObtenerResumen(int ordenId)
    {
        var orden = _repositorio.ObtenerPorId(ordenId);
        if (orden is null)
        {
            return $"Orden {ordenId} no encontrada";
        }

        var totalPlanchas = _context.PlanchasAcero.Count(p => p.OrdenId == ordenId);
        return $"Orden {orden.NumeroOrden} | Cliente: {orden.Cliente} | Planchas: {totalPlanchas}";
    }
}

public static class Program
{
    public static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("AceriaDB")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'AceriaDB'.");

        var services = new ServiceCollection();
        services.AddDbContext<AceriaDbContext>(options =>
            options
                .UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(60);
                })
                .LogTo(
                    global::System.Console.WriteLine,
                    new[] { "Microsoft.EntityFrameworkCore.Database.Command" },
                    LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

        services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
        services.AddScoped<IServicioOrdenes, ServicioOrdenes>();

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        // ============================================================
        // PASO 5 - SEPARACIÓN DE ÁMBITOS
        // Cada CreateScope debe obtener una instancia diferente de
        // AceriaDbContext. Se ejecuta para poder observarlo directamente.
        // ============================================================
        DemostrarSeparacionAmbitos(provider);

        // ============================================================
        // PASO 7 - ERROR COMÚN: RESOLVER Scoped DESDE EL PROVEEDOR RAÍZ
        // Descomenta esta línea para provocar InvalidOperationException.
        // Después vuelve a comentarla y usa siempre CreateScope().
        // ============================================================
        // var contextRaiz = provider.GetRequiredService<AceriaDbContext>();

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        using (var scope = provider.CreateScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
            repositorio.Agregar(new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", FechaCreacion = DateTime.Now });
            repositorio.Agregar(new OrdenFabricacion { NumeroOrden = "OF-002", Cliente = "Constructora del Sur", FechaCreacion = DateTime.Now });
            repositorio.Agregar(new OrdenFabricacion { NumeroOrden = "OF-003", Cliente = "Constructora del Este", FechaCreacion = DateTime.Now });
            repositorio.Guardar();
        }

        using (var scope = provider.CreateScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
            global::System.Console.WriteLine("--- Órdenes ---");
            foreach (var orden in repositorio.ObtenerTodas())
            {
                global::System.Console.WriteLine($"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
            }
        }

        using (var scope = provider.CreateScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
            var orden = repositorio.ObtenerPorNumero("OF-002");
            if (orden is not null)
            {
                orden.Cliente = "Constructora del Oeste";
                repositorio.Guardar();
                global::System.Console.WriteLine($"Orden {orden.NumeroOrden} actualizada a {orden.Cliente}");
            }
        }

        using (var scope = provider.CreateScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
            var orden = repositorio.ObtenerPorNumero("OF-003");
            if (orden is not null)
            {
                repositorio.Eliminar(orden);
                repositorio.Guardar();
                global::System.Console.WriteLine($"Orden {orden.NumeroOrden} eliminada");
            }
        }

        using (var scope = provider.CreateScope())
        {
            var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
            global::System.Console.WriteLine("--- Órdenes finales ---");
            foreach (var orden in repositorio.ObtenerTodas())
            {
                global::System.Console.WriteLine($"Id: {orden.Id} | Número: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
            }

            var servicio = scope.ServiceProvider.GetRequiredService<IServicioOrdenes>();
            global::System.Console.WriteLine(servicio.ObtenerResumen(1));
            global::System.Console.WriteLine(servicio.ObtenerResumen(2));
            global::System.Console.WriteLine(servicio.ObtenerResumen(999));
        }
    }

    public static void DemostrarSeparacionAmbitos(IServiceProvider provider)
    {
        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<AceriaDbContext>();

            global::System.Console.WriteLine(
                $"DbContext en ámbito 1: {context.GetHashCode()}");
        }

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<AceriaDbContext>();

            global::System.Console.WriteLine(
                $"DbContext en ámbito 2: {context.GetHashCode()}");
        }
    }

}
