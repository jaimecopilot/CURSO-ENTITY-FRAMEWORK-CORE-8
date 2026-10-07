using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class PaginacionUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public PaginacionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.10 PAGINACION ===");

        var offset = _unidad.Ordenes.ObtenerPaginaOffsetM4(2, 5);
        var primeraKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(DateTime.MinValue, 0, 5);
        var cursor = primeraKeyset.Elementos.Last();
        var segundaKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(
            cursor.FechaCreacion,
            cursor.Id,
            5);

        if (offset.Elementos.Count != 5 ||
            primeraKeyset.Elementos.Count != 5 ||
            segundaKeyset.Elementos.Count != 5)
            throw new InvalidOperationException("4.10: paginacion no devolvio el tamano esperado.");

        if (primeraKeyset.Elementos
            .Select(x => x.Id)
            .Intersect(segundaKeyset.Elementos.Select(x => x.Id))
            .Any())
            throw new InvalidOperationException("4.10: keyset repitio filas entre paginas.");

        Console.WriteLine("--- OFFSET ---");
        Console.WriteLine(offset.Sql);
        Console.WriteLine("--- KEYSET ---");
        Console.WriteLine(segundaKeyset.Sql);
    }

    /*
    // ERROR CONTROLADO M04 4.10 - CURSOR NO UNICO SALTA FILAS
    public void EjecutarErrorCursorNoUnico()
    {
        var universo = _unidad.Ordenes.ObtenerPaginaOffsetM4(1, 20);
        var primera = _unidad.Ordenes.ObtenerPaginaKeysetSoloEstadoM4("", 2);
        var cursor = primera.Elementos.Last().Estado;
        var segunda = _unidad.Ordenes.ObtenerPaginaKeysetSoloEstadoM4(cursor, 2);

        var enProcesoTotales = universo.Elementos.Count(x => x.Estado == "EnProceso");
        var enProcesoVistos = primera.Elementos.Concat(segunda.Elementos)
            .Count(x => x.Estado == "EnProceso");

        if (enProcesoTotales <= enProcesoVistos)
            throw new InvalidOperationException("4.10 error controlado: el dataset no demuestra filas saltadas.");

        Console.WriteLine(
            $"Error controlado 4.10 OK | cursor no unico Estado | EnProceso total={enProcesoTotales} | vistos={enProcesoVistos}");
    }
    */

    /*
    // RETO M04 4.10 - FILTRO DE ESTADO + CURSOR COMPUESTO
    public void EjecutarRetoFiltroEstado()
    {
        var primera = _unidad.Ordenes.ObtenerPaginaKeysetPorEstadoM4(
            "Pendiente", DateTime.MinValue, 0, 5);
        var cursor = primera.Elementos.Last();
        var segunda = _unidad.Ordenes.ObtenerPaginaKeysetPorEstadoM4(
            "Pendiente", cursor.FechaCreacion, cursor.Id, 5);

        if (primera.Elementos.Count != 5 || segunda.Elementos.Count != 5)
            throw new InvalidOperationException("Reto 4.10: tamaño de página inesperado.");

        if (primera.Elementos.Concat(segunda.Elementos).Any(x => x.Estado != "Pendiente"))
            throw new InvalidOperationException("Reto 4.10: el filtro de estado no se conservó.");

        if (primera.Elementos.Select(x => x.Id)
            .Intersect(segunda.Elementos.Select(x => x.Id)).Any())
            throw new InvalidOperationException("Reto 4.10: se repitieron filas entre páginas.");

        Console.WriteLine(
            "Reto 4.10 OK | Estado=Pendiente | cursor=FechaCreacion+Id | 5+5 filas sin repetición");
        Console.WriteLine(segunda.Sql);
    }
    */

}


