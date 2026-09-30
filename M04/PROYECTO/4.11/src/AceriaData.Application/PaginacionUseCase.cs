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
}
