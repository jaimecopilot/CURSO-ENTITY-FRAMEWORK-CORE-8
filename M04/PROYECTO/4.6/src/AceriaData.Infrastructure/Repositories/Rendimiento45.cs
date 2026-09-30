using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public SolucionNMasUnoMetricaDto EjecutarIncludeContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .Include(o => o.Planchas)
            .OrderBy(o => o.Id)
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count)
        };
    }

    public SolucionNMasUnoMetricaDto EjecutarProyeccionContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, TotalPlanchas = o.Planchas.Count })
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.TotalPlanchas)
        };
    }

    public SolucionNMasUnoMetricaDto EjecutarSplitQueryContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Planchas)
            .Include(o => o.OrdenesAleaciones)
                .ThenInclude(oa => oa.Aleacion)
            .AsSplitQuery()
            .OrderBy(o => o.Id)
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count + o.OrdenesAleaciones.Count)
        };
    }
}
