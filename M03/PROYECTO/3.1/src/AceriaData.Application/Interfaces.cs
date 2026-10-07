using AceriaData.Domain.Entities;

namespace AceriaData.Application.Interfaces;

public interface IOrdenRepositorio
{
    OrdenFabricacion? ObtenerPorId(int id);
    OrdenFabricacion? ObtenerPorNumero(string numeroOrden);
    List<OrdenFabricacion> ObtenerTodas();
    IQueryable<OrdenFabricacion> Consulta();
    string ObtenerSqlFundamentos();

    /*
    // RETO M03 3.1 - PUERTO SQL OPCIONAL
    // Se activa coordinadamente con el repositorio y el caso de uso del reto.
    string ObtenerSqlRetoFundamentos(string cliente, string? estado);
    */
    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
}

public interface IUnidadDeTrabajo : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    int Guardar();
}
