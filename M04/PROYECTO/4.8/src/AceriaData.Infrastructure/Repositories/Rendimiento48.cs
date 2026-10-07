using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private IQueryable<OrdenFabricacion> ConsultaDosColeccionesM4() =>
        _context.OrdenesFabricacion
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Planchas)
            .Include(o => o.OrdenesAleaciones)
                .ThenInclude(oa => oa.Aleacion)
            .OrderBy(o => o.Id);

    public SplitQueryMetricaDto MedirSingleQueryM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().AsSingleQuery().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }

    public SplitQueryMetricaDto MedirSplitQueryM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().AsSplitQuery().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }

    public string ObtenerSqlSingleQueryM4() =>
        ConsultaDosColeccionesM4().AsSingleQuery().ToQueryString();

    public string ObtenerSqlSplitQueryM4() =>
        ConsultaDosColeccionesM4().AsSplitQuery().ToQueryString();

    /*
    // RETO M04 4.8 - CONSULTA SIN OVERRIDE EXPLICITO
    public SplitQueryMetricaDto MedirComportamientoGlobalM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }
    */

}


// FRAGMENTO PDF M04 4.8 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     private IQueryable<OrdenFabricacion> ConsultaDosColeccionesM4() =>
//         _context.OrdenesFabricacion
//             .AsNoTrackingWithIdentityResolution()
//             .Include(o => o.Planchas)
//             .Include(o => o.OrdenesAleaciones)
//                 .ThenInclude(oa => oa.Aleacion)
//             .OrderBy(o => o.Id);
//
//     public SplitQueryMetricaDto MedirSingleQueryM4()
//     {
//         SqlCommandCounterInterceptor.Instance.Reset();
//         var ordenes = ConsultaDosColeccionesM4().AsSingleQuery().ToList();
//
//         return new SplitQueryMetricaDto
//         {
//             Ordenes = ordenes.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             Planchas = ordenes.Sum(o => o.Planchas.Count),
//             RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
//         };
//     }
//
//     public SplitQueryMetricaDto MedirSplitQueryM4()
//     {
//         SqlCommandCounterInterceptor.Instance.Reset();
//         var ordenes = ConsultaDosColeccionesM4().AsSplitQuery().ToList();
//
//         return new SplitQueryMetricaDto
//         {
//             Ordenes = ordenes.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             Planchas = ordenes.Sum(o => o.Planchas.Count),
//             RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
//         };
//     }
//
//     public string ObtenerSqlSingleQueryM4() =>
//         ConsultaDosColeccionesM4().AsSingleQuery().ToQueryString();
//
//     public string ObtenerSqlSplitQueryM4() =>
//         ConsultaDosColeccionesM4().AsSplitQuery().ToQueryString();
// }
// ========================================================================

// CANONICAL INLINE M04 4.8 - BLOCK 06
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerConVariasColeccionesSingleQuery para eliminar AsNoTrackingWithIdentityResolution y usar AsNoTracking:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConVariasColeccionesSingleQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSingleQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL INLINE M04 4.8 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConVariasColeccionesSingleQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSingleQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// Resultado esperado con la solución: las entidades relacionadas se resuelven a instancias únicas.
//
// ========================================================================
