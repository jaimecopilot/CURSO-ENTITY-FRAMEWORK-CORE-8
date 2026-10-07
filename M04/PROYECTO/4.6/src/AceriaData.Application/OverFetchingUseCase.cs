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

// CANONICAL INLINE M04 4.6 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de over-fetching
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/OverFetchingUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class OverFetchingUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public OverFetchingUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== OVER-FETCHING ===");
//
//         DemostrarEntidadesCompletas();
//         DemostrarProyeccion();
//         DemostrarPaginacion();
//         DemostrarFiltroYProyeccion();
//         CompararRendimiento();
//     }
//
//     private void DemostrarEntidadesCompletas()
//     {
//         Console.WriteLine("\n--- Entidades completas (over-fetching) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesCompletas();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//         }
//     }
//
//     private void DemostrarProyeccion()
//     {
//         Console.WriteLine("\n--- Proyección (sin over-fetching de columnas) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenesProyectados();
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado}");
//         }
//     }
//
//     private void DemostrarPaginacion()
//     {
//         Console.WriteLine("\n--- Paginación (sin over-fetching de filas) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenesPaginados(1, 3);
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado}");
//         }
//     }
//
//     private void DemostrarFiltroYProyeccion()
//     {
//         Console.WriteLine("\n--- Filtro y proyección ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenesFiltradosYProyectados("Pendiente");
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes pendientes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado}");
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroCompletas = Stopwatch.StartNew();
//         var ordenesCompletas = _unidad.Ordenes.ObtenerOrdenesCompletas();
//         cronometroCompletas.Stop();
//
//         var cronometroProyectadas = Stopwatch.StartNew();
//         var resumenesProyectados = _unidad.Ordenes.ObtenerResumenesProyectados();
//         cronometroProyectadas.Stop();
//
//         Console.WriteLine($"Entidades completas: {cronometroCompletas.ElapsedMilliseconds} ms | Registros: {ordenesCompletas.Count}");
//         Console.WriteLine($"Proyección: {cronometroProyectadas.ElapsedMilliseconds} ms | Registros: {resumenesProyectados.Count}");
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.6 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarSqlComparativo()
// {
//     Console.WriteLine("\n--- SQL de entidades completas ---");
//     var sqlCompletas = _unidad.Ordenes.ObtenerSqlEntidadesCompletas();
//     Console.WriteLine(sqlCompletas);
//
//     Console.WriteLine("\n--- SQL de proyección ---");
//     var sqlProyeccion = _unidad.Ordenes.ObtenerSqlProyeccionResumen();
//     Console.WriteLine(sqlProyeccion);
// }
// ========================================================================

// CANONICAL INLINE M04 4.6 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// DemostrarSqlComparativo();
// ========================================================================
