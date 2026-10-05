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
    private readonly string? _connectionString;

    public AceriaDbContext(DbContextOptions<AceriaDbContext> options)
        : base(options)
    {
    }

    // Se conserva para la fábrica de diseño y para el reto AuditarProveedor.
    public AceriaDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<OrdenFabricacion> OrdenesFabricacion => Set<OrdenFabricacion>();
    public DbSet<PlanchaAcero> PlanchasAcero => Set<PlanchaAcero>();
    public DbSet<Aleacion> Aleaciones => Set<Aleacion>();
    public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && _connectionString is not null)
        {
            optionsBuilder
                .UseSqlServer(_connectionString, sqlOptions =>
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
                .EnableDetailedErrors();
        }
    }
}

public static class Program
{
    public static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("AceriaDB")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'AceriaDB'.");

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

        using var provider = services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<AceriaDbContext>();

            context.Database.EnsureDeleted();
            context.Database.Migrate();

            context.OrdenesFabricacion.AddRange(
                new OrdenFabricacion
                {
                    NumeroOrden = "OF-001",
                    Cliente = "Constructora del Norte",
                    FechaCreacion = DateTime.Now
                },
                new OrdenFabricacion
                {
                    NumeroOrden = "OF-002",
                    Cliente = "Constructora del Sur",
                    FechaCreacion = DateTime.Now
                },
                new OrdenFabricacion
                {
                    NumeroOrden = "OF-003",
                    Cliente = "Constructora del Este",
                    FechaCreacion = DateTime.Now
                },
                new OrdenFabricacion
                {
                    NumeroOrden = "OF-004",
                    Cliente = "Constructora del Norte",
                    FechaCreacion = DateTime.Now
                },
                new OrdenFabricacion
                {
                    NumeroOrden = "OF-005",
                    Cliente = "Constructora del Norte",
                    FechaCreacion = DateTime.Now
                },
                new OrdenFabricacion
                {
                    NumeroOrden = "OF-006",
                    Cliente = "Constructora del Oeste",
                    FechaCreacion = DateTime.Now
                });

            context.SaveChanges();
        }

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<AceriaDbContext>();

            Console.WriteLine(
                $"Proveedor activo: {context.Database.ProviderName}");

            var consulta = context.OrdenesFabricacion
                .Where(o => o.Cliente == "Constructora del Norte")
                .OrderBy(o => o.Id);

            Console.WriteLine("--- SQL de consulta ---");
            Console.WriteLine(consulta.ToQueryString());

            var ordenes = await consulta.ToListAsync();
            Console.WriteLine($"Órdenes recuperadas: {ordenes.Count}");

            Console.WriteLine("--- Mapeo del modelo ---");
            foreach (var entityType in context.Model
                         .GetEntityTypes()
                         .OrderBy(e => e.ClrType.Name))
            {
                Console.WriteLine($"Entidad: {entityType.ClrType.Name}");

                foreach (var property in entityType.GetProperties())
                {
                    Console.WriteLine(
                        $"  {property.Name} | CLR: {property.ClrType.Name} | SQL: {property.GetColumnType() ?? "convención del proveedor"}");
                }
            }

            var pagina = context.OrdenesFabricacion
                .OrderBy(o => o.Id)
                .Skip(2)
                .Take(3);

            Console.WriteLine("--- SQL de paginación ---");
            Console.WriteLine(pagina.ToQueryString());

            var existe = context.OrdenesFabricacion
                .Any(o => o.NumeroOrden == "OF-001");

            Console.WriteLine($"Existe OF-001: {existe}");
        }

        AuditarProveedor(connectionString);

        // ============================================================
        // PASO 19 - INSTANCIA DE SERVIDOR INCORRECTA
        // Descomenta SOLO este bloque para provocar el error de conexión
        // descrito en la práctica. Después vuelve a dejarlo comentado.
        // ============================================================
        /*
        var conexionIncorrecta =
            @"Server=(localdb)\InstanciaQueNoExiste;Database=AceriaDB;Trusted_Connection=True;";

        using (var contextoIncorrecto =
               new AceriaDbContext(conexionIncorrecta))
        {
            contextoIncorrecto.Database.OpenConnection();
        }
        */

        // ============================================================
        // PASO 20 - MISMO PROVEEDOR, OTRA BASE DE DATOS
        // Descomenta SOLO este bloque para crear AceriaDB_Laboratorio
        // en la misma instancia SQL Server LocalDB.
        // ============================================================
        /*
        var conexionLaboratorio =
            @"Server=(localdb)\MSSQLLocalDB;Database=AceriaDB_Laboratorio;Trusted_Connection=True;";

        using (var contextoLaboratorio =
               new AceriaDbContext(conexionLaboratorio))
        {
            contextoLaboratorio.Database.Migrate();
            Console.WriteLine(
                $"Proveedor laboratorio: {contextoLaboratorio.Database.ProviderName}");
            Console.WriteLine(
                $"Base laboratorio: {contextoLaboratorio.Database.GetDbConnection().Database}");
        }
        */
    }

    // Reto resuelto del PDF: auditar el proveedor real.
    public static void AuditarProveedor(string connectionString)
    {
        using var context = new AceriaDbContext(connectionString);

        Console.WriteLine(
            $"Proveedor: {context.Database.ProviderName}");
        Console.WriteLine(
            $"Base de datos: {context.Database.GetDbConnection().Database}");
        Console.WriteLine(
            $"Origen: {context.Database.GetDbConnection().DataSource}");

        var consulta = context.OrdenesFabricacion
            .Where(o => o.Id > 0)
            .OrderBy(o => o.Id)
            .Take(5);

        Console.WriteLine("--- SQL generado ---");
        Console.WriteLine(consulta.ToQueryString());

        Console.WriteLine("--- Entidades del modelo ---");
        foreach (var entityType in context.Model
                     .GetEntityTypes()
                     .OrderBy(e => e.ClrType.Name))
        {
            Console.WriteLine(entityType.ClrType.Name);
        }
    }
}
