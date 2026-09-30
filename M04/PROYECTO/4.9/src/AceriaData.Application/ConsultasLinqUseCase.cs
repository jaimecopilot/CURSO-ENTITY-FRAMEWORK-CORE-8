using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConsultasLinqUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        var todas = _unidad.Ordenes.ObtenerTodas();
        var norte = todas.Where(o => o.Cliente == "Constructora del Norte").ToList();
        Console.WriteLine($"Fundamento histórico LINQ: {norte.Count} órdenes del Norte. En 3.12 la composición SQL queda encapsulada en Infrastructure.");
    }
}
