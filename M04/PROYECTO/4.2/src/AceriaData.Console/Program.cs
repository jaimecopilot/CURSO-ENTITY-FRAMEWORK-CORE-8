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

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<TrackingUseCase>();
useCase.Ejecutar();

/*
// ERROR CONTROLADO M04 4.2 - EJECUTAR ESTADO PREVIO
useCase.EjecutarErrorEstadoPrevio();
*/

/*
// RETO M04 4.2 - EJECUTAR GRAFO
useCase.EjecutarRetoGrafo();
*/

Console.WriteLine("4.2 OK");


// FRAGMENTO PDF M04 4.2 - PASO 6
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
// var useCase = scope.ServiceProvider.GetRequiredService<TrackingUseCase>();
// useCase.Ejecutar();
//
// Console.WriteLine("4.2 OK");
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<TrackingUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con varias órdenes
// SOURCE TARGET: Paso 7: Insertar datos de prueba con varias órdenes
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método DemostrarSinTracking para usar AsNoTracking en un DbContext distinto:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarSinTrackingAislado()
// {
//     Console.WriteLine("\n--- Sin Tracking (DbContext aislado) ---");
//
//     using var scope = _provider.CreateScope();
//     var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
//
//     var ordenes = repositorio.ObtenerSinTracking();
//     var rastreadas = repositorio.ContarEntidadesRastreadas();
//
//     Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//     Console.WriteLine($"Entidades rastreadas: {rastreadas}");
// }
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 10
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarTrackingConInclude()
// {
//     Console.WriteLine("\n--- Tracking con Include ---");
//
//     using var scopeCon = _provider.CreateScope();
//     var repoCon = scopeCon.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
//     var ordenesCon = repoCon.ObtenerConPlanchasConTracking();
//     var rastreadasCon = repoCon.ContarEntidadesRastreadas();
//     Console.WriteLine($"Con Tracking: órdenes: {ordenesCon.Count} | entidades rastreadas: {rastreadasCon}");
//
//     using var scopeSin = _provider.CreateScope();
//     var repoSin = scopeSin.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
//     var ordenesSin = repoSin.ObtenerConPlanchasSinTracking();
//     var rastreadasSin = repoSin.ContarEntidadesRastreadas();
//     Console.WriteLine($"Sin Tracking: órdenes: {ordenesSin.Count} | entidades rastreadas: {rastreadasSin}");
// }
// ========================================================================
