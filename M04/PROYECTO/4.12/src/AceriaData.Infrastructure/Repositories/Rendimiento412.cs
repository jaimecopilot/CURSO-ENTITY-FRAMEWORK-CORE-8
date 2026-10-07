using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public ChecklistRendimientoDto EjecutarChecklistFinalM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consulta = _context.OrdenesFabricacion
            .TagWith("M4.12-CHECKLIST-FINAL")
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .Take(5);

        var sql = consulta.ToQueryString();
        var filas = consulta.ToList();

        return new ChecklistRendimientoDto
        {
            Filas = filas.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql,
            DecisionCompiledQuery =
                "No se aplica: esta consulta final no se ha demostrado como hot path; medir antes de introducir complejidad.",
            DecisionLoading =
                "Proyeccion escalar: no se cargan navegaciones, por lo que Include/SplitQuery no aportan valor en esta consulta."
        };
    }

    /*
    // ERROR CONTROLADO M04 4.12 - ENTIDAD COMPLETA CON TRACKING
    public ChecklistRendimientoDto EjecutarChecklistIneficienteM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consulta = _context.OrdenesFabricacion
            .TagWith("M4.12-ERROR-CHECKLIST")
            .AsTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Take(5);

        var sql = consulta.ToQueryString();
        var filas = consulta.ToList();

        return new ChecklistRendimientoDto
        {
            Filas = filas.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql,
            DecisionCompiledQuery = "Error controlado: aplicar técnicas por costumbre no sustituye la medición.",
            DecisionLoading = "Error controlado: se materializa la entidad completa aunque la salida solo necesita un resumen."
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
//     public ChecklistRendimientoDto EjecutarChecklistFinalM4()
//     {
//         _context.ChangeTracker.Clear();
//         SqlCommandCounterInterceptor.Instance.Reset();
//
//         var consulta = _context.OrdenesFabricacion
//             .TagWith("M4.12-CHECKLIST-FINAL")
//             .AsNoTracking()
//             .Where(o => o.Estado == "Pendiente")
//             .OrderBy(o => o.FechaCreacion)
//             .ThenBy(o => o.Id)
//             .Select(o => new OrdenPaginaDto
//             {
//                 Id = o.Id,
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .Take(5);
//
//         var sql = consulta.ToQueryString();
//         var filas = consulta.ToList();
//
//         return new ChecklistRendimientoDto
//         {
//             Filas = filas.Count,
//             ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
//             EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
//             Sql = sql,
//             DecisionCompiledQuery =
//                 "No se aplica: esta consulta final no se ha demostrado como hot path; medir antes de introducir complejidad.",
//             DecisionLoading =
//                 "Proyeccion escalar: no se cargan navegaciones, por lo que Include/SplitQuery no aportan valor en esta consulta."
//         };
//     }
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 02
// SECTION: Paso 3: Implementar los métodos del checklist en el repositorio
// UBICACION EN EL EJERCICIO: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerSinOptimizar()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// /// <summary>
// /// Obtiene las órdenes con optimización de tracking.
// /// Usa AsNoTracking porque es una consulta de solo lectura.
// /// </summary>
// public List<OrdenFabricacion> ObtenerOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// /// <summary>
// /// Obtiene los resúmenes de las órdenes.
// /// Usa proyección para reducir el volumen de datos transferidos.
// /// Usa AsNoTracking porque es una consulta de solo lectura.
// /// </summary>
// public List<OrdenResumenDto> ObtenerResumenOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
//
// /// <summary>
// /// Obtiene las órdenes con planchas y detalle.
// /// Usa Include para evitar N+1.
// /// Usa AsSplitQuery para evitar el producto cartesiano.
// /// Usa AsNoTrackingWithIdentityResolution para evitar instancias duplicadas.
// /// </summary>
// public List<OrdenFabricacion> ObtenerConRelacionesOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 07
// SECTION: Paso 10: Diagnosticar un error común
// UBICACION EN EL EJERCICIO: Modificar el método ObtenerConRelacionesOptimizado para eliminar AsSplitQuery:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConRelacionesOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 08
// SECTION: Paso 10: Diagnosticar un error común
// UBICACION EN EL EJERCICIO: Paso 10: Diagnosticar un error común
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConRelacionesOptimizado()
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

// EJEMPLO COMPLEMENTARIO - BLOQUE 09
// SECTION: Paso 1: Consulta sin optimizar:
// UBICACION EN EL EJERCICIO: Paso 1: Consulta sin optimizar:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerSinOptimizarCompleta()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .ToList();
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 10
// SECTION: Paso 2: Consulta optimizada con el checklist completo:
// UBICACION EN EL EJERCICIO: Paso 2: Consulta optimizada con el checklist completo:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// /// <summary>
// /// Obtiene las órdenes pendientes con planchas y detalle.
// /// Usa AsNoTrackingWithIdentityResolution para evitar tracking y duplicados.
// /// Usa Include para evitar N+1.
// /// Usa AsSplitQuery para evitar el producto cartesiano.
// /// Usa filtro por estado para reducir el número de filas.
// /// Usa paginación para limitar el número de resultados.
// /// </summary>
// public List<OrdenFabricacion> ObtenerOptimizadoCompleta(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .ToList();
// }
// ========================================================================
