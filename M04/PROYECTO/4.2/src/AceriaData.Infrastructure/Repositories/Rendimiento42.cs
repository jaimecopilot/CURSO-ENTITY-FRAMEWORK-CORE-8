using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public TrackingMetricaDto MedirConsultaConTrackingM4()
    {
        _context.ChangeTracker.Clear();
        var consulta = _context.OrdenesFabricacion
            .AsTracking()
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }

    public TrackingMetricaDto MedirConsultaSinTrackingM4()
    {
        _context.ChangeTracker.Clear();
        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }

    /*
    // ERROR CONTROLADO M04 4.2 - NOTRACKING SIN LIMPIAR ESTADO PREVIO
    public TrackingMetricaDto MedirConsultaSinTrackingSinLimpiarM4()
    {
        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }
    */

    /*
    // RETO M04 4.2 - GRAFO CON Y SIN TRACKING
    public TrackingMetricaDto MedirGrafoConTrackingM4()
    {
        _context.ChangeTracker.Clear();
        var consulta = _context.OrdenesFabricacion
            .AsTracking()
            .Include(o => o.Planchas)
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }

    public TrackingMetricaDto MedirGrafoSinTrackingM4()
    {
        _context.ChangeTracker.Clear();
        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .Include(o => o.Planchas)
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }
    */

}


// FRAGMENTO PDF M04 4.2 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public TrackingMetricaDto MedirConsultaConTrackingM4()
//     {
//         _context.ChangeTracker.Clear();
//         var consulta = _context.OrdenesFabricacion
//             .AsTracking()
//             .OrderBy(o => o.Id);
//         var sql = consulta.ToQueryString();
//         var filas = consulta.ToList().Count;
//         return new TrackingMetricaDto
//         {
//             Filas = filas,
//             EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
//             Sql = sql
//         };
//     }
//
//     public TrackingMetricaDto MedirConsultaSinTrackingM4()
//     {
//         _context.ChangeTracker.Clear();
//         var consulta = _context.OrdenesFabricacion
//             .AsNoTracking()
//             .OrderBy(o => o.Id);
//         var sql = consulta.ToQueryString();
//         var filas = consulta.ToList().Count;
//         return new TrackingMetricaDto
//         {
//             Filas = filas,
//             EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
//             Sql = sql
//         };
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de tracking en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConTracking()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerSinTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public int ContarEntidadesRastreadas()
// {
//     return _context.ChangeTracker.Entries().Count();
// }
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 09
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasConTracking()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConPlanchasSinTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================
