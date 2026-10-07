using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using AceriaData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AceriaData.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAceriaInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AceriaDbContext>(o => o.UseSqlServer(connectionString));
        services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        return services;
    }
}

// ============================================================================
// EJEMPLO DEL PASO 6B
// Registro de Infrastructure mediante IServiceCollection.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 6 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Infrastructure.Persistence;
// using AceriaData.Infrastructure.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// 
// namespace AceriaData.Infrastructure;
// 
// public static class DependencyInjection
// {
//     public static IServiceCollection AddAceriaInfrastructure(this IServiceCollection services, string connectionString)
//     {
//         services.AddDbContext<AceriaDbContext>(o => o.UseSqlServer(connectionString));
//         services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
//         services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
//         return services;
//     }
// }
// ============================================================================
