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
}
