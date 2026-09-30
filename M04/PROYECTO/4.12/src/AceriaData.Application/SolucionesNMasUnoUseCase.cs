using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SolucionesNMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");
        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
            throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");

        Console.WriteLine($"Include: {include.ConsultasSql} consulta.");
        Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");
    }
}
