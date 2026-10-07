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


// EJEMPLO DEL PASO 4
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

// VARIANTE COMENTADA - BLOQUE 02
// PASO: Paso 3: Implementar los métodos de solución en el repositorio
// UBICACIÓN INDICADA: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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

// VARIANTE COMENTADA - BLOQUE 07
// PASO: Paso 10: Diagnosticar un error común
// UBICACIÓN INDICADA: Modificar el método ObtenerConPlanchasYDetalleSplitQuery para eliminar AsSplitQuery:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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

// VARIANTE COMENTADA - BLOQUE 08
// PASO: Paso 10: Diagnosticar un error común
// UBICACIÓN INDICADA: Paso 10: Diagnosticar un error común
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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

// VARIANTE COMENTADA - BLOQUE 10
// PASO: Paso 2: Implementar el método en OrdenRepositorio:
// UBICACIÓN INDICADA: Paso 2: Implementar el método en OrdenRepositorio:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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
