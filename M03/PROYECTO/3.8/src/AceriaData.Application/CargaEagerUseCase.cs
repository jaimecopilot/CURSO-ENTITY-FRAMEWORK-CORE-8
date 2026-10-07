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

        /*
        // ERROR CONTROLADO M03 3.8 - FILTERED INCLUDE CON TRACKING Y FIX-UP
        // Demuestra el Paso 9: entidades ya rastreadas pueden reincorporarse a la navegación filtrada.
        var conTracking = _unidad.Ordenes.ObtenerCantidadPlanchasFilteredIncludeConTrackingReto();
        var segura = filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count;

        if (conTracking != 2 || segura != 1)
            throw new InvalidOperationException("Error controlado 3.8: no se reprodujo la diferencia tracking/fix-up frente a AsNoTracking.");

        Console.WriteLine($"Error controlado 3.8 OK | Tracking: {conTracking} | AsNoTracking: {segura}");
        */

        /*
        // RETO M03 3.8 - SINGLEQUERY VS SPLITQUERY MISMO GRAFO
        // Demuestra el Paso 10 y el laboratorio adicional con las mismas relaciones.
        var autoReto = auto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count;
        var sinAutoReto = sinAuto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count;
        var filtradaReto = filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count;
        var splitPlanchas = split.Sum(o => o.Planchas.Count);
        var splitAleaciones = split.Sum(o => o.OrdenesAleaciones.Count);

        if (autoReto != 2 || sinAutoReto != 0 || filtradaReto != 1 ||
            splitPlanchas != 5 || splitAleaciones != 4)
            throw new InvalidOperationException("Reto 3.8: el grafo cargado no coincide con el dataset esperado.");

        var sqlSingle = _unidad.Ordenes.ObtenerSqlCargaCompletaSingleQueryReto();
        var sqlSplit = _unidad.Ordenes.ObtenerSqlCargaCompletaSplitQueryReto();

        Console.WriteLine($"Reto 3.8 OK | Auto: {autoReto} | SinAuto: {sinAutoReto} | Filtrada: {filtradaReto} | Split Planchas: {splitPlanchas} | Split Aleaciones: {splitAleaciones}");
        Console.WriteLine("RETO_SINGLE_SQL_INICIO");
        Console.WriteLine(sqlSingle);
        Console.WriteLine("RETO_SINGLE_SQL_FIN");
        Console.WriteLine("RETO_SPLIT_SQL_INICIO");
        Console.WriteLine(sqlSplit);
        Console.WriteLine("RETO_SPLIT_SQL_FIN");
        */
    }
}

// ============================================================================
// EJEMPLO DEL PASO 4
// COPIA COMENTADA DEL BLOQUE DE LA PRÁCTICA PARA QUE PUEDAS PROBARLO.
// Para utilizarla, comenta temporalmente la implementación activa equivalente y descomenta esta copia en una rama o copia de trabajo.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class CargaEagerUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public CargaEagerUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== EAGER LOADING ===");
//         var planchas = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();
//         var detalle = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleInclude();
//         var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleacionesInclude();
//         var filtradas = _unidad.Ordenes.ObtenerOrdenesConPlanchasPesadasInclude();
//         var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSplitQuery();
//         var auto = _unidad.Ordenes.ObtenerOrdenesAutoInclude();
//         var sinAuto = _unidad.Ordenes.ObtenerOrdenesIgnorandoAutoInclude();
// 
//         if (planchas.Count != 5 || detalle.Count != 5 || aleaciones.Count != 5 || split.Count != 5)
//             throw new InvalidOperationException("Carga Eager inesperada.");
//         if (filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 1)
//             throw new InvalidOperationException("Filtered Include inesperado.");
//         if (auto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 2)
//             throw new InvalidOperationException("AutoInclude no cargó Planchas.");
//         if (sinAuto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 0)
//             throw new InvalidOperationException("IgnoreAutoIncludes no suprimió la carga automática.");
//         if (split.Sum(o => o.Planchas.Count) != 5 || split.Sum(o => o.OrdenesAleaciones.Count) != 4)
//             throw new InvalidOperationException("SplitQuery no materializó las dos colecciones esperadas.");
// 
//         Console.WriteLine($"Include: {planchas.Count} órdenes | SplitQuery: {split.Count} | AutoInclude: {auto.Count} | IgnoreAutoIncludes: {sinAuto.Count}");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlInclude());
//     }
// }
// ============================================================================
