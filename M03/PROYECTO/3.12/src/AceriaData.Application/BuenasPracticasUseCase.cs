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

        /*
        // ERROR CONTROLADO M03 3.12 - IQUERYABLE EXPUESTO EN APPLICATION
        // Demuestra el Paso 9: compila y funciona, pero permite que Application componga
        // directamente la consulta del proveedor y rompe la frontera buscada al cerrar M3.
        var antiPatron = _unidad.Ordenes.ConsultaAntiPatron()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ToList();

        if (antiPatron.Count != 3)
            throw new InvalidOperationException("Error controlado 3.12: IQueryable público devolvió un resultado inesperado.");

        Console.WriteLine($"Error controlado 3.12 OK | IQueryable compuesto en Application: {antiPatron.Count}");
        */

        /*
        // RETO M03 3.12 - FRONTERA LIMPIA Y BUENAS PRACTICAS
        // La validación ejecutable usa únicamente métodos específicos del puerto.
        // El E2E comprobará además que el contrato activo no contiene IQueryable
        // aunque BuscarOrdenes siga componiendo IQueryable dentro de Infrastructure.
        var retoResumenes = _unidad.Ordenes.ObtenerResumenesPendientesOptimizado();
        var retoExiste = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();
        var retoSplit = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();
        var retoInexistente = _unidad.Ordenes.ObtenerPorNumeroOptimizado("OF-2024-9999");

        if (retoResumenes.Count != 3 || !retoExiste || retoSplit.Count != 5 || retoInexistente is not null)
            throw new InvalidOperationException("Reto 3.12: la frontera final no conserva el comportamiento esperado.");

        Console.WriteLine($"Reto 3.12 OK | Pendientes: {retoResumenes.Count} | Any: {retoExiste} | Split: {retoSplit.Count} | Inexistente: null");
        */
    }
}

// ============================================================================
// FRAGMENTO PDF M03 3.12 - PASO 4
// COPIA PEDAGÓGICA EXACTA DEL BLOQUE PUBLICADO EN M03_PRACTICA.
// El E2E sustituye temporalmente el archivo activo por esta copia y la compila.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class BuenasPracticasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public BuenasPracticasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== BUENAS PRÁCTICAS EN EL ACCESO A DATOS ===");
//         var resumenes = _unidad.Ordenes.ObtenerResumenesPendientesOptimizado();
//         var existe = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();
//         var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();
//         var inexistente = _unidad.Ordenes.ObtenerPorNumeroOptimizado("OF-2024-9999");
//         if (resumenes.Count != 3 || !existe || split.Count != 5 || inexistente is not null) throw new InvalidOperationException("Buenas prácticas: validación E2E fallida.");
//         Console.WriteLine($"Pendientes proyectadas: {resumenes.Count} | Any: {existe} | SplitQuery: {split.Count}");
//         Console.WriteLine("El contrato final ya no expone IQueryable fuera de Infrastructure.");
//     }
// }
// ============================================================================
