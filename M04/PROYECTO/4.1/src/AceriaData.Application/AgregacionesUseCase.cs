using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class AgregacionesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public AgregacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== AGREGACIONES ===");
        var total = _unidad.Ordenes.ContarOrdenes();
        var pendientes = _unidad.Ordenes.ContarOrdenesPorEstado("Pendiente");
        var existe = _unidad.Ordenes.ExisteAlgunaOrden();
        var todasConEstado = _unidad.Ordenes.TodasLasOrdenesTienenEstado();
        var peso = _unidad.Ordenes.ObtenerPesoTotalDePlanchas();
        var promedio = _unidad.Ordenes.ObtenerPesoPromedioDePlanchas();
        var minimo = _unidad.Ordenes.ObtenerPesoMinimoDePlanchas();
        var maximo = _unidad.Ordenes.ObtenerPesoMaximoDePlanchas();
        var porCliente = _unidad.Ordenes.ObtenerResumenPorCliente();
        var porEstado = _unidad.Ordenes.ObtenerResumenPorEstado();
        var mensual = _unidad.Ordenes.ObtenerResumenMensual();
        if (total != 5 || pendientes != 3 || !existe || !todasConEstado || peso != 1426.9m || promedio != 285.38m || minimo != 125.6m || maximo != 450.0m || porCliente.Count != 3 || porEstado.Count != 3 || mensual.Count != 5) throw new InvalidOperationException("Agregaciones inesperadas.");
        Console.WriteLine($"Órdenes: {total} | Pendientes: {pendientes} | Peso: {peso:N1} kg | Promedio: {promedio:N2} kg | Min: {minimo:N1} kg | Max: {maximo:N1} kg");
    }
}
