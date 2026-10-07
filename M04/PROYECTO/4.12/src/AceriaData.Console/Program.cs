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

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<ChecklistRendimientoUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.12 OK");

/*
// ERROR CONTROLADO M04 4.12 - EJECUTAR CHECKLIST INEFICIENTE
useCase.EjecutarErrorChecklistIneficiente();
*/

/*
// RETO M04 4.12 - EJECUTAR AUDITORIA COMPLETA
useCase.EjecutarRetoAuditoria();
*/


// EJEMPLO DEL PASO 6
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
// services.AddScoped<PaginacionUseCase>();
// services.AddScoped<DiagnosticoRendimientoUseCase>();
// services.AddScoped<ChecklistRendimientoUseCase>();
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
// var useCase = scope.ServiceProvider.GetRequiredService<ChecklistRendimientoUseCase>();
// useCase.Ejecutar();
//
// Console.WriteLine("4.12 OK");
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// UBICACION EN EL EJERCICIO: Modificar el método Main:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ChecklistRendimientoUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 06
// SECTION: Paso 7: Insertar datos de prueba con varias órdenes y planchas
// UBICACION EN EL EJERCICIO: Paso 7: Insertar datos de prueba con varias órdenes y planchas
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var ordenes = new List<OrdenFabricacion>();
//     for (int i = 1; i <= 20; i++)
//     {
//         ordenes.Add(new OrdenFabricacion
//         {
//             NumeroOrden = $"OF-2024-{i:D4}",
//             Cliente = i % 2 == 0 ? "Constructora del Norte" : "Constructora del Sur",
//             Estado = i % 3 == 0 ? "EnProceso" : "Pendiente",
//             FechaCreacion = new DateTime(2024, 1, 1).AddDays(i)
//         });
//     }
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
//
//     var planchas = new List<PlanchaAcero>();
//     foreach (var orden in ordenes)
//     {
//         planchas.Add(new PlanchaAcero { OrdenId = orden.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true });
//     }
//
//     context.PlanchasAcero.AddRange(planchas);
//     context.SaveChanges();
// }
// ========================================================================
