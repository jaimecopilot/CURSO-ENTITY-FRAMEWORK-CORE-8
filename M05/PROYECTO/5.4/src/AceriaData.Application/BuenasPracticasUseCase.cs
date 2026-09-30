using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class BuenasPracticasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public BuenasPracticasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== BUENAS PRÁCTICAS EN EL ACCESO A DATOS ===");
        var resumenes = _unidad.Ordenes.ObtenerResumenesPendientesOptimizado();
        var existe = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();
        var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();
        var inexistente = _unidad.Ordenes.ObtenerPorNumeroOptimizado("OF-2024-9999");
        if (resumenes.Count != 3 || !existe || split.Count != 5 || inexistente is not null) throw new InvalidOperationException("Buenas prácticas: validación E2E fallida.");
        Console.WriteLine($"Pendientes proyectadas: {resumenes.Count} | Any: {existe} | SplitQuery: {split.Count}");
        Console.WriteLine("El contrato final ya no expone IQueryable fuera de Infrastructure.");
    }
}
