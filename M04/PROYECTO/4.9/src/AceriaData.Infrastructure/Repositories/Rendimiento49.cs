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
