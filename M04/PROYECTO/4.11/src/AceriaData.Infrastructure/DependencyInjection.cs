using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using AceriaData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AceriaData.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAceriaInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AceriaDbContext>(o => o
            .UseSqlServer(connectionString)
            .EnableDetailedErrors()
            .LogTo(
                Console.WriteLine,
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information)
            .AddInterceptors(SqlCommandCounterInterceptor.Instance));
        services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        return services;
    }
}

/*
// OPCIONES PEDAGOGICAS M04 4.11 - LOGGING Y WARNINGS
// Este bloque muestra opciones adicionales que el alumno puede activar de forma consciente.
// EnableSensitiveDataLogging queda deliberadamente comentado: puede exponer valores sensibles.
private static void AplicarOpcionesDiagnostico(
    DbContextOptionsBuilder options,
    ILoggerFactory loggerFactory)
{
    options.UseLoggerFactory(loggerFactory);
    options.EnableDetailedErrors();
    options.ConfigureWarnings(w => w.Log(RelationalEventId.CommandExecuted));
    // options.EnableSensitiveDataLogging();
}
*/
