using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
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

Console.WriteLine("5.7 OK");
