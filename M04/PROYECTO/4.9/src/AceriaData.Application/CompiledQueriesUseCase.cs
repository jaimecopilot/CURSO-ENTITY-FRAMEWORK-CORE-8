using System.Diagnostics;
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CompiledQueriesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CompiledQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.9 COMPILED QUERIES ===");

        var normal = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");
        var compilada = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");

        if (!normal.Select(o => o.Id).SequenceEqual(compilada.Select(o => o.Id)))
            throw new InvalidOperationException("4.9: consulta normal y compilada no son equivalentes.");

        var swNormal = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
            _ = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");
        swNormal.Stop();

        var swCompilada = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
            _ = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");
        swCompilada.Stop();

        Console.WriteLine($"Normal: {swNormal.ElapsedTicks} ticks | Compilada: {swCompilada.ElapsedTicks} ticks");
        Console.WriteLine(
            "Medicion observacional: no se exige que la compiled query gane en un dataset pequeno; se valida equivalencia y reutilizacion.");
    }

    /*
    // ERROR CONTROLADO M04 4.9 - DELEGADO NO REUTILIZADO
    public void EjecutarErrorCompilarCadaLlamada()
    {
        var primera = _unidad.Ordenes.ObtenerCompilandoCadaVezM4("Pendiente");
        var segunda = _unidad.Ordenes.ObtenerCompilandoCadaVezM4("Pendiente");

        if (!primera.Filas.Select(o => o.Id).SequenceEqual(segunda.Filas.Select(o => o.Id)))
            throw new InvalidOperationException("4.9 error controlado: cambió el resultado al repetir la consulta.");

        if (segunda.Compilaciones != primera.Compilaciones + 1)
            throw new InvalidOperationException("4.9 error controlado: no se detectó una nueva compilación por llamada.");

        Console.WriteLine(
            $"Error controlado 4.9 OK | compilar por llamada crea delegados repetidos: {primera.Compilaciones}->{segunda.Compilaciones}");
    }
    */

    /*
    // RETO M04 4.9 - COMPILED ASYNC QUERY PROYECTADA
    public async Task EjecutarRetoAsync()
    {
        var normal = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente")
            .Select(o => new AceriaData.Application.Dtos.OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .ToList();

        var compilada = await _unidad.Ordenes.ObtenerResumenesCompiladosAsyncM4("Pendiente");

        if (!normal.Select(o => o.NumeroOrden).SequenceEqual(compilada.Select(o => o.NumeroOrden)))
            throw new InvalidOperationException("Reto 4.9: la compiled async query proyectada no es equivalente.");

        Console.WriteLine(
            $"Reto 4.9 OK | CompileAsyncQuery proyectada | Filas={compilada.Count}");
        Console.WriteLine(
            "Coste evitado: parte de la preparación de EF; no elimina red, ejecución SQL ni materialización.");
    }
    */

}


// FRAGMENTO PDF M04 4.9 - PASO 5
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class CompiledQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public CompiledQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.9 COMPILED QUERIES ===");
//
//         var normal = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");
//         var compilada = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");
//
//         if (!normal.Select(o => o.Id).SequenceEqual(compilada.Select(o => o.Id)))
//             throw new InvalidOperationException("4.9: consulta normal y compilada no son equivalentes.");
//
//         var swNormal = Stopwatch.StartNew();
//         for (var i = 0; i < 20; i++)
//             _ = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");
//         swNormal.Stop();
//
//         var swCompilada = Stopwatch.StartNew();
//         for (var i = 0; i < 20; i++)
//             _ = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");
//         swCompilada.Stop();
//
//         Console.WriteLine($"Normal: {swNormal.ElapsedTicks} ticks | Compilada: {swCompilada.ElapsedTicks} ticks");
//         Console.WriteLine(
//             "Medicion observacional: no se exige que la compiled query gane en un dataset pequeno; se valida equivalencia y reutilizacion.");
//     }
// }
// ========================================================================
