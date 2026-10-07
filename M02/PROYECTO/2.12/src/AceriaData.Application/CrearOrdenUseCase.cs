using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;

namespace AceriaData.Application.UseCases;

public sealed class CrearOrdenUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CrearOrdenUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public int Ejecutar(string numeroOrden, string cliente)
    {
        _unidad.Ordenes.Agregar(new OrdenFabricacion
        {
            NumeroOrden = numeroOrden,
            Cliente = cliente,
            FechaCreacion = DateTime.UtcNow,
            Estado = "Pendiente"
        });
        return _unidad.Guardar();
    }
}

// ============================================================================
// FRAGMENTO PDF M02 2.12 - PASO 4
// Caso de uso CrearOrdenUseCase.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 4 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class CrearOrdenUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public CrearOrdenUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public int Ejecutar(string numeroOrden, string cliente)
//     {
//         _unidad.Ordenes.Agregar(new OrdenFabricacion
//         {
//             NumeroOrden = numeroOrden,
//             Cliente = cliente,
//             FechaCreacion = DateTime.UtcNow,
//             Estado = "Pendiente"
//         });
//         return _unidad.Guardar();
//     }
// }
// ============================================================================
