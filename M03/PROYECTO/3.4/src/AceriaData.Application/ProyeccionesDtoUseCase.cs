using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ProyeccionesDtoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ProyeccionesDtoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== PROYECCIONES A DTOs ===");
        var conPlanchas = _unidad.Ordenes.ObtenerOrdenesConPlanchas();
        var conDetalle = _unidad.Ordenes.ObtenerOrdenesConDetalle();
        var completas = _unidad.Ordenes.ObtenerOrdenesCompletas();
        var primera = completas.Single(o => o.NumeroOrden == "OF-2024-0001");
        if (conPlanchas.Count != 5 || conDetalle.Count != 5 || completas.Count != 5 || primera.Planchas.Count != 2 || primera.Detalle is null) throw new InvalidOperationException("Proyecciones DTO inesperadas.");
        Console.WriteLine($"OF-2024-0001 -> planchas: {primera.Planchas.Count}, detalle: {primera.Detalle.ComposicionQuimica}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccionNavegacion());

        /*
        // ERROR CONTROLADO M03 3.4 - RELACION OPCIONAL SIN COMPROBAR NULL
        // Demuestra el Paso 9 usando una orden cuyo Detalle opcional no existe.
        var sinDetalleError = _unidad.Ordenes.ObtenerOrdenesCompletas()
            .Single(o => o.NumeroOrden == "OF-2024-0005");

        Console.WriteLine(sinDetalleError.Detalle!.ComposicionQuimica);
        */

        /*
        // RETO M03 3.4 - DTO CON PLANCHAS Y DETALLE OPCIONAL
        // Demuestra el Paso 10 y el laboratorio adicional sin exponer entidades.
        var completasReto = _unidad.Ordenes.ObtenerOrdenesCompletas();
        var conDetalleReto = completasReto.Single(o => o.NumeroOrden == "OF-2024-0001");
        var sinDetalleReto = completasReto.Single(o => o.NumeroOrden == "OF-2024-0005");

        if (conDetalleReto.Planchas.Count != 2 || conDetalleReto.Detalle is null)
            throw new InvalidOperationException("Reto 3.4: OF-2024-0001 debe conservar dos planchas y detalle.");

        if (sinDetalleReto.Detalle is not null)
            throw new InvalidOperationException("Reto 3.4: OF-2024-0005 debe conservar Detalle == null.");

        Console.WriteLine($"Reto 3.4 OK | OF-2024-0001 planchas: {conDetalleReto.Planchas.Count} | OF-2024-0005 detalle: null");
        */
    }
}

// ============================================================================
// FRAGMENTO PDF M03 3.4 - PASO 4
// COPIA PEDAGÓGICA EXACTA DEL BLOQUE PUBLICADO EN M03_PRACTICA.
// El E2E sustituye temporalmente el archivo activo por esta copia y la compila.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class ProyeccionesDtoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public ProyeccionesDtoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PROYECCIONES A DTOs ===");
//         var conPlanchas = _unidad.Ordenes.ObtenerOrdenesConPlanchas();
//         var conDetalle = _unidad.Ordenes.ObtenerOrdenesConDetalle();
//         var completas = _unidad.Ordenes.ObtenerOrdenesCompletas();
//         var primera = completas.Single(o => o.NumeroOrden == "OF-2024-0001");
//         if (conPlanchas.Count != 5 || conDetalle.Count != 5 || completas.Count != 5 || primera.Planchas.Count != 2 || primera.Detalle is null) throw new InvalidOperationException("Proyecciones DTO inesperadas.");
//         Console.WriteLine($"OF-2024-0001 -> planchas: {primera.Planchas.Count}, detalle: {primera.Detalle.ComposicionQuimica}");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccionNavegacion());
//     }
// }
// ============================================================================
