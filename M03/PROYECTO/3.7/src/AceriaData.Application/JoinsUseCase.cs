using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class JoinsUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public JoinsUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== JOINS Y NAVEGACIÓN ===");
        var inner = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();
        var left = _unidad.Ordenes.ObtenerLeftJoinOrdenesPlanchas();
        var detalle = _unidad.Ordenes.ObtenerOrdenesConDetalleJoin();
        var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleaciones();
        if (inner.Count != 5 || left.Count != 6 || detalle.Count != 5 || aleaciones.Count != 5) throw new InvalidOperationException("Joins inesperados.");
        if (!left.Any(x => x.NumeroOrden == "OF-2024-0004" && x.Peso is null)) throw new InvalidOperationException("LEFT JOIN no conservó la orden sin planchas.");
        Console.WriteLine($"INNER filas: {inner.Count} | LEFT filas: {left.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlJoinExplicito());

        /*
        // ERROR CONTROLADO M03 3.7 - INNER JOIN PIERDE ORDEN SIN PLANCHAS
        // Demuestra el Paso 9: un INNER JOIN sólo conserva coincidencias.
        var innerError = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();

        if (innerError.Any(x => x.NumeroOrden == "OF-2024-0004"))
            throw new InvalidOperationException("Error controlado 3.7: el INNER JOIN conservó una orden sin planchas.");

        Console.WriteLine("Error controlado 3.7 OK | INNER JOIN no contiene OF-2024-0004");
        */

        /*
        // RETO M03 3.7 - LEFT JOIN CONSERVA ORDEN SIN PLANCHAS
        // Demuestra el Paso 10 y el laboratorio adicional comparando INNER y LEFT JOIN.
        var innerReto = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();
        var leftReto = _unidad.Ordenes.ObtenerLeftJoinOrdenesPlanchas();

        if (innerReto.Count != 5 || leftReto.Count != 6)
            throw new InvalidOperationException("Reto 3.7: cardinalidad inesperada al comparar INNER y LEFT JOIN.");

        if (innerReto.Any(x => x.NumeroOrden == "OF-2024-0004"))
            throw new InvalidOperationException("Reto 3.7: INNER JOIN no debe contener OF-2024-0004.");

        var ordenSinPlanchas = leftReto.SingleOrDefault(x =>
            x.NumeroOrden == "OF-2024-0004" && x.Peso is null && x.Espesor is null);

        if (ordenSinPlanchas is null)
            throw new InvalidOperationException("Reto 3.7: LEFT JOIN no conservó OF-2024-0004 con datos relacionados nulos.");

        var sqlLeft = _unidad.Ordenes.ObtenerSqlLeftJoinReto();

        Console.WriteLine($"Reto 3.7 OK | INNER: {innerReto.Count} | LEFT: {leftReto.Count} | Conservada: {ordenSinPlanchas.NumeroOrden}");
        Console.WriteLine("RETO_LEFT_JOIN_SQL_INICIO");
        Console.WriteLine(sqlLeft);
        Console.WriteLine("RETO_LEFT_JOIN_SQL_FIN");
        */
    }
}

// ============================================================================
// FRAGMENTO PDF M03 3.7 - PASO 4
// COPIA PEDAGÓGICA EXACTA DEL BLOQUE PUBLICADO EN M03_PRACTICA.
// El E2E sustituye temporalmente el archivo activo por esta copia y la compila.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class JoinsUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public JoinsUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== JOINS Y NAVEGACIÓN ===");
//         var inner = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();
//         var left = _unidad.Ordenes.ObtenerLeftJoinOrdenesPlanchas();
//         var detalle = _unidad.Ordenes.ObtenerOrdenesConDetalleJoin();
//         var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleaciones();
//         if (inner.Count != 5 || left.Count != 6 || detalle.Count != 5 || aleaciones.Count != 5) throw new InvalidOperationException("Joins inesperados.");
//         if (!left.Any(x => x.NumeroOrden == "OF-2024-0004" && x.Peso is null)) throw new InvalidOperationException("LEFT JOIN no conservó la orden sin planchas.");
//         Console.WriteLine($"INNER filas: {inner.Count} | LEFT filas: {left.Count}");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlJoinExplicito());
//     }
// }
// ============================================================================
