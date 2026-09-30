using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class DiagnosticoRendimientoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public DiagnosticoRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.11 DIAGNOSTICO DE RENDIMIENTO ===");
        var d = _unidad.Ordenes.DiagnosticarPendientesM4();

        if (d.Filas == 0 || d.ConsultasSql != 1 || d.EntidadesRastreadas != 0)
            throw new InvalidOperationException(
                "4.11: las metricas observables no coinciden con la consulta optimizada.");
        if (!d.Sql.Contains("M4.11-DIAGNOSTICO", StringComparison.Ordinal))
            throw new InvalidOperationException("4.11: falta TagWith en el SQL de diagnostico.");

        Console.WriteLine(
            $"Filas={d.Filas} | SQL commands={d.ConsultasSql} | Tracking={d.EntidadesRastreadas} | Ticks={d.Ticks}");
        Console.WriteLine(d.Sql);
    }
}
