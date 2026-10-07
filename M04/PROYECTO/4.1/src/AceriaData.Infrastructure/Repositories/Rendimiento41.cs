using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public string ObtenerSqlPendientesOrdenadasM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion)
        .ThenBy(o => o.Id)
        .ToQueryString();

    public string ObtenerSqlConIncludeM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Include(o => o.Planchas)
        .Where(o => o.Cliente == "Constructora del Norte")
        .OrderBy(o => o.Id)
        .ToQueryString();

    public string ObtenerSqlConProyeccionM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion)
        .ThenBy(o => o.Id)
        .Select(o => new { o.NumeroOrden, o.Cliente })
        .ToQueryString();

    /*
    // ERROR CONTROLADO M04 4.1 - MATERIALIZAR ANTES DE TOQUERYSTRING
    public string ErrorMaterializarAntesDeToQueryStringM4()
    {
        var materializada = _context.OrdenesFabricacion
            .AsNoTracking()
            .ToList();

        return materializada.ToQueryString();
    }
    */

    /*
    // RETO M04 4.1 - DOS COLECCIONES Y SQL SIN MATERIALIZAR
    public string ObtenerSqlDosColeccionesM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Include(o => o.Planchas)
        .Include(o => o.OrdenesAleaciones)
        .OrderBy(o => o.Id)
        .ToQueryString();
    */

}


// EJEMPLO DEL PASO 4 - RENDIMIENTO41
// ------------------------------------------------------------------------
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public string ObtenerSqlPendientesOrdenadasM4() => _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .ToQueryString();
//
//     public string ObtenerSqlConIncludeM4() => _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .Where(o => o.Cliente == "Constructora del Norte")
//         .OrderBy(o => o.Id)
//         .ToQueryString();
//
//     public string ObtenerSqlConProyeccionM4() => _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Select(o => new { o.NumeroOrden, o.Cliente })
//         .ToQueryString();
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 02
// PASO: Paso 3: Implementar los métodos de análisis de SQL en el repositorio
// UBICACIÓN INDICADA: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasPendientes()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion);
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlConsultasConInclude()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Where(o => o.Cliente == "Constructora del Norte");
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlConsultasConProyeccion()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .Select(o => new { o.NumeroOrden, o.Cliente });
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlConsultasConFiltroGlobal()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Include(o => o.Planchas);
//
//     return consulta.ToQueryString();
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 07
// PASO: Paso 10: Diagnosticar un error común
// UBICACIÓN INDICADA: Modificar el método ObtenerSqlConsultasConProyeccion para llamar a ToList antes de ToQueryString:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasConProyeccion()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .Select(o => new { o.NumeroOrden, o.Cliente })
//         .ToList();
//
//     return "La consulta ya se ha ejecutado, no se puede obtener el SQL.";
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 08
// PASO: Paso 10: Diagnosticar un error común
// UBICACIÓN INDICADA: Paso 10: Diagnosticar un error común
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasConProyeccion()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .Select(o => new { o.NumeroOrden, o.Cliente });
//
//     return consulta.ToQueryString();
// }
// Resultado esperado con la solución: el método devuelve el SQL sin ejecutar la consulta.
//
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 10
// PASO: Paso 2: Implementar el método en OrdenRepositorio:
// UBICACIÓN INDICADA: Paso 2: Implementar el método en OrdenRepositorio:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasConMultiplesInclude()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .Where(o => o.Estado == "Pendiente");
//
//     return consulta.ToQueryString();
// }
// ========================================================================
