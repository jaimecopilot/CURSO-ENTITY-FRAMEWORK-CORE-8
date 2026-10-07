using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SolucionesNMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");
        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
            throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");

        Console.WriteLine($"Include: {include.ConsultasSql} consulta.");
        Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");
    }

    /*
    // ERROR CONTROLADO M04 4.5 - EVITAR N+1 NO IMPLICA UNA CONSULTA
    public void EjecutarDiagnosticoNoTodoEsUnaConsulta()
    {
        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
            throw new InvalidOperationException("4.5 diagnóstico: Include/proyección no tienen la forma esperada.");
        if (split.ConsultasSql == 1)
            throw new InvalidOperationException("4.5 diagnóstico: SplitQuery se ha confundido con una única consulta.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.5 diagnóstico: se esperaban 3 comandos SplitQuery y se obtuvieron {split.ConsultasSql}.");

        Console.WriteLine(
            $"Error controlado 4.5 OK | evitar N+1 no implica 1 comando | Include={include.ConsultasSql} | Proyeccion={proyeccion.ConsultasSql} | Split={split.ConsultasSql}");
    }
    */

    /*
    // RETO M04 4.5 - GRAFO COMPLETO SPLIT QUERY
    public void EjecutarRetoGrafoCompleto()
    {
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (split.Ordenes != 5)
            throw new InvalidOperationException($"Reto 4.5: se esperaban 5 órdenes y se obtuvieron {split.Ordenes}.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException($"Reto 4.5: se esperaban 3 comandos y se obtuvieron {split.ConsultasSql}.");
        if (split.ElementosRelacionados != 9)
            throw new InvalidOperationException(
                $"Reto 4.5: se esperaban 9 elementos relacionados y se obtuvieron {split.ElementosRelacionados}.");

        Console.WriteLine(
            $"Reto 4.5 OK | Ordenes={split.Ordenes} | Relacionados={split.ElementosRelacionados} | Consultas SQL={split.ConsultasSql}");
        Console.WriteLine(
            "Justificacion: consulta raiz + colección Planchas + colección OrdenesAleaciones; ThenInclude(Aleacion) viaja con la consulta de esa colección.");
    }
    */

}


// EJEMPLO DEL PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class SolucionesNMasUnoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");
//         var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
//         var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
//         var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();
//
//         if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
//             throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");
//         if (split.ConsultasSql != 3)
//             throw new InvalidOperationException(
//                 $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");
//
//         Console.WriteLine($"Include: {include.ConsultasSql} consulta.");
//         Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");
//         Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 03
// PASO: Paso 4: Crear el caso de uso de solución al problema N+1
// UBICACIÓN INDICADA: Crear el archivo src/AceriaData.Application/UseCases/SolucionN1UseCase.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class SolucionN1UseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public SolucionN1UseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== SOLUCIÓN AL PROBLEMA N+1 ===");
//
//         DemostrarConInclude();
//         DemostrarConThenInclude();
//         DemostrarConSplitQuery();
//         DemostrarConProyeccion();
//         CompararTodasLasSoluciones();
//     }
//
//     private void DemostrarConInclude()
//     {
//         Console.WriteLine("\n--- Solución con Include ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConPlanchasInclude();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas");
//         }
//     }
//
//     private void DemostrarConThenInclude()
//     {
//         Console.WriteLine("\n--- Solución con ThenInclude ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConAleacionesThenInclude();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.OrdenesAleaciones.Count} aleaciones");
//         }
//     }
//
//     private void DemostrarConSplitQuery()
//     {
//         Console.WriteLine("\n--- Solución con AsSplitQuery ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConPlanchasYDetalleSplitQuery();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             var detalle = orden.Detalle == null ? "Sin detalle" : orden.Detalle.ComposicionQuimica;
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas | Detalle: {detalle}");
//         }
//     }
//
//     private void DemostrarConProyeccion()
//     {
//         Console.WriteLine("\n--- Solución con proyección ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenConPlanchasYDetalleProyeccion();
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             var detalle = resumen.Detalle == null ? "Sin detalle" : resumen.Detalle.ComposicionQuimica;
//             Console.WriteLine($"  {resumen.NumeroOrden}: {resumen.Planchas.Count} planchas | Detalle: {detalle}");
//         }
//     }
//
//     private void CompararTodasLasSoluciones()
//     {
//         Console.WriteLine("\n--- Comparación de todas las soluciones ---");
//
//         var cronometroInclude = Stopwatch.StartNew();
//         var ordenesInclude = _unidad.Ordenes.ObtenerConPlanchasInclude();
//         cronometroInclude.Stop();
//
//         var cronometroThenInclude = Stopwatch.StartNew();
//         var ordenesThenInclude = _unidad.Ordenes.ObtenerConAleacionesThenInclude();
//         cronometroThenInclude.Stop();
//
//         var cronometroSplitQuery = Stopwatch.StartNew();
//         var ordenesSplitQuery = _unidad.Ordenes.ObtenerConPlanchasYDetalleSplitQuery();
//         cronometroSplitQuery.Stop();
//
//         var cronometroProyeccion = Stopwatch.StartNew();
//         var resumenesProyeccion = _unidad.Ordenes.ObtenerResumenConPlanchasYDetalleProyeccion();
//         cronometroProyeccion.Stop();
//
//         Console.WriteLine($"Include: {cronometroInclude.ElapsedMilliseconds} ms");
//         Console.WriteLine($"ThenInclude: {cronometroThenInclude.ElapsedMilliseconds} ms");
//         Console.WriteLine($"AsSplitQuery: {cronometroSplitQuery.ElapsedMilliseconds} ms");
//         Console.WriteLine($"Proyección: {cronometroProyeccion.ElapsedMilliseconds} ms");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 11
// PASO: Paso 3: Añadir la demostración en el caso de uso:
// UBICACIÓN INDICADA: Paso 3: Añadir la demostración en el caso de uso:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// private void DemostrarCompletaConSplitQuery()
// {
//     Console.WriteLine("\n--- Completa con AsSplitQuery ---");
//
//     var cronometro = Stopwatch.StartNew();
//     var ordenes = _unidad.Ordenes.ObtenerOrdenesCompletasConSplitQuery();
//     cronometro.Stop();
//
//     Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     foreach (var orden in ordenes)
//     {
//         var detalle = orden.Detalle == null ? "Sin detalle" : orden.Detalle.ComposicionQuimica;
//         Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas | {orden.OrdenesAleaciones.Count} aleaciones | Detalle: {detalle}");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 12
// PASO: Paso 4: Llamar al método desde Ejecutar:
// UBICACIÓN INDICADA: Paso 4: Llamar al método desde Ejecutar:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// DemostrarCompletaConSplitQuery();
// ========================================================================
