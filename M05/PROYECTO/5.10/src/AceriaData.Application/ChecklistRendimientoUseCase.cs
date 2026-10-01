using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ChecklistRendimientoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ChecklistRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.12 CHECKLIST FINAL DE OPTIMIZACION ===");
        var r = _unidad.Ordenes.EjecutarChecklistFinalM4();

        if (r.Filas == 0 || r.ConsultasSql != 1 || r.EntidadesRastreadas != 0)
            throw new InvalidOperationException(
                "4.12: el cierre no cumple consulta unica/no-tracking.");
        if (r.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.12: la consulta final incurre en over-fetching.");
        if (!r.Sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.12: falta ordenacion determinista.");

        Console.WriteLine(
            $"Filas={r.Filas} | Consultas={r.ConsultasSql} | Tracking={r.EntidadesRastreadas}");
        Console.WriteLine(r.DecisionLoading);
        Console.WriteLine(r.DecisionCompiledQuery);
        Console.WriteLine(r.Sql);
    }
}
