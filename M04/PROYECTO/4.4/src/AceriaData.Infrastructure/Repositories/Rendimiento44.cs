using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public NMasUnoMetricaDto EjecutarNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();

        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, o.NumeroOrden })
            .ToList();

        var totalPlanchas = 0;
        foreach (var orden in ordenes)
        {
            totalPlanchas += _context.PlanchasAcero
                .AsNoTracking()
                .Count(p => p.OrdenId == orden.Id);
        }

        return new NMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = totalPlanchas
        };
    }

    /*
    // ERROR CONTROLADO M04 4.4 - CONTADOR SIN RESET
    public NMasUnoMetricaDto EjecutarNMasUnoSinResetM4()
    {
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, o.NumeroOrden })
            .ToList();

        var totalPlanchas = 0;
        foreach (var orden in ordenes)
        {
            totalPlanchas += _context.PlanchasAcero
                .AsNoTracking()
                .Count(p => p.OrdenId == orden.Id);
        }

        return new NMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = totalPlanchas
        };
    }
    */

    /*
    // RETO M04 4.4 - N+1 SOBRE DETALLE POR ORDEN
    public (int Ordenes, int ConsultasSql, int Detalles) EjecutarNMasUnoDetalleM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();

        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, o.NumeroOrden })
            .ToList();

        var detalles = 0;
        foreach (var orden in ordenes)
        {
            detalles += _context.DetallesOrden
                .AsNoTracking()
                .Count(d => d.OrdenId == orden.Id);
        }

        return (
            ordenes.Count,
            checked((int)SqlCommandCounterInterceptor.Instance.Count),
            detalles);
    }
    */

}


// EJEMPLO DEL PASO 4 - RENDIMIENTO44
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public NMasUnoMetricaDto EjecutarNMasUnoM4()
//     {
//         SqlCommandCounterInterceptor.Instance.Reset();
//
//         var ordenes = _context.OrdenesFabricacion
//             .AsNoTracking()
//             .OrderBy(o => o.Id)
//             .Select(o => new { o.Id, o.NumeroOrden })
//             .ToList();
//
//         var totalPlanchas = 0;
//         foreach (var orden in ordenes)
//         {
//             totalPlanchas += _context.PlanchasAcero
//                 .AsNoTracking()
//                 .Count(p => p.OrdenId == orden.Id);
//         }
//
//         return new NMasUnoMetricaDto
//         {
//             Ordenes = ordenes.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             Planchas = totalPlanchas
//         };
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 02
// PASO: Paso 3: Implementar los métodos del problema N+1 en el repositorio
// UBICACIÓN INDICADA: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasN1()
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     foreach (var orden in ordenes)
//     {
//         var planchas = _context.PlanchasAcero
//             .AsNoTracking()
//             .Where(p => p.OrdenId == orden.Id)
//             .ToList();
//
//         orden.Planchas = planchas;
//     }
//
//     return ordenes;
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasSinN1()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConDetalleN1()
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     foreach (var orden in ordenes)
//     {
//         var detalle = _context.DetallesOrden
//             .AsNoTracking()
//             .FirstOrDefault(d => d.OrdenId == orden.Id);
//
//         orden.Detalle = detalle;
//     }
//
//     return ordenes;
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConDetalleSinN1()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 07
// PASO: Paso 10: Diagnosticar un error común
// UBICACIÓN INDICADA: Modificar el método ObtenerOrdenesConPlanchasSinN1 para usar AsNoTracking después del Include:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasSinN1()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================
