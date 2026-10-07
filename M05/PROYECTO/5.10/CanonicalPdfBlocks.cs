// ========================================================================
// M05 5.10 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 01
// SECTION: Paso 3: Configurar Serilog en el método Main
// SOURCE TARGET: Modificar el método Main de src/AceriaData.Console/Program.cs:
// ------------------------------------------------------------------------
// using Serilog;
//
// public static void Main()
// {
//     Log.Logger = new LoggerConfiguration()
//         .MinimumLevel.Information()
//         .WriteTo.Console()
//         .WriteTo.File("logs/aceria-.log", rollingInterval: RollingInterval.Day)
//         .CreateLogger();
//
//     try
//     {
//         Log.Information("Iniciando la aplicación AceriaData");
//
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//             .AddEnvironmentVariables()
//             .Build();
//
//         var connectionString = configuration.GetConnectionString("AceriaDB")
//             ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'AceriaDB'.");
//
//         var services = new ServiceCollection();
//
//         services.AddLogging(builder =>
//         {
//             builder.AddSerilog(Log.Logger, dispose: true);
//         });
//
//         services.AddDbContext<AceriaDbContext>(options =>
//             options
//                 .UseSqlServer(connectionString, sqlOptions =>
//                 {
//                     sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//                     sqlOptions.CommandTimeout(60);
//                 })
//                 .EnableSensitiveDataLogging()
//                 .EnableDetailedErrors());
//
//         services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
//         services.AddScoped<LoggingDiagnosticoUseCase>();
//
//         _provider = services.BuildServiceProvider();
//
//         using (var scope = _provider.CreateScope())
//         {
//             var useCase = scope.ServiceProvider.GetRequiredService<LoggingDiagnosticoUseCase>();
//             useCase.Ejecutar();
//         }
//     }
//     catch (Exception ex)
//     {
//         Log.Fatal(ex, "La aplicación ha terminado debido a una excepción no controlada");
//     }
//     finally
//     {
//         Log.CloseAndFlush();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 02
// SECTION: Paso 4: Crear el caso de uso de logging y diagnóstico
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/LoggingDiagnosticoUseCase.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using Microsoft.Extensions.Logging;
//
// namespace AceriaData.Application.UseCases;
//
// public class LoggingDiagnosticoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     private readonly ILogger<LoggingDiagnosticoUseCase> _logger;
//
//     public LoggingDiagnosticoUseCase(IUnidadDeTrabajo unidad, ILogger<LoggingDiagnosticoUseCase> logger)
//     {
//         _unidad = unidad;
//         _logger = logger;
//     }
//
//     public void Ejecutar()
//     {
//         _logger.LogInformation("=== LOGGING Y DIAGNÓSTICO ===");
//
//         DemostrarLoggingDeConsultas();
//         DemostrarLoggingDeEscritura();
//     }
//
//     private void DemostrarLoggingDeConsultas()
//     {
//         _logger.LogInformation("--- Consultas ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerTodas();
//         _logger.LogInformation("Se han cargado {Total} órdenes", ordenes.Count);
//
//         var pendientes = _unidad.Ordenes.ObtenerPendientes();
//         _logger.LogInformation("Se han cargado {Total} órdenes pendientes", pendientes.Count);
//     }
//
//     private void DemostrarLoggingDeEscritura()
//     {
//         _logger.LogInformation("--- Escritura ---");
//
//         var orden = new OrdenFabricacion
//         {
//             NumeroOrden = "OF-LOG-001",
//             Cliente = "Constructora del Norte",
//             Estado = "Pendiente",
//             FechaCreacion = DateTime.Now
//         };
//
//         _unidad.Ordenes.Agregar(orden);
//         var filas = _unidad.Guardar();
//
//         _logger.LogInformation("Se han insertado {Filas} filas. Orden creada: {NumeroOrden}", filas, orden.NumeroOrden);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 03
// SECTION: Paso 5: Configurar el filtrado de Serilog
// SOURCE TARGET: Modificar la configuración de Serilog en el método Main:
// ------------------------------------------------------------------------
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
//     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Update", Serilog.Events.LogEventLevel.Information)
//     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", Serilog.Events.LogEventLevel.Warning)
//     .WriteTo.Console()
//     .WriteTo.File("logs/aceria-.log", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 04
// SECTION: Paso 6: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar el método Main para registrar el caso de uso:
// ------------------------------------------------------------------------
// services.AddScoped<LoggingDiagnosticoUseCase>();
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 05
// SECTION: Paso 7: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<LoggingDiagnosticoUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 06
// SECTION: Paso 8: Insertar datos de prueba iniciales
// SOURCE TARGET: Asegurarse de que hay datos en la base de datos. Modificar el método Main para insertar órdenes:
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var ordenes = new List<OrdenFabricacion>
//     {
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 2, 20) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 3, 10) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 07
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Modificar la configuración de Serilog para no filtrar los mensajes de EF Core:
// ------------------------------------------------------------------------
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .WriteTo.Console()
//     .WriteTo.File("logs/aceria-.log", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 08
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Paso 11: Diagnosticar un error común
// ------------------------------------------------------------------------
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
//     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", Serilog.Events.LogEventLevel.Warning)
//     .WriteTo.Console()
//     .WriteTo.File("logs/aceria-.log", rollingInterval: RollingInterval.Day)
//     .CreateLogger();
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 09
// SECTION: Paso 1: Modificar el repositorio para recibir ILogger<T>:
// SOURCE TARGET: ### Paso 1: Modificar el repositorio para recibir ILogger<T>:
// ------------------------------------------------------------------------
// using Microsoft.Extensions.Logging;
//
// public class OrdenRepositorio : Repositorio<OrdenFabricacion>, IOrdenRepositorio
// {
//     private readonly ILogger<OrdenRepositorio> _logger;
//
//     public OrdenRepositorio(AceriaDbContext context, ILogger<OrdenRepositorio> logger) : base(context)
//     {
//         _logger = logger;
//     }
//
//     public override void Agregar(OrdenFabricacion orden)
//     {
//         _logger.LogInformation("Agregando orden {NumeroOrden} del cliente {Cliente}", orden.NumeroOrden, orden.Cliente);
//         base.Agregar(orden);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 10
// SECTION: Paso 2: Registrar el repositorio en el contenedor:
// SOURCE TARGET: Paso 2: Registrar el repositorio en el contenedor:
// ------------------------------------------------------------------------
// services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 11
// SECTION: Paso 12: Configurar un sink de archivo con rotación y retención
// SOURCE TARGET: Paso 12: Configurar un sink de archivo con rotación y retención
// ------------------------------------------------------------------------
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .WriteTo.Console()
//     .WriteTo.File(
//         path: "logs/aceriadata-.log",
//         rollingInterval: RollingInterval.Day,
//         fileSizeLimitBytes: 10 * 1024 * 1024,
//         rollOnFileSizeLimit: true,
//         retainedFileCountLimit: 7,
//         shared: true)
//     .CreateLogger();
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 12
// SECTION: Paso 13: Implementar un observador completo de `DiagnosticListener`
// SOURCE TARGET: Paso 13: Implementar un observador completo de `DiagnosticListener`
// ------------------------------------------------------------------------
// using System.Diagnostics;
//
// public sealed class EfDiagnosticObserver :
//     IObserver<DiagnosticListener>,
//     IObserver<KeyValuePair<string, object?>>, IDisposable
// {
//     private readonly List<IDisposable> _subscriptions = new();
//     private readonly Action<string> _write;
//
//     public EfDiagnosticObserver(Action<string> write) => _write = write;
//
//     public void Start() =>
//         _subscriptions.Add(DiagnosticListener.AllListeners.Subscribe(this));
//
//     public void OnNext(DiagnosticListener listener)
//     {
//         if (listener.Name == "Microsoft.EntityFrameworkCore")
//             _subscriptions.Add(listener.Subscribe(this));
//     }
//
//     public void OnNext(KeyValuePair<string, object?> evt)
//     {
//         if (evt.Key.Contains("CommandExecuting", StringComparison.Ordinal) ||
//             evt.Key.Contains("CommandExecuted", StringComparison.Ordinal) ||
//             evt.Key.Contains("SaveChanges", StringComparison.Ordinal))
//         {
//             _write($"EF-DIAG | {evt.Key}");
//         }
//     }
//
//     public void OnError(Exception error) => _write($"EF-DIAG-ERROR | {error.Message}");
//     public void OnCompleted() { }
//
//     public void Dispose()
//     {
//         foreach (var subscription in _subscriptions)
//             subscription.Dispose();
//         _subscriptions.Clear();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 13
// SECTION: Paso 14: Observar los EventCounters de EF Core 8
// SOURCE TARGET: Paso 14: Observar los EventCounters de EF Core 8
// ------------------------------------------------------------------------
// Console.WriteLine($"PID: {Environment.ProcessId}");
// ========================================================================

// CANONICAL PDF M05 5.10 - BLOCK 14
// SECTION: Paso 15: Añadir Application Insights como destino mediante Azure Monitor/OpenTelemetry
// SOURCE TARGET: Paso 15: Añadir Application Insights como destino mediante Azure Monitor/OpenTelemetry
// ------------------------------------------------------------------------
// using Azure.Monitor.OpenTelemetry.Exporter;
// using OpenTelemetry;
// using OpenTelemetry.Trace;
//
// var aiConnectionString = Environment.GetEnvironmentVariable(
//     "APPLICATIONINSIGHTS_CONNECTION_STRING");
//
// TracerProvider? azureMonitor = null;
// if (!string.IsNullOrWhiteSpace(aiConnectionString))
// {
//     azureMonitor = Sdk.CreateTracerProviderBuilder()
//         .AddSource("AceriaData")
//         .AddAzureMonitorTraceExporter(options =>
//             options.ConnectionString = aiConnectionString)
//         .Build();
// }
// ========================================================================

