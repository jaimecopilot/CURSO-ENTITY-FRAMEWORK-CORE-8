using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

using var provider = services.BuildServiceProvider(
    new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

using (var scopeInicial = provider.CreateScope())
{
    var context = scopeInicial.ServiceProvider.GetRequiredService<AceriaDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
    DemoData.Seed(context);
    context.ChangeTracker.Clear();
}

using (var scope = provider.CreateScope())
{
    var useCase = scope.ServiceProvider.GetRequiredService<TokensConcurrenciaM5UseCase>();
    useCase.Ejecutar();
}

Console.WriteLine("5.2 OK");

/*
// RETO M05 5.2 - EJECUTAR INSPECCION DEL PREDICADO
using (var scopeReto = provider.CreateScope())
{
    var reto = scopeReto.ServiceProvider.GetRequiredService<TokensConcurrenciaM5UseCase>();
    reto.EjecutarRetoPredicadoConcurrencia();
}
*/
