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

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<DiagnosticoRendimientoUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.11 OK");

/*
// ERROR CONTROLADO M04 4.11 - EJECUTAR CONTADOR MANUAL
useCase.EjecutarErrorContadorManual();
*/

/*
// RETO M04 4.11 - EJECUTAR DIAGNOSTICLISTENER
useCase.EjecutarRetoDiagnosticListener();
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
// var useCase = scope.ServiceProvider.GetRequiredService<DiagnosticoRendimientoUseCase>();
// useCase.Ejecutar();
//
// Console.WriteLine("4.11 OK");
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 05
// SECTION: Paso 6: Registrar el caso de uso en el contenedor
// UBICACION EN EL EJERCICIO: Modificar src/AceriaData.Console/Program.cs:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// services.AddScoped<DiagnosticoUseCase>();
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 06
// SECTION: Paso 7: Llamar al caso de uso desde la consola
// UBICACION EN EL EJERCICIO: Modificar el método Main:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<DiagnosticoUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 07
// SECTION: Paso 8: Insertar datos de prueba con varias órdenes
// UBICACION EN EL EJERCICIO: Paso 8: Insertar datos de prueba con varias órdenes
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
// }
// ========================================================================
