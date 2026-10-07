using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public SolucionNMasUnoMetricaDto EjecutarIncludeContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .Include(o => o.Planchas)
            .OrderBy(o => o.Id)
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count)
        };
    }

    public SolucionNMasUnoMetricaDto EjecutarProyeccionContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, TotalPlanchas = o.Planchas.Count })
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.TotalPlanchas)
        };
    }

    public SolucionNMasUnoMetricaDto EjecutarSplitQueryContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Planchas)
            .Include(o => o.OrdenesAleaciones)
                .ThenInclude(oa => oa.Aleacion)
            .AsSplitQuery()
            .OrderBy(o => o.Id)
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count + o.OrdenesAleaciones.Count)
        };
    }
}


// FRAGMENTO PDF M04 4.5 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public SolucionNMasUnoMetricaDto EjecutarIncludeContraNMasUnoM4()
//     {
//         SqlCommandCounterInterceptor.Instance.Reset();
//         var ordenes = _context.OrdenesFabricacion
//             .AsNoTracking()
//             .Include(o => o.Planchas)
//             .OrderBy(o => o.Id)
//             .ToList();
//
//         return new SolucionNMasUnoMetricaDto
//         {
//             Ordenes = ordenes.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count)
//         };
//     }
//
//     public SolucionNMasUnoMetricaDto EjecutarProyeccionContraNMasUnoM4()
//     {
//         SqlCommandCounterInterceptor.Instance.Reset();
//         var ordenes = _context.OrdenesFabricacion
//             .AsNoTracking()
//             .OrderBy(o => o.Id)
//             .Select(o => new { o.Id, TotalPlanchas = o.Planchas.Count })
//             .ToList();
//
//         return new SolucionNMasUnoMetricaDto
//         {
//             Ordenes = ordenes.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             ElementosRelacionados = ordenes.Sum(o => o.TotalPlanchas)
//         };
//     }
//
//     public SolucionNMasUnoMetricaDto EjecutarSplitQueryContraNMasUnoM4()
//     {
//         SqlCommandCounterInterceptor.Instance.Reset();
//         var ordenes = _context.OrdenesFabricacion
//             .AsNoTrackingWithIdentityResolution()
//             .Include(o => o.Planchas)
//             .Include(o => o.OrdenesAleaciones)
//                 .ThenInclude(oa => oa.Aleacion)
//             .AsSplitQuery()
//             .OrderBy(o => o.Id)
//             .ToList();
//
//         return new SolucionNMasUnoMetricaDto
//         {
//             Ordenes = ordenes.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count + o.OrdenesAleaciones.Count)
//         };
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.5 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de solución en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasInclude()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConAleacionesThenInclude()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenConPlanchasYDetalleDto> ObtenerResumenConPlanchasYDetalleProyeccion()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConPlanchasYDetalleDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             Planchas = o.Planchas.Select(p => new PlanchaDto
//             {
//                 Id = p.Id,
//                 Espesor = p.Espesor,
//                 Ancho = p.Ancho,
//                 Largo = p.Largo,
//                 Peso = p.Peso,
//                 Activa = p.Activa
//             }).ToList(),
//             Detalle = o.Detalle == null ? null : new DetalleDto
//             {
//                 ComposicionQuimica = o.Detalle.ComposicionQuimica,
//                 TemperaturaColada = o.Detalle.TemperaturaColada,
//                 Notas = o.Detalle.Notas
//             }
//         })
//         .ToList();
// }
// ========================================================================

// CANONICAL INLINE M04 4.5 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerConPlanchasYDetalleSplitQuery para eliminar AsSplitQuery:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL INLINE M04 4.5 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// Resultado esperado con la solución: la consulta se divide en varias y no se produce el producto cartesiano.
//
// ========================================================================

// CANONICAL INLINE M04 4.5 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesCompletasConSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================
