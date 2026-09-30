using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private IQueryable<OrdenFabricacion> ConsultaDosColeccionesM4() =>
        _context.OrdenesFabricacion
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Planchas)
            .Include(o => o.OrdenesAleaciones)
                .ThenInclude(oa => oa.Aleacion)
            .OrderBy(o => o.Id);

    public SplitQueryMetricaDto MedirSingleQueryM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().AsSingleQuery().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }

    public SplitQueryMetricaDto MedirSplitQueryM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().AsSplitQuery().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }

    public string ObtenerSqlSingleQueryM4() =>
        ConsultaDosColeccionesM4().AsSingleQuery().ToQueryString();

    public string ObtenerSqlSplitQueryM4() =>
        ConsultaDosColeccionesM4().AsSplitQuery().ToQueryString();
}
