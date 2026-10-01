using AceriaData.Infrastructure;

namespace AceriaData.ConsoleApp;

public sealed class BuenasPracticasAntiPatronesM5Runner
{
    private readonly BuenasPracticasAntiPatronesM5Diagnostico _diagnostico;

    public BuenasPracticasAntiPatronesM5Runner(BuenasPracticasAntiPatronesM5Diagnostico diagnostico) =>
        _diagnostico = diagnostico;

    public void Ejecutar()
    {
        var resultado = _diagnostico.Ejecutar();
        var n = resultado.NMasUno;
        var o = resultado.OverFetching;
        var t = resultado.Traduccion;

        if (!n.ResultadosEquivalentes)
            throw new InvalidOperationException("N+1, Include y proyección no devolvieron resultados equivalentes.");
        if (n.ConsultasNMasUno <= n.ConsultasInclude || n.ConsultasNMasUno <= n.ConsultasProyeccion)
            throw new InvalidOperationException("El escenario N+1 no produjo más roundtrips que las alternativas.");
        if (n.TrackingInclude != 0 || n.TrackingProyeccion != 0)
            throw new InvalidOperationException("Las alternativas de lectura no deberían dejar entidades rastreadas.");

        if (!o.ResultadosEquivalentes)
            throw new InvalidOperationException("Entidad completa y proyección no son funcionalmente equivalentes.");
        if (o.ColumnasEntidadCompleta <= o.ColumnasProyeccion)
            throw new InvalidOperationException("La proyección no redujo las columnas materializadas.");
        if (o.TrackingEntidadCompleta <= 0 || o.TrackingProyeccion != 0)
            throw new InvalidOperationException("No se observó la diferencia de tracking esperada.");
        if (!o.SqlEntidadCompletaIncluyeRowVersion || !o.SqlProyeccionExcluyeRowVersion)
            throw new InvalidOperationException("El SQL no demuestra la reducción de columnas de la proyección.");

        if (!t.MetodoNoTraducibleFalla || t.ComandosEmitidosAntesDelFallo != 0)
            throw new InvalidOperationException("El método no traducible no falló antes de emitir SQL.");
        if (!t.EvaluacionClienteExplicitaFunciona || t.ComandosEvaluacionCliente != 1)
            throw new InvalidOperationException("No se demostró la evaluación cliente explícita.");

        Console.WriteLine($"5.12 N+1 ROUNDTRIPS: {n.ConsultasNMasUno} -> INCLUDE {n.ConsultasInclude} -> PROJECTION {n.ConsultasProyeccion}");
        Console.WriteLine($"5.12 N+1 RESULTADOS EQUIVALENTES: {n.ResultadosEquivalentes}");
        Console.WriteLine($"5.12 N+1 TRACKING INCLUDE: {n.TrackingInclude}");
        Console.WriteLine($"5.12 N+1 TRACKING PROJECTION: {n.TrackingProyeccion}");

        Console.WriteLine($"5.12 OVERFETCH COLUMNAS: {o.ColumnasEntidadCompleta} -> {o.ColumnasProyeccion}");
        Console.WriteLine($"5.12 OVERFETCH TRACKING: {o.TrackingEntidadCompleta} -> {o.TrackingProyeccion}");
        Console.WriteLine($"5.12 OVERFETCH RESULTADOS EQUIVALENTES: {o.ResultadosEquivalentes}");
        Console.WriteLine($"5.12 PROJECTION EXCLUDES ROWVERSION: {o.SqlProyeccionExcluyeRowVersion}");

        Console.WriteLine($"5.12 METODO WHERE FALLA TRADUCCION: {t.MetodoNoTraducibleFalla}");
        Console.WriteLine($"5.12 SQL ANTES DEL FALLO: {t.ComandosEmitidosAntesDelFallo}");
        Console.WriteLine($"5.12 EVALUACION CLIENTE EXPLICITA: {t.EvaluacionClienteExplicitaFunciona}");
        Console.WriteLine($"5.12 MATRIZ FILAS: {resultado.Matriz.Count}");

        foreach (var fila in resultado.Matriz)
        {
            Console.WriteLine($"MATRIZ | {fila.Caso} | {fila.Consecuencia} | {fila.Refactor} | {fila.TradeOff}");
        }
    }
}
