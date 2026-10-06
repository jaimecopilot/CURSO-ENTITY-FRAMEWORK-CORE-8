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
