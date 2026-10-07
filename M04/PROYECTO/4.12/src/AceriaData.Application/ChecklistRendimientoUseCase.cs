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

// CANONICAL INLINE M04 4.12 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso del checklist de rendimiento
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ChecklistRendimientoUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ChecklistRendimientoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ChecklistRendimientoUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CHECKLIST DE RENDIMIENTO ===");
//
//         MostrarChecklist();
//         CompararSinOptimizarVsOptimizado();
//         CompararEntidadesVsProyeccion();
//         CompararSinIncludeVsConInclude();
//     }
//
//     private void MostrarChecklist()
//     {
//         Console.WriteLine("\n--- Checklist de rendimiento ---");
//         Console.WriteLine("1. ¿Se usa AsNoTracking en consultas de solo lectura?");
//         Console.WriteLine("2. ¿Se proyectan solo las columnas necesarias?");
//         Console.WriteLine("3. ¿Se evita el problema N+1 con Include?");
//         Console.WriteLine("4. ¿Se evita el producto cartesiano con AsSplitQuery?");
//         Console.WriteLine("5. ¿Se aplican filtros y paginación en el servidor?");
//         Console.WriteLine("6. ¿Se evitan funciones en Where que impidan índices?");
//         Console.WriteLine("7. ¿Se usan Compiled Queries en consultas frecuentes?");
//         Console.WriteLine("8. ¿Se miden los tiempos y se cuentan las consultas?");
//     }
//
//     private void CompararSinOptimizarVsOptimizado()
//     {
//         Console.WriteLine("\n--- Sin optimizar vs optimizado (tracking) ---");
//
//         var cronometroSin = Stopwatch.StartNew();
//         var ordenesSin = _unidad.Ordenes.ObtenerSinOptimizar();
//         cronometroSin.Stop();
//
//         var cronometroCon = Stopwatch.StartNew();
//         var ordenesCon = _unidad.Ordenes.ObtenerOptimizado();
//         cronometroCon.Stop();
//
//         Console.WriteLine($"Sin optimizar: {cronometroSin.ElapsedMilliseconds} ms | Órdenes: {ordenesSin.Count}");
//         Console.WriteLine($"Optimizado: {cronometroCon.ElapsedMilliseconds} ms | Órdenes: {ordenesCon.Count}");
//     }
//
//     private void CompararEntidadesVsProyeccion()
//     {
//         Console.WriteLine("\n--- Entidades completas vs proyección ---");
//
//         var cronometroEntidades = Stopwatch.StartNew();
//         var ordenesEntidades = _unidad.Ordenes.ObtenerOptimizado();
//         cronometroEntidades.Stop();
//
//         var cronometroProyeccion = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenOptimizado();
//         cronometroProyeccion.Stop();
//
//         Console.WriteLine($"Entidades completas: {cronometroEntidades.ElapsedMilliseconds} ms | Órdenes: {ordenesEntidades.Count}");
//         Console.WriteLine($"Proyección: {cronometroProyeccion.ElapsedMilliseconds} ms | Resúmenes: {resumenes.Count}");
//     }
//
//     private void CompararSinIncludeVsConInclude()
//     {
//         Console.WriteLine("\n--- Sin Include vs con Include y AsSplitQuery ---");
//
//         var cronometroSinInclude = Stopwatch.StartNew();
//         var ordenesSinInclude = _unidad.Ordenes.ObtenerOptimizado();
//         cronometroSinInclude.Stop();
//
//         var cronometroConInclude = Stopwatch.StartNew();
//         var ordenesConInclude = _unidad.Ordenes.ObtenerConRelacionesOptimizado();
//         cronometroConInclude.Stop();
//
//         Console.WriteLine($"Sin Include: {cronometroSinInclude.ElapsedMilliseconds} ms | Órdenes: {ordenesSinInclude.Count}");
//         Console.WriteLine($"Con Include y AsSplitQuery: {cronometroConInclude.ElapsedMilliseconds} ms | Órdenes: {ordenesConInclude.Count}");
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.12 - BLOCK 11
// SECTION: Paso 3: Medir el tiempo antes y después:
// SOURCE TARGET: Paso 3: Medir el tiempo antes y después:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void CompararCompleta()
// {
//     Console.WriteLine("\n--- Completa sin optimizar vs optimizada ---");
//
//     var cronometroSin = Stopwatch.StartNew();
//     var ordenesSin = _unidad.Ordenes.ObtenerSinOptimizarCompleta();
//     cronometroSin.Stop();
//
//     var cronometroCon = Stopwatch.StartNew();
//     var ordenesCon = _unidad.Ordenes.ObtenerOptimizadoCompleta(1, 10);
//     cronometroCon.Stop();
//
//     Console.WriteLine($"Sin optimizar: {cronometroSin.ElapsedMilliseconds} ms | Órdenes: {ordenesSin.Count}");
//     Console.WriteLine($"Optimizado: {cronometroCon.ElapsedMilliseconds} ms | Órdenes: {ordenesCon.Count}");
// }
// ========================================================================

// CANONICAL INLINE M04 4.12 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// CompararCompleta();
// ========================================================================
