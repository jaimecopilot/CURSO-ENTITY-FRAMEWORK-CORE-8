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
