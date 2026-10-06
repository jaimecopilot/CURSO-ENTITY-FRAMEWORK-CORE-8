using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public PaginaOrdenesDto ObtenerPaginaOffsetM4(int pagina, int tamano)
    {
        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));

        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            });

        return new PaginaOrdenesDto
        {
            Sql = consulta.ToQueryString(),
            Elementos = consulta.ToList()
        };
    }

    public PaginaOrdenesDto ObtenerPaginaKeysetM4(
        DateTime ultimaFecha,
        int ultimoId,
        int tamano)
    {
        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));

        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o =>
                o.FechaCreacion > ultimaFecha ||
                (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Take(tamano)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            });

        return new PaginaOrdenesDto
        {
            Sql = consulta.ToQueryString(),
            Elementos = consulta.ToList()
        };
    }

    /*
    // ERROR CONTROLADO M04 4.10 - CURSOR NO UNICO
    public PaginaOrdenesDto ObtenerPaginaKeysetSoloEstadoM4(string ultimoEstado, int tamano)
    {
        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));

        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => string.Compare(o.Estado, ultimoEstado) > 0)
            .OrderBy(o => o.Estado)
            .Take(tamano)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            });

        return new PaginaOrdenesDto
        {
            Sql = consulta.ToQueryString(),
            Elementos = consulta.ToList()
        };
    }
    */

    /*
    // RETO M04 4.10 - KEYSET FILTRADO POR ESTADO CON CURSOR COMPUESTO
    public PaginaOrdenesDto ObtenerPaginaKeysetPorEstadoM4(
        string estado,
        DateTime ultimaFecha,
        int ultimoId,
        int tamano)
    {
        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));

        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Estado == estado)
            .Where(o =>
                o.FechaCreacion > ultimaFecha ||
                (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Take(tamano)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            });

        return new PaginaOrdenesDto
        {
            Sql = consulta.ToQueryString(),
            Elementos = consulta.ToList()
        };
    }
    */

}


// FRAGMENTO PDF M04 4.10 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     public PaginaOrdenesDto ObtenerPaginaOffsetM4(int pagina, int tamano)
//     {
//         if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
//         if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));
//
//         var consulta = _context.OrdenesFabricacion
//             .AsNoTracking()
//             .OrderBy(o => o.FechaCreacion)
//             .ThenBy(o => o.Id)
//             .Skip((pagina - 1) * tamano)
//             .Take(tamano)
//             .Select(o => new OrdenPaginaDto
//             {
//                 Id = o.Id,
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             });
//
//         return new PaginaOrdenesDto
//         {
//             Sql = consulta.ToQueryString(),
//             Elementos = consulta.ToList()
//         };
//     }
//
//     public PaginaOrdenesDto ObtenerPaginaKeysetM4(
//         DateTime ultimaFecha,
//         int ultimoId,
//         int tamano)
//     {
//         if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));
//
//         var consulta = _context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o =>
//                 o.FechaCreacion > ultimaFecha ||
//                 (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//             .OrderBy(o => o.FechaCreacion)
//             .ThenBy(o => o.Id)
//             .Take(tamano)
//             .Select(o => new OrdenPaginaDto
//             {
//                 Id = o.Id,
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             });
//
//         return new PaginaOrdenesDto
//         {
//             Sql = consulta.ToQueryString(),
//             Elementos = consulta.ToList()
//         };
//     }
// }
// ========================================================================
