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
services.AddScoped<AgregacionesUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateOnBuild = true,
    ValidateScopes = true
});
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<AgregacionesUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.5 OK");

// ============================================================================
// FRAGMENTO PDF M03 3.5 - PASO 5
// COPIA PEDAGÓGICA EXACTA DEL BLOQUE PUBLICADO EN M03_PRACTICA.
// El E2E sustituye temporalmente el archivo activo por esta copia y la ejecuta.
// ----------------------------------------------------------------------------
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
// services.AddScoped<AgregacionesUseCase>();
// 
// using var provider = services.BuildServiceProvider(new ServiceProviderOptions
// {
//     ValidateOnBuild = true,
//     ValidateScopes = true
// });
// using var scope = provider.CreateScope();
// 
// var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
// context.Database.EnsureDeleted();
// context.Database.Migrate();
// DemoData.Seed(context);
// context.ChangeTracker.Clear();
// 
// var useCase = scope.ServiceProvider.GetRequiredService<AgregacionesUseCase>();
// useCase.Ejecutar();
// 
// Console.WriteLine("3.5 OK");
// ============================================================================
