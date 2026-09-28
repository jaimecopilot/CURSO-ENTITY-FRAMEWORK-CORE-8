using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class AgrupacionesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public AgrupacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== AGRUPACIONES CON PROYECCIÓN ===");
        var porCliente = _unidad.Ordenes.ObtenerResumenPorClienteConOrdenes();
        var porClienteEstado = _unidad.Ordenes.ObtenerResumenPorClienteYEstado();
        var having = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();
        var mensual = _unidad.Ordenes.ObtenerResumenMensualConOrdenes();
        var norte = porCliente.Single(x => x.Cliente == "Constructora del Norte");
        if (norte.TotalOrdenes != 3 || norte.Ordenes.Count != 3 || porClienteEstado.Count != 4 || having.Count != 1 || mensual.Count != 5) throw new InvalidOperationException("Agrupaciones inesperadas.");
        Console.WriteLine($"Norte: {norte.TotalOrdenes} órdenes | HAVING: {having.Count} grupo");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlAgrupacionClienteEstado());
    }
}
