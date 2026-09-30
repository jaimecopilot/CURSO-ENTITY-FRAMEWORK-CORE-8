using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public ChecklistRendimientoDto EjecutarChecklistFinalM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consulta = _context.OrdenesFabricacion
            .TagWith("M4.12-CHECKLIST-FINAL")
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .Take(5);

        var sql = consulta.ToQueryString();
        var filas = consulta.ToList();

        return new ChecklistRendimientoDto
        {
            Filas = filas.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql,
            DecisionCompiledQuery =
                "No se aplica: esta consulta final no se ha demostrado como hot path; medir antes de introducir complejidad.",
            DecisionLoading =
                "Proyeccion escalar: no se cargan navegaciones, por lo que Include/SplitQuery no aportan valor en esta consulta."
        };
    }
}
