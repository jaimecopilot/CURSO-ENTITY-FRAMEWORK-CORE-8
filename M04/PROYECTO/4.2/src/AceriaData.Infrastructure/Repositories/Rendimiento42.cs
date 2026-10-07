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


// EJEMPLO DEL PASO 4
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

// VARIANTE COMENTADA - BLOQUE 02
// PASO: Paso 3: Implementar los métodos de tracking en el repositorio
// UBICACIÓN INDICADA: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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

// VARIANTE COMENTADA - BLOQUE 09
// PASO: Paso 2: Implementar los métodos en OrdenRepositorio:
// UBICACIÓN INDICADA: Paso 2: Implementar los métodos en OrdenRepositorio:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
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
