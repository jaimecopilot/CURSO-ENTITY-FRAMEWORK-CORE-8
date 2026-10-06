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
