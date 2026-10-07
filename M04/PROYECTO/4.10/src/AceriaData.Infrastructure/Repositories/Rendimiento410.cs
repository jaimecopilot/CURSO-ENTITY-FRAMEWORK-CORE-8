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

// CANONICAL INLINE M04 4.10 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de paginación en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPaginadoOffset(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
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
// public List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha || (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Take(tamanoPagina)
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
// public int ContarOrdenesPaginadas()
// {
//     return _context.OrdenesFabricacion.Count();
// }
//
// public string ObtenerSqlPaginadoOffset(int pagina, int tamanoPagina)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         });
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha || (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         });
//
//     return consulta.ToQueryString();
// }
// ========================================================================

// CANONICAL INLINE M04 4.10 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerPaginadoKeyset para usar solo la fecha como clave:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha)
//         .OrderBy(o => o.FechaCreacion)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion \        })
//         .ToList();
// }
// ========================================================================

// CANONICAL INLINE M04 4.10 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha || (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// Resultado esperado con la solución: la paginación devuelve las filas correctas aunque haya fechas duplicadas.
//
// ========================================================================

// CANONICAL INLINE M04 4.10 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPendientesPaginado(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// ========================================================================
