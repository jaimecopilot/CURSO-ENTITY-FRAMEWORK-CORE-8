using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CargaLazyUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CargaLazyUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== LAZY LOADING (DEMOSTRACIÓN) ===");
        var ordenes = _unidad.Ordenes.ObtenerTodasSinInclude();
        var totalPlanchas = 0;
        foreach (var orden in ordenes)
            totalPlanchas += orden.Planchas.Count;
        if (ordenes.Count != 5 || totalPlanchas != 5) throw new InvalidOperationException("Lazy Loading no cargó las relaciones.");
        Console.WriteLine($"Órdenes: {ordenes.Count} | Planchas accedidas bajo demanda: {totalPlanchas}");
        Console.WriteLine("Advertencia docente: el acceso dentro de un bucle puede producir N+1 consultas.");
    }
}
