using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public IdentityResolutionMetricaDto MedirNoTrackingSinResolucionM4()
    {
        _context.ChangeTracker.Clear();
        var entidades = _context.OrdenesAleaciones
            .AsNoTracking()
            .OrderBy(oa => oa.OrdenFabricacionId)
            .Select(oa => oa.Aleacion)
            .ToList();

        return new IdentityResolutionMetricaDto
        {
            Filas = entidades.Count,
            ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),
            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
        };
    }

    public IdentityResolutionMetricaDto MedirNoTrackingConResolucionM4()
    {
        _context.ChangeTracker.Clear();
        var entidades = _context.OrdenesAleaciones
            .AsNoTrackingWithIdentityResolution()
            .OrderBy(oa => oa.OrdenFabricacionId)
            .Select(oa => oa.Aleacion)
            .ToList();

        return new IdentityResolutionMetricaDto
        {
            Filas = entidades.Count,
            ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),
            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
        };
    }

    /*
    // ERROR CONTROLADO M04 4.3 - ENTIDAD SIN CLAVES REPETIDAS
    public IdentityResolutionMetricaDto MedirPlanchasSinResolucionM4()
    {
        _context.ChangeTracker.Clear();
        var entidades = _context.PlanchasAcero
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToList();

        return new IdentityResolutionMetricaDto
        {
            Filas = entidades.Count,
            ClavesUnicas = entidades.Select(p => p.Id).Distinct().Count(),
            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
        };
    }

    public IdentityResolutionMetricaDto MedirPlanchasConResolucionM4()
    {
        _context.ChangeTracker.Clear();
        var entidades = _context.PlanchasAcero
            .AsNoTrackingWithIdentityResolution()
            .OrderBy(p => p.Id)
            .ToList();

        return new IdentityResolutionMetricaDto
        {
            Filas = entidades.Count,
            ClavesUnicas = entidades.Select(p => p.Id).Distinct().Count(),
            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
        };
    }
    */

}


// FRAGMENTO PDF M04 4.3 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public IdentityResolutionMetricaDto MedirNoTrackingSinResolucionM4()
//     {
//         _context.ChangeTracker.Clear();
//         var entidades = _context.OrdenesAleaciones
//             .AsNoTracking()
//             .OrderBy(oa => oa.OrdenFabricacionId)
//             .Select(oa => oa.Aleacion)
//             .ToList();
//
//         return new IdentityResolutionMetricaDto
//         {
//             Filas = entidades.Count,
//             ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),
//             InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
//         };
//     }
//
//     public IdentityResolutionMetricaDto MedirNoTrackingConResolucionM4()
//     {
//         _context.ChangeTracker.Clear();
//         var entidades = _context.OrdenesAleaciones
//             .AsNoTrackingWithIdentityResolution()
//             .OrderBy(oa => oa.OrdenFabricacionId)
//             .Select(oa => oa.Aleacion)
//             .ToList();
//
//         return new IdentityResolutionMetricaDto
//         {
//             Filas = entidades.Count,
//             ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),
//             InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
//         };
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasAsNoTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConPlanchasConResolucionIdentidad()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public int ContarPlanchasInstanciadas(List<OrdenFabricacion> ordenes)
// {
//     var instancias = new HashSet<PlanchaAcero>(ReferenceEqualityComparer.Instance);
//     foreach (var orden in ordenes)
//     {
//         foreach (var plancha in orden.Planchas)
//         {
//             instancias.Add(plancha);
//         }
//     }
//     return instancias.Count;
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 10
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConDosColeccionesAsNoTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConDosColeccionesConResolucionIdentidad()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================
