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


// FRAGMENTO PDF M04 4.4 - PASO 4 - RENDIMIENTO44
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
