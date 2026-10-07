using AceriaData.Domain.Entities;

namespace AceriaData.Application.Interfaces;

public interface IOrdenRepositorio
{
    OrdenFabricacion? ObtenerPorId(int id);
    OrdenFabricacion? ObtenerPorNumero(string numeroOrden);
    List<OrdenFabricacion> ObtenerTodas();
    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
}

public interface IUnidadDeTrabajo : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    int Guardar();
}

// ============================================================================
// EJEMPLO DEL PASO 3
// Puertos IOrdenRepositorio e IUnidadDeTrabajo.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 3 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// 
// namespace AceriaData.Application.Interfaces;
// 
// public interface IOrdenRepositorio
// {
//     OrdenFabricacion? ObtenerPorId(int id);
//     OrdenFabricacion? ObtenerPorNumero(string numeroOrden);
//     List<OrdenFabricacion> ObtenerTodas();
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// 
// public interface IUnidadDeTrabajo : IDisposable
// {
//     IOrdenRepositorio Ordenes { get; }
//     int Guardar();
// }
// ============================================================================
