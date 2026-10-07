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


// EJEMPLO DEL PASO 4
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

// VARIANTE COMENTADA - BLOQUE 02
// PASO: Paso 3: Implementar los métodos en el repositorio
// UBICACIÓN INDICADA: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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

// VARIANTE COMENTADA - BLOQUE 10
// PASO: Paso 2: Implementar los métodos en OrdenRepositorio:
// UBICACIÓN INDICADA: Paso 2: Implementar los métodos en OrdenRepositorio:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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
