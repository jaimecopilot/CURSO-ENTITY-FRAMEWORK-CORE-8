using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class OverFetchingUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public OverFetchingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.6 OVER-FETCHING ===");
        var completas = _unidad.Ordenes.ObtenerPendientesEntidadCompletaM4();
        var proyectadas = _unidad.Ordenes.ObtenerPendientesProyectadasM4();
        var sqlCompleto = _unidad.Ordenes.ObtenerSqlPendientesEntidadCompletaM4();
        var sqlProyectado = _unidad.Ordenes.ObtenerSqlPendientesProyectadasM4();

        if (completas.Count != proyectadas.Count)
            throw new InvalidOperationException("4.6: la proyeccion cambio la cardinalidad.");
        if (!sqlCompleto.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.6: el SQL completo no evidencia columnas extra.");
        if (sqlProyectado.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.6: la proyeccion sigue recuperando Observaciones.");

        Console.WriteLine($"Filas equivalentes: {completas.Count}");
        Console.WriteLine("--- SQL entidad completa ---");
        Console.WriteLine(sqlCompleto);
        Console.WriteLine("--- SQL proyeccion ---");
        Console.WriteLine(sqlProyectado);
    }
}