// FRAGMENTO PDF M04 4.10 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class PaginacionUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public PaginacionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.10 PAGINACION ===");
//
//         var offset = _unidad.Ordenes.ObtenerPaginaOffsetM4(2, 5);
//         var primeraKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(DateTime.MinValue, 0, 5);
//         var cursor = primeraKeyset.Elementos.Last();
//         var segundaKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(
//             cursor.FechaCreacion,
//             cursor.Id,
//             5);
//
//         if (offset.Elementos.Count != 5 ||
//             primeraKeyset.Elementos.Count != 5 ||
//             segundaKeyset.Elementos.Count != 5)
//             throw new InvalidOperationException("4.10: paginacion no devolvio el tamano esperado.");
//
//         if (primeraKeyset.Elementos
//             .Select(x => x.Id)
//             .Intersect(segundaKeyset.Elementos.Select(x => x.Id))
//             .Any())
//             throw new InvalidOperationException("4.10: keyset repitio filas entre paginas.");
//
//         Console.WriteLine("--- OFFSET ---");
//         Console.WriteLine(offset.Sql);
//         Console.WriteLine("--- KEYSET ---");
//         Console.WriteLine(segundaKeyset.Sql);
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.10 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de paginación eficiente
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/PaginacionUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class PaginacionUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public PaginacionUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PAGINACIÓN EFICIENTE ===");
//
//         DemostrarOffsetPagination();
//         DemostrarKeysetPagination();
//         CompararRendimiento();
//         MostrarSql();
//     }
//
//     private void DemostrarOffsetPagination()
//     {
//         Console.WriteLine("\n--- Offset pagination ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var pagina1 = _unidad.Ordenes.ObtenerPaginadoOffset(1, 3);
//         var pagina2 = _unidad.Ordenes.ObtenerPaginadoOffset(2, 3);
//         cronometro.Stop();
//
//         Console.WriteLine($"Página 1: {pagina1.Count} órdenes | Página 2: {pagina2.Count} órdenes | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in pagina1)
//         {
//             Console.WriteLine($"  P1: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//         foreach (var orden in pagina2)
//         {
//             Console.WriteLine($"  P2: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
//
//     private void DemostrarKeysetPagination()
//     {
//         Console.WriteLine("\n--- Keyset pagination ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var pagina1 = _unidad.Ordenes.ObtenerPaginadoKeyset(DateTime.MinValue, 0, 3);
//         if (pagina1.Count > 0)
//         {
//             var ultima = pagina1.Last();
//             var pagina2 = _unidad.Ordenes.ObtenerPaginadoKeyset(ultima.FechaCreacion, 0, 3);
//             cronometro.Stop();
//
//             Console.WriteLine($"Página 1: {pagina1.Count} órdenes | Página 2: {pagina2.Count} órdenes | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//             foreach (var orden in pagina1)
//             {
//                 Console.WriteLine($"  P1: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//             }
//             foreach (var orden in pagina2)
//             {
//                 Console.WriteLine($"  P2: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//             }
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroOffset = Stopwatch.StartNew();
//         for (int i = 1; i <= 10; i++)
//         {
//             _unidad.Ordenes.ObtenerPaginadoOffset(i, 3);
//         }
//         cronometroOffset.Stop();
//
//         var cronometroKeyset = Stopwatch.StartNew();
//         var ultimaFecha = DateTime.MinValue;
//         var ultimoId = 0;
//         for (int i = 0; i < 10; i++)
//         {
//             var pagina = _unidad.Ordenes.ObtenerPaginadoKeyset(ultimaFecha, ultimoId, 3);
//             if (pagina.Count == 0) break;
//             var ultima = pagina.Last();
//             ultimaFecha = ultima.FechaCreacion;
//             ultimoId = 0;
//         }
//         cronometroKeyset.Stop();
//
//         Console.WriteLine($"Offset pagination (10 páginas): {cronometroOffset.ElapsedMilliseconds} ms");
//         Console.WriteLine($"Keyset pagination (10 páginas): {cronometroKeyset.ElapsedMilliseconds} ms");
//     }
//
//     private void MostrarSql()
//     {
//         Console.WriteLine("\n--- SQL de offset pagination ---");
//         var sqlOffset = _unidad.Ordenes.ObtenerSqlPaginadoOffset(2, 3);
//         Console.WriteLine(sqlOffset);
//
//         Console.WriteLine("\n--- SQL de keyset pagination ---");
//         var sqlKeyset = _unidad.Ordenes.ObtenerSqlPaginadoKeyset(new DateTime(2024, 3, 10), 3, 3);
//         Console.WriteLine(sqlKeyset);
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.10 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarPaginadoConFiltro()
// {
//     Console.WriteLine("\n--- Paginación con filtro y proyección ---");
//
//     var pagina1 = _unidad.Ordenes.ObtenerPendientesPaginado(1, 3);
//     var pagina2 = _unidad.Ordenes.ObtenerPendientesPaginado(2, 3);
//
//     Console.WriteLine($"Página 1: {pagina1.Count} órdenes | Página 2: {pagina2.Count} órdenes");
//     foreach (var orden in pagina1)
//     {
//         Console.WriteLine($"  P1: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//     }
//     foreach (var orden in pagina2)
//     {
//         Console.WriteLine($"  P2: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.10 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// DemostrarPaginadoConFiltro();
// ========================================================================
