using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private IQueryable<OrdenFabricacion> PendientesM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion)
        .ThenBy(o => o.Id);

    public List<OrdenFabricacion> ObtenerPendientesEntidadCompletaM4() =>
        PendientesM4().ToList();

    public List<OrdenResumenDto> ObtenerPendientesProyectadasM4() =>
        PendientesM4()
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .ToList();

    public string ObtenerSqlPendientesEntidadCompletaM4() =>
        PendientesM4().ToQueryString();

    public string ObtenerSqlPendientesProyectadasM4() =>
        PendientesM4()
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .ToQueryString();

    /*
    // ERROR CONTROLADO M04 4.6 - MATERIALIZAR ANTES DE PROYECTAR
    public (int Filas, string Sql) MaterializarAntesDeProyectarM4()
    {
        var consultaCompleta = PendientesM4();
        var sqlEjecutado = consultaCompleta.ToQueryString();

        var materializadas = consultaCompleta.ToList();
        var proyectadasEnMemoria = materializadas
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .ToList();

        return (proyectadasEnMemoria.Count, sqlEjecutado);
    }
    */

}


// FRAGMENTO PDF M04 4.6 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     private IQueryable<OrdenFabricacion> PendientesM4() => _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id);
//
//     public List<OrdenFabricacion> ObtenerPendientesEntidadCompletaM4() =>
//         PendientesM4().ToList();
//
//     public List<OrdenResumenDto> ObtenerPendientesProyectadasM4() =>
//         PendientesM4()
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .ToList();
//
//     public string ObtenerSqlPendientesEntidadCompletaM4() =>
//         PendientesM4().ToQueryString();
//
//     public string ObtenerSqlPendientesProyectadasM4() =>
//         PendientesM4()
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .ToQueryString();
// }
// ========================================================================

// CANONICAL INLINE M04 4.6 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de over-fetching en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesCompletas()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenResumenDto> ObtenerResumenesProyectados()
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
// public List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
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
// public List<OrdenResumenDto> ObtenerResumenesFiltradosYProyectados(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.FechaCreacion)
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

// CANONICAL INLINE M04 4.6 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerResumenesPaginados para aplicar ToList antes del Skip y Take:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList()
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

// CANONICAL INLINE M04 4.6 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
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
// Resultado esperado con la solución: el SQL incluye OFFSET y FETCH y solo se transfieren las filas de la página.
//
// ========================================================================

// CANONICAL INLINE M04 4.6 - BLOCK 10
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public string ObtenerSqlEntidadesCompletas()
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden);
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlProyeccionResumen()
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
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
