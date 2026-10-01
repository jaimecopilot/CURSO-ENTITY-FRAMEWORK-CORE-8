using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SplitQueriesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");
        var single = _unidad.Ordenes.MedirSingleQueryM4();
        var split = _unidad.Ordenes.MedirSplitQueryM4();

        if (single.ConsultasSql != 1)
            throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");
        if (single.Ordenes != split.Ordenes ||
            single.Planchas != split.Planchas ||
            single.RelacionesAleacion != split.RelacionesAleacion)
            throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");

        Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");
        Console.WriteLine("--- ToQueryString SingleQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());
        Console.WriteLine("--- ToQueryString SplitQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());
    }
}
