using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.ConsoleApp.Diagnostics;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using AceriaData.Infrastructure.Repositories;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
Directory.CreateDirectory(logDirectory);
foreach (var oldFile in Directory.GetFiles(logDirectory, "aceria-*.log"))
{
    File.Delete(oldFile);
}

var serilog = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Update", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logDirectory, "aceria-.log"),
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 4096,
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 3,
        shared: false)
    .CreateLogger();

using var diagnosticObserver = new EfDiagnosticObserver();
using var counterListener = new EfEventCounterListener();

var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.SetMinimumLevel(LogLevel.Information);
    builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
    builder.AddFilter("Microsoft.EntityFrameworkCore.Update", LogLevel.Information);
    builder.AddFilter("Microsoft.EntityFrameworkCore.Query", LogLevel.Warning);
    builder.AddSerilog(serilog, dispose: false);
});
services.AddApplicationInsightsTelemetryWorkerService(options =>
{
    options.ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000";
});
services.AddSingleton<CollectingTelemetryChannel>();
services.AddSingleton<ITelemetryChannel>(sp => sp.GetRequiredService<CollectingTelemetryChannel>());

services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();
services.AddScoped<SplitQueriesUseCase>();
services.AddScoped<CompiledQueriesUseCase>();
services.AddScoped<PaginacionUseCase>();
services.AddScoped<DiagnosticoRendimientoUseCase>();
services.AddScoped<ChecklistRendimientoUseCase>();
services.AddScoped<ConcurrenciaOptimistaM5UseCase>();
services.AddScoped<TokensConcurrenciaM5UseCase>();
services.AddScoped<ResolucionConflictosM5UseCase>();
services.AddScoped<TransaccionesM5UseCase>();
services.AddScoped<TransaccionesAmbientalesM5UseCase>();
services.AddScoped<MigracionesProduccionM5UseCase>();
services.AddScoped<RepositorioUnidadTrabajoM5UseCase>();
services.AddScoped<LoggingDiagnosticoM5Runner>();
services.AddScoped<BuenasPracticasAntiPatronesM5Runner>();

using var provider = services.BuildServiceProvider(
    new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

using (var scopeInicial = provider.CreateScope())
{
    var context = scopeInicial.ServiceProvider.GetRequiredService<AceriaDbContext>();
    context.Database.EnsureDeleted();
}

using (var scope = provider.CreateScope())
{
    var useCase = scope.ServiceProvider.GetRequiredService<MigracionesProduccionM5UseCase>();
    await useCase.EjecutarAsync();
}

using (var scopeDatos = provider.CreateScope())
{
    var context = scopeDatos.ServiceProvider.GetRequiredService<AceriaDbContext>();
    DemoData.Seed(context);
    context.ChangeTracker.Clear();
}

using (var scopeRepositorio = provider.CreateScope())
{
    var useCase = scopeRepositorio.ServiceProvider.GetRequiredService<RepositorioUnidadTrabajoM5UseCase>();
    await useCase.EjecutarAsync();
}

using (var scopeDiagnostico = provider.CreateScope())
{
    var diagnostico = scopeDiagnostico.ServiceProvider.GetRequiredService<RepositorioUnidadTrabajoM5Diagnostico>();
    await diagnostico.EjecutarAsync();
}

using (var scopeLogging = provider.CreateScope())
{
    var runner = scopeLogging.ServiceProvider.GetRequiredService<LoggingDiagnosticoM5Runner>();
    await runner.EjecutarAsync();
}

using (var scopeBuenasPracticas = provider.CreateScope())
{
    var runner = scopeBuenasPracticas.ServiceProvider.GetRequiredService<BuenasPracticasAntiPatronesM5Runner>();
    runner.Ejecutar();
}

for (var i = 0; i < 80; i++)
{
    serilog.Information("ROTACION 5.10 {Indice} {Payload}", i, new string('X', 180));
}

for (var intento = 0; intento < 5 && counterListener.CounterCount == 0; intento++)
{
    await Task.Delay(1000);
}

var telemetryChannel = provider.GetRequiredService<CollectingTelemetryChannel>();
if (diagnosticObserver.EfEventCount == 0)
    throw new InvalidOperationException("No se capturaron eventos DiagnosticSource de EF Core.");
if (counterListener.CounterCount == 0)
    throw new InvalidOperationException("No se capturaron EventCounters de EF Core 8.");
if (telemetryChannel.Count == 0)
    throw new InvalidOperationException("Application Insights no produjo telemetria en el canal local.");

serilog.Dispose();

var logFiles = Directory.GetFiles(logDirectory, "aceria-*.log");
if (logFiles.Length < 2)
    throw new InvalidOperationException("No se produjo rotacion de archivo por tamano.");
if (logFiles.Length > 3)
    throw new InvalidOperationException("retainedFileCountLimit no se respeto.");

Console.WriteLine($"5.10 DIAGNOSTIC EVENTS: {diagnosticObserver.EfEventCount}");
Console.WriteLine($"5.10 EVENT COUNTERS: {counterListener.CounterCount}");
Console.WriteLine($"5.10 TELEMETRY ITEMS: {telemetryChannel.Count}");
Console.WriteLine($"5.10 LOG FILES: {logFiles.Length}");

var waitText = Environment.GetEnvironmentVariable("ACERIA_COUNTER_WAIT_MS");
if (int.TryParse(waitText, out var waitMs) && waitMs > 0)
{
    Console.WriteLine($"5.10 COUNTER PID: {Environment.ProcessId}");
    await Task.Delay(waitMs);
}

Console.WriteLine("5.12 OK");
