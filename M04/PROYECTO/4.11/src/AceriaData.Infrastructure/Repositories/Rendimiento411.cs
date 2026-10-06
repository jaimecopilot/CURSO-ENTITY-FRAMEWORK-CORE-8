using System.Diagnostics;
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public DiagnosticoRendimientoDto DiagnosticarPendientesM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consulta = _context.OrdenesFabricacion
            .TagWith("M4.11-DIAGNOSTICO")
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .Take(10);

        var sql = consulta.ToQueryString();

        var sw = Stopwatch.StartNew();
        var filas = consulta.ToList();
        sw.Stop();

        return new DiagnosticoRendimientoDto
        {
            Filas = filas.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Ticks = sw.ElapsedTicks,
            Sql = sql
        };
    }

    /*
    // ERROR CONTROLADO M04 4.11 - CONTADOR MANUAL NO REPRESENTA ROUNDTRIPS
    public DiagnosticoContadorManualDto DiagnosticarConContadorManualM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var contadorManual = 0;

        contadorManual++;
        _ = _context.OrdenesFabricacion
            .AsNoTracking()
            .Count(o => o.Estado == "Pendiente");

        // Segundo roundtrip real olvidado por el contador manual.
        _ = _context.OrdenesFabricacion
            .AsNoTracking()
            .Count(o => o.Estado == "EnProceso");

        return new DiagnosticoContadorManualDto
        {
            ContadorManual = contadorManual,
            ComandosReales = checked((int)SqlCommandCounterInterceptor.Instance.Count)
        };
    }
    */

    /*
    // RETO M04 4.11 - DIAGNOSTICLISTENER CON UMBRAL CONFIGURABLE
    public DiagnosticoListenerDto DiagnosticarConDiagnosticListenerM4(double umbralMs)
    {
        if (umbralMs < 0) throw new ArgumentOutOfRangeException(nameof(umbralMs));

        using var observer = EfCommandDiagnosticObserver.Start(TimeSpan.FromMilliseconds(umbralMs));
        SqlCommandCounterInterceptor.Instance.Reset();

        var filas = _context.OrdenesFabricacion
            .TagWith("M4.11-RETO-DIAGNOSTICLISTENER")
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Take(10)
            .ToList();

        return new DiagnosticoListenerDto
        {
            Filas = filas.Count,
            ComandosReales = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ComandosObservados = observer.CommandExecutedCount,
            ConsultasLentas = observer.SlowQueryCount,
            UmbralMs = umbralMs
        };
    }
    */
}


// FRAGMENTO PDF M04 4.11 - PASO 4
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Dtos;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public DiagnosticoRendimientoDto DiagnosticarPendientesM4()
//     {
//         _context.ChangeTracker.Clear();
//         SqlCommandCounterInterceptor.Instance.Reset();
//
//         var consulta = _context.OrdenesFabricacion
//             .TagWith("M4.11-DIAGNOSTICO")
//             .AsNoTracking()
//             .Where(o => o.Estado == "Pendiente")
//             .OrderBy(o => o.FechaCreacion)
//             .ThenBy(o => o.Id)
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .Take(10);
//
//         var sql = consulta.ToQueryString();
//
//         var sw = Stopwatch.StartNew();
//         var filas = consulta.ToList();
//         sw.Stop();
//
//         return new DiagnosticoRendimientoDto
//         {
//             Filas = filas.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
//             Ticks = sw.ElapsedTicks,
//             Sql = sql
//         };
//     }
// }
// ========================================================================
