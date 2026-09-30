using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using AceriaData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
        services.AddScoped<IConcurrenciaOptimistaM5Repositorio, ConcurrenciaOptimistaM5Repositorio>();
        services.AddScoped<ITokensConcurrenciaM5Repositorio, TokensConcurrenciaM5Repositorio>();
        services.AddScoped<IResolucionConflictosM5Repositorio, ResolucionConflictosM5Repositorio>();
        services.AddScoped<ITransaccionesM5Repositorio, TransaccionesM5Repositorio>();

        return services;
    }
}
