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

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<IdentityResolutionUseCase>();
useCase.Ejecutar();

/*
// ERROR CONTROLADO M04 4.3 - EJECUTAR ENTIDAD NO REPETIDA
useCase.EjecutarErrorEntidadNoRepetida();
*/

/*
// RETO M04 4.3 - EJECUTAR COMPARACION DE REFERENCIAS
useCase.EjecutarRetoReferencias();
*/

Console.WriteLine("4.3 OK");


// FRAGMENTO PDF M04 4.3 - PASO 6
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
// var useCase = scope.ServiceProvider.GetRequiredService<IdentityResolutionUseCase>();
// useCase.Ejecutar();
//
// Console.WriteLine("4.3 OK");
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// services.AddScoped<ResolucionIdentidadUseCase>();
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ResolucionIdentidadUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con planchas compartidas
// SOURCE TARGET: Paso 7: Insertar datos de prueba con planchas compartidas
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     var orden3 = new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2, orden3);
//     context.SaveChanges();
//
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
//     var plancha4 = new PlanchaAcero { OrdenId = orden3.Id, Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m, Activa = true };
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3, plancha4);
//     context.SaveChanges();
// }
// ========================================================================
