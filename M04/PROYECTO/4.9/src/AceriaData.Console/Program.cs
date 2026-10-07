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

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<CompiledQueriesUseCase>();
useCase.Ejecutar();

/*
// ERROR CONTROLADO M04 4.9 - EJECUTAR COMPILACION POR LLAMADA
useCase.EjecutarErrorCompilarCadaLlamada();
*/

/*
// RETO M04 4.9 - EJECUTAR COMPILED ASYNC QUERY
await useCase.EjecutarRetoAsync();
*/

Console.WriteLine("4.9 OK");


// FRAGMENTO PDF M04 4.9 - PASO 6
// ------------------------------------------------------------------------
// using AceriaData.Application.UseCases;
// using AceriaData.ConsoleApp;
// using AceriaData.Infrastructure;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// var configuration = new ConfigurationBuilder()
//     .SetBasePath(AppContext.BaseDirectory)
//     .AddJsonFile("appsettings.json", optional: false)
//     .AddEnvironmentVariables()
//     .Build();
//
// var connectionString = configuration.GetConnectionString("AceriaDB")
//     ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");
//
// var services = new ServiceCollection();
// services.AddAceriaInfrastructure(connectionString);
// services.AddScoped<BuenasPracticasUseCase>();
// services.AddScoped<AnalisisSqlUseCase>();
// services.AddScoped<TrackingUseCase>();
// services.AddScoped<IdentityResolutionUseCase>();
// services.AddScoped<NMasUnoUseCase>();
// services.AddScoped<SolucionesNMasUnoUseCase>();
// services.AddScoped<OverFetchingUseCase>();
// services.AddScoped<TraduccionConsultasUseCase>();
// services.AddScoped<SplitQueriesUseCase>();
// services.AddScoped<CompiledQueriesUseCase>();
//
// using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
// using var scope = provider.CreateScope();
//
// var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
// context.Database.EnsureDeleted();
// context.Database.Migrate();
// DemoData.Seed(context);
// context.ChangeTracker.Clear();
//
// var useCase = scope.ServiceProvider.GetRequiredService<CompiledQueriesUseCase>();
// useCase.Ejecutar();
//
// Console.WriteLine("4.9 OK");
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 06
// SECTION: Paso 7: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<CompiledQueriesUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 07
// SECTION: Paso 8: Insertar datos de prueba con varias órdenes
// SOURCE TARGET: Paso 8: Insertar datos de prueba con varias órdenes
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0004", Cliente = "Constructora del Oeste", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 4, 5) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0006", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 6, 18) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================
