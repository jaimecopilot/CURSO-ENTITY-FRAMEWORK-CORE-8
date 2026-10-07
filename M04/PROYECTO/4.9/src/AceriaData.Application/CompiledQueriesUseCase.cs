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

// CANONICAL INLINE M04 4.9 - BLOCK 04
// SECTION: Paso 5: Crear el caso de uso de Compiled Queries
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/CompiledQueriesUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class CompiledQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public CompiledQueriesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== COMPILED QUERIES ===");
//
//         DemostrarConsultaCompilada();
//         DemostrarConsultaNoCompilada();
//         CompararRendimiento();
//     }
//
//     private void DemostrarConsultaCompilada()
//     {
//         Console.WriteLine("\n--- Consulta compilada ---");
//
//         var cronometro = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             var ordenes = _unidad.Ordenes.ObtenerPorEstadoCompilada("Pendiente");
//         }
//         cronometro.Stop();
//
//         Console.WriteLine($"100 ejecuciones: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarConsultaNoCompilada()
//     {
//         Console.WriteLine("\n--- Consulta no compilada ---");
//
//         var cronometro = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             var ordenes = _unidad.Ordenes.ObtenerPorEstadoNoCompilada("Pendiente");
//         }
//         cronometro.Stop();
//
//         Console.WriteLine($"100 ejecuciones: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroCompilada = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             _unidad.Ordenes.ObtenerPorEstadoCompilada("Pendiente");
//         }
//         cronometroCompilada.Stop();
//
//         var cronometroNoCompilada = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             _unidad.Ordenes.ObtenerPorEstadoNoCompilada("Pendiente");
//         }
//         cronometroNoCompilada.Stop();
//
//         Console.WriteLine($"Compilada: {cronometroCompilada.ElapsedMilliseconds} ms");
//         Console.WriteLine($"No compilada: {cronometroNoCompilada.ElapsedMilliseconds} ms");
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 09
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Paso 11: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// return OrdenConsultasCompiladas.ObtenerPorEstado(_context, estado);
// Resultado esperado con la solución: la consulta se compila una sola vez y se reutiliza en todas las llamadas.
//
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 10
// SECTION: Paso 1: Añadir las consultas compiladas a OrdenConsultasCompiladas:
// SOURCE TARGET: Paso 1: Añadir las consultas compiladas a OrdenConsultasCompiladas:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public static readonly Func<AceriaDbContext, string, List<OrdenFabricacion>> ObtenerPorEstadoCompleta =
//     EF.CompileQuery((AceriaDbContext context, string estado) =>
//         context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == estado)
//             .OrderBy(o => o.FechaCreacion)
//             .ToList());
//
// public static readonly Func<AceriaDbContext, string, List<OrdenResumenDto>> ObtenerPorEstadoProyectada =
//     EF.CompileQuery((AceriaDbContext context, string estado) =>
//         context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == estado)
//             .OrderBy(o => o.FechaCreacion)
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .ToList());
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 13
// SECTION: Paso 4: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 4: Añadir la demostración en el caso de uso:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarProyectadaCompilada()
// {
//     Console.WriteLine("\n--- Consulta compilada con proyección ---");
//
//     var cronometro = Stopwatch.StartNew();
//     for (int i = 0; i < 100; i++)
//     {
//         _unidad.Ordenes.ObtenerPorEstadoProyectadaCompilada("Pendiente");
//     }
//     cronometro.Stop();
//
//     Console.WriteLine($"100 ejecuciones: {cronometro.ElapsedMilliseconds} ms");
// }
// ========================================================================

// CANONICAL INLINE M04 4.9 - BLOCK 14
// SECTION: Paso 5: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 5: Llamar al método desde Ejecutar:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// DemostrarProyectadaCompilada();
// ========================================================================
