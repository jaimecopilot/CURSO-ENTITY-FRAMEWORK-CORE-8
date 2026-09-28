using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CargaEagerUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CargaEagerUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== EAGER LOADING ===");
        var planchas = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();
        var detalle = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleInclude();
        var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleacionesInclude();
        var filtradas = _unidad.Ordenes.ObtenerOrdenesConPlanchasPesadasInclude();
        var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSplitQuery();
        var auto = _unidad.Ordenes.ObtenerOrdenesAutoInclude();
        var sinAuto = _unidad.Ordenes.ObtenerOrdenesIgnorandoAutoInclude();

        if (planchas.Count != 5 || detalle.Count != 5 || aleaciones.Count != 5 || split.Count != 5)
            throw new InvalidOperationException("Carga Eager inesperada.");
        if (filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 1)
            throw new InvalidOperationException("Filtered Include inesperado.");
        if (auto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 2)
            throw new InvalidOperationException("AutoInclude no cargó Planchas.");
        if (sinAuto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 0)
            throw new InvalidOperationException("IgnoreAutoIncludes no suprimió la carga automática.");
        if (split.Sum(o => o.Planchas.Count) != 5 || split.Sum(o => o.OrdenesAleaciones.Count) != 4)
            throw new InvalidOperationException("SplitQuery no materializó las dos colecciones esperadas.");

        Console.WriteLine($"Include: {planchas.Count} órdenes | SplitQuery: {split.Count} | AutoInclude: {auto.Count} | IgnoreAutoIncludes: {sinAuto.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlInclude());
    }
}
