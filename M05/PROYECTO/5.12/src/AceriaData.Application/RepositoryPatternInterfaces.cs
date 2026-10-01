using AceriaData.Domain.Entities;

namespace AceriaData.Application.Interfaces;

public interface IRepositorio<T> where T : class
{
    T? ObtenerPorId(int id);
    List<T> ObtenerTodas();
    void Agregar(T entidad);
    void Eliminar(T entidad);
}

public interface IDetalleOrdenRepositorio : IRepositorio<DetalleOrden>
{
    DetalleOrden? ObtenerPorOrden(int ordenId);
    List<DetalleOrden> ObtenerConTemperaturaMayorA(double temperatura);
}
