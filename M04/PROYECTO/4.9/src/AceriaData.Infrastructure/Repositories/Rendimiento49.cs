using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private static readonly Func<AceriaDbContext, string, IEnumerable<OrdenFabricacion>>
        ConsultaCompiladaPorEstadoM4 =
            EF.CompileQuery(
                (AceriaDbContext context, string estado) =>
                    context.OrdenesFabricacion
                        .AsNoTracking()
                        .Where(o => o.Estado == estado)
                        .OrderBy(o => o.FechaCreacion)
                        .ThenBy(o => o.Id)
                        .Select(o => o));

    public List<OrdenFabricacion> ObtenerPorEstadoNormalM4(string estado) =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Estado == estado)
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .ToList();

    public List<OrdenFabricacion> ObtenerPorEstadoCompiladoM4(string estado) =>
        ConsultaCompiladaPorEstadoM4(_context, estado).ToList();

    /*
    // ERROR CONTROLADO M04 4.9 - COMPILAR EN CADA LLAMADA
    private static int _compilacionesPorLlamadaM4;

    public (List<OrdenFabricacion> Filas, int Compilaciones) ObtenerCompilandoCadaVezM4(string estado)
    {
        var consulta = EF.CompileQuery(
            (AceriaDbContext context, string valorEstado) =>
                context.OrdenesFabricacion
                    .AsNoTracking()
                    .Where(o => o.Estado == valorEstado)
                    .OrderBy(o => o.FechaCreacion)
                    .ThenBy(o => o.Id)
                    .Select(o => o));

        var compilaciones = Interlocked.Increment(ref _compilacionesPorLlamadaM4);
        return (consulta(_context, estado).ToList(), compilaciones);
    }
    */

    /*
    // RETO M04 4.9 - COMPILED ASYNC QUERY PROYECTADA
    private static readonly Func<AceriaDbContext, string, IAsyncEnumerable<AceriaData.Application.Dtos.OrdenResumenDto>>
        ConsultaCompiladaProyectadaAsyncM4 =
            EF.CompileAsyncQuery(
                (AceriaDbContext context, string estado) =>
                    context.OrdenesFabricacion
                        .AsNoTracking()
                        .Where(o => o.Estado == estado)
                        .OrderBy(o => o.FechaCreacion)
                        .ThenBy(o => o.Id)
                        .Select(o => new AceriaData.Application.Dtos.OrdenResumenDto
                        {
                            NumeroOrden = o.NumeroOrden,
                            Cliente = o.Cliente,
                            Estado = o.Estado,
                            FechaCreacion = o.FechaCreacion
                        }));

    public async Task<List<AceriaData.Application.Dtos.OrdenResumenDto>> ObtenerResumenesCompiladosAsyncM4(string estado)
    {
        var elementos = new List<AceriaData.Application.Dtos.OrdenResumenDto>();
        await foreach (var elemento in ConsultaCompiladaProyectadaAsyncM4(_context, estado))
            elementos.Add(elemento);

        return elementos;
    }
    */

}


// FRAGMENTO PDF M04 4.9 - PASO 4
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     private static readonly Func<AceriaDbContext, string, IEnumerable<OrdenFabricacion>>
//         ConsultaCompiladaPorEstadoM4 =
//             EF.CompileQuery(
//                 (AceriaDbContext context, string estado) =>
//                     context.OrdenesFabricacion
//                         .AsNoTracking()
//                         .Where(o => o.Estado == estado)
//                         .OrderBy(o => o.FechaCreacion)
//                         .ThenBy(o => o.Id)
//                         .Select(o => o));
//
//     public List<OrdenFabricacion> ObtenerPorEstadoNormalM4(string estado) =>
//         _context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == estado)
//             .OrderBy(o => o.FechaCreacion)
//             .ThenBy(o => o.Id)
//             .ToList();
//
//     public List<OrdenFabricacion> ObtenerPorEstadoCompiladoM4(string estado) =>
//         ConsultaCompiladaPorEstadoM4(_context, estado).ToList();
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 01
// SECTION: Paso 2: Crear la clase de consultas compiladas
// SOURCE TARGET: Crear el archivo src/AceriaData.Infrastructure/Repositories/OrdenConsultasCompiladas.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public static class OrdenConsultasCompiladas
// {
//     public static readonly Func<AceriaDbContext, string, List<OrdenFabricacion>> ObtenerPorEstado =
//         EF.CompileQuery((AceriaDbContext context, string estado) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => o.Estado == estado)
//                 .OrderBy(o => o.FechaCreacion)
//                 .ToList());
//
//     public static readonly Func<AceriaDbContext, string, List<OrdenFabricacion>> ObtenerPorCliente =
//         EF.CompileQuery((AceriaDbContext context, string cliente) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => o.Cliente == cliente)
//                 .OrderBy(o => o.FechaCreacion)
//                 .ToList());
//
//     public static readonly Func<AceriaDbContext, DateTime, DateTime, List<OrdenFabricacion>> ObtenerPorRangoDeFechas =
//         EF.CompileQuery((AceriaDbContext context, DateTime desde, DateTime hasta) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
//                 .OrderBy(o => o.FechaCreacion)
//                 .ToList());
//
//     public static readonly Func<AceriaDbContext, int, OrdenFabricacion?> ObtenerPorId =
//         EF.CompileQuery((AceriaDbContext context, int id) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .FirstOrDefault(o => o.Id == id));
//
//     public static readonly Func<AceriaDbContext, int> ContarOrdenes =
//         EF.CompileQuery((AceriaDbContext context) =>
//             context.OrdenesFabricacion.Count());
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 03
// SECTION: Paso 4: Implementar los métodos de Compiled Queries en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorEstadoCompilada(string estado)
// {
//     return OrdenConsultasCompiladas.ObtenerPorEstado(_context, estado);
// }
//
// public List<OrdenFabricacion> ObtenerPorEstadoNoCompilada(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.FechaCreacion)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorClienteCompilada(string cliente)
// {
//     return OrdenConsultasCompiladas.ObtenerPorCliente(_context, cliente);
// }
//
// public OrdenFabricacion? ObtenerPorIdCompilada(int id)
// {
//     return OrdenConsultasCompiladas.ObtenerPorId(_context, id);
// }
//
// public int ContarOrdenesCompilada()
// {
//     return OrdenConsultasCompiladas.ContarOrdenes(_context);
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 08
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerPorEstadoCompilada para compilar la consulta cada vez que se llama:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorEstadoCompilada(string estado)
// {
//     var consultaCompilada = EF.CompileQuery((AceriaDbContext context, string e) =>
//         context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == e)
//             .OrderBy(o => o.FechaCreacion)
//             .ToList());
//
//     return consultaCompilada(_context, estado);
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 12
// SECTION: Paso 3: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 3: Implementar el método en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPorEstadoProyectadaCompilada(string estado)
// {
//     return OrdenConsultasCompiladas.ObtenerPorEstadoProyectada(_context, estado);
// }
// ========================================================================
