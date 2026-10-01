using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;

namespace AceriaData.Application.UseCases;

public sealed class RepositorioUnidadTrabajoM5UseCase
{
    private readonly IUnidadDeTrabajo _unidad;

    public RepositorioUnidadTrabajoM5UseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public Task EjecutarAsync()
    {
        Console.WriteLine("=== 5.9 REPOSITORIO Y UNIDAD DE TRABAJO ===");

        var antes = _unidad.Ordenes.ContarOrdenes();

        var orden = new OrdenFabricacion
        {
            NumeroOrden = ("OF-M5-59-" + Guid.NewGuid().ToString("N"))[..20],
            Cliente = "Cliente Repository",
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow
        };

        var detalle = new DetalleOrden
        {
            Orden = orden,
            ComposicionQuimica = "C: 0.45%, Mn: 0.75%",
            TemperaturaColada = 1550.5,
            EstadoDetalle = "Pendiente"
        };

        _unidad.Ordenes.Agregar(orden);
        _unidad.Detalles.Agregar(detalle);
        var filas = _unidad.Guardar();

        var recuperada = _unidad.Ordenes.ObtenerPorNumero(orden.NumeroOrden);
        var detalleRecuperado = recuperada is null ? null : _unidad.Detalles.ObtenerPorOrden(recuperada.Id);

        Console.WriteLine($"Órdenes antes: {antes}");
        Console.WriteLine($"Filas afectadas por UoW: {filas}");
        Console.WriteLine($"Orden creada y recuperada: {recuperada is not null}");
        Console.WriteLine($"Detalle creado y recuperado: {detalleRecuperado is not null}");
        Console.WriteLine("Repository/UoW es una decisión arquitectónica de AceriaData; DbContext ya incorpora capacidades de repositorio y unidad de trabajo.");

        return Task.CompletedTask;
    }
}
