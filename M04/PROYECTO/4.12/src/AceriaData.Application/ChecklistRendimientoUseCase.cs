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

    /*
    // ERROR CONTROLADO M04 4.12 - ENTIDAD COMPLETA CON TRACKING
    public void EjecutarErrorChecklistIneficiente()
    {
        var r = _unidad.Ordenes.EjecutarChecklistIneficienteM4();

        if (r.Filas != 5 || r.ConsultasSql != 1 || r.EntidadesRastreadas != 5)
            throw new InvalidOperationException(
                "4.12 error controlado: no se observó tracking completo sobre las cinco filas.");

        if (!r.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.12 error controlado: la entidad completa no demuestra over-fetching.");

        Console.WriteLine(
            $"Error controlado 4.12 OK | filas={r.Filas} | consultas={r.ConsultasSql} | tracking={r.EntidadesRastreadas} | entidad completa");
        Console.WriteLine(r.Sql);
    }
    */

    /*
    // RETO M04 4.12 - AUDITORIA COMPLETA DE DECISIONES
    public void EjecutarRetoAuditoria()
    {
        var r = _unidad.Ordenes.EjecutarChecklistFinalM4();

        if (r.Filas != 5 || r.ConsultasSql != 1 || r.EntidadesRastreadas != 0)
            throw new InvalidOperationException("Reto 4.12: las métricas finales no son reproducibles.");

        if (r.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase) ||
            !r.Sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Reto 4.12: la forma SQL no coincide con la auditoría.");

        Console.WriteLine(
            "Reto 4.12 OK | aplicadas=proyeccion,AsNoTracking,orden,Take,TagWith | descartadas=Include,SplitQuery,CompiledQuery");
        Console.WriteLine(
            $"Evidencia: comandos={r.ConsultasSql} | tracking={r.EntidadesRastreadas} | filas={r.Filas} | sin Observaciones | ORDER BY presente");
        Console.WriteLine(r.DecisionLoading);
        Console.WriteLine(r.DecisionCompiledQuery);
    }
    */
}


// FRAGMENTO PDF M04 4.12 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class ChecklistRendimientoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public ChecklistRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.12 CHECKLIST FINAL DE OPTIMIZACION ===");
//         var r = _unidad.Ordenes.EjecutarChecklistFinalM4();
//
//         if (r.Filas == 0 || r.ConsultasSql != 1 || r.EntidadesRastreadas != 0)
//             throw new InvalidOperationException(
//                 "4.12: el cierre no cumple consulta unica/no-tracking.");
//         if (r.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException(
//                 "4.12: la consulta final incurre en over-fetching.");
//         if (!r.Sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException(
//                 "4.12: falta ordenacion determinista.");
//
//         Console.WriteLine(
//             $"Filas={r.Filas} | Consultas={r.ConsultasSql} | Tracking={r.EntidadesRastreadas}");
//         Console.WriteLine(r.DecisionLoading);
//         Console.WriteLine(r.DecisionCompiledQuery);
//         Console.WriteLine(r.Sql);
//     }
// }
// ========================================================================
