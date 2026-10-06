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


// FRAGMENTO PDF M04 4.1 - PASO 4 - RENDIMIENTO41
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
