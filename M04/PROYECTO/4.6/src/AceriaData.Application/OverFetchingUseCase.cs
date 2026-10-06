using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class OverFetchingUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public OverFetchingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.6 OVER-FETCHING ===");
        var completas = _unidad.Ordenes.ObtenerPendientesEntidadCompletaM4();
        var proyectadas = _unidad.Ordenes.ObtenerPendientesProyectadasM4();
        var sqlCompleto = _unidad.Ordenes.ObtenerSqlPendientesEntidadCompletaM4();
        var sqlProyectado = _unidad.Ordenes.ObtenerSqlPendientesProyectadasM4();

        if (completas.Count != proyectadas.Count)
            throw new InvalidOperationException("4.6: la proyeccion cambio la cardinalidad.");
        if (!sqlCompleto.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.6: el SQL completo no evidencia columnas extra.");
        if (sqlProyectado.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.6: la proyeccion sigue recuperando Observaciones.");

        Console.WriteLine($"Filas equivalentes: {completas.Count}");
        Console.WriteLine("--- SQL entidad completa ---");
        Console.WriteLine(sqlCompleto);
        Console.WriteLine("--- SQL proyeccion ---");
        Console.WriteLine(sqlProyectado);
    }

    /*
    // ERROR CONTROLADO M04 4.6 - PROYECCION DEMASIADO TARDE
    public void EjecutarErrorMaterializacionTemprana()
    {
        var resultado = _unidad.Ordenes.MaterializarAntesDeProyectarM4();

        if (resultado.Filas == 0)
            throw new InvalidOperationException("4.6 error controlado: el escenario no devolvió filas.");
        if (!resultado.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase) ||
            !resultado.Sql.Contains("FechaEntrega", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.6 error controlado: no se observa el SELECT completo previo a la proyección en memoria.");

        Console.WriteLine(
            $"Error controlado 4.6 OK | Filas={resultado.Filas} | ToList antes de Select conserva SELECT completo");
    }
    */

    /*
    // RETO M04 4.6 - COLUMNAS ELIMINADAS POR LA PROYECCION
    public void EjecutarRetoColumnas()
    {
        var sqlCompleto = _unidad.Ordenes.ObtenerSqlPendientesEntidadCompletaM4();
        var sqlProyectado = _unidad.Ordenes.ObtenerSqlPendientesProyectadasM4();

        var eliminadas = new[] { "Observaciones", "FechaEntrega" }
            .Where(columna =>
                sqlCompleto.Contains(columna, StringComparison.OrdinalIgnoreCase) &&
                !sqlProyectado.Contains(columna, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (eliminadas.Length != 2)
            throw new InvalidOperationException(
                "Reto 4.6: no desaparecieron del SELECT las dos columnas esperadas.");

        Console.WriteLine(
            $"Reto 4.6 OK | columnas eliminadas del SELECT: {string.Join(",", eliminadas)}");
        Console.WriteLine(
            "Menos columnas transferidas implica menos datos que transportar y materializar para la misma cardinalidad.");
    }
    */

}


// FRAGMENTO PDF M04 4.6 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class OverFetchingUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public OverFetchingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.6 OVER-FETCHING ===");
//         var completas = _unidad.Ordenes.ObtenerPendientesEntidadCompletaM4();
//         var proyectadas = _unidad.Ordenes.ObtenerPendientesProyectadasM4();
//         var sqlCompleto = _unidad.Ordenes.ObtenerSqlPendientesEntidadCompletaM4();
//         var sqlProyectado = _unidad.Ordenes.ObtenerSqlPendientesProyectadasM4();
//
//         if (completas.Count != proyectadas.Count)
//             throw new InvalidOperationException("4.6: la proyeccion cambio la cardinalidad.");
//         if (!sqlCompleto.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.6: el SQL completo no evidencia columnas extra.");
//         if (sqlProyectado.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.6: la proyeccion sigue recuperando Observaciones.");
//
//         Console.WriteLine($"Filas equivalentes: {completas.Count}");
//         Console.WriteLine("--- SQL entidad completa ---");
//         Console.WriteLine(sqlCompleto);
//         Console.WriteLine("--- SQL proyeccion ---");
//         Console.WriteLine(sqlProyectado);
//     }
// }
// ========================================================================
