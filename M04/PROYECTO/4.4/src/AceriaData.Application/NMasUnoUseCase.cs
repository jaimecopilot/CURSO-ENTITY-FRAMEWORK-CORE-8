using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class NMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");
        var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();

        if (metrica.ConsultasSql != metrica.Ordenes + 1)
            throw new InvalidOperationException(
                $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");

        Console.WriteLine(
            $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");
        Console.WriteLine(
            "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.");
    }

    /*
    // ERROR CONTROLADO M04 4.4 - MEDICION CONTAMINADA POR ESTADO PREVIO
    public void EjecutarErrorContadorSinReset()
    {
        var limpia = _unidad.Ordenes.EjecutarNMasUnoM4();
        var contaminada = _unidad.Ordenes.EjecutarNMasUnoSinResetM4();

        if (limpia.ConsultasSql != limpia.Ordenes + 1)
            throw new InvalidOperationException("4.4 error controlado: la medicion limpia no representa N+1.");
        if (contaminada.ConsultasSql <= contaminada.Ordenes + 1)
            throw new InvalidOperationException("4.4 error controlado: el contador sin Reset no quedó contaminado.");

        Console.WriteLine(
            $"Error controlado 4.4 OK | limpio={limpia.ConsultasSql} | sin reset={contaminada.ConsultasSql}");
    }
    */

    /*
    // RETO M04 4.4 - DETALLE POR ORDEN
    public void EjecutarRetoDetalle()
    {
        var metrica = _unidad.Ordenes.EjecutarNMasUnoDetalleM4();

        if (metrica.ConsultasSql != metrica.Ordenes + 1)
            throw new InvalidOperationException(
                $"Reto 4.4: se esperaban N+1 comandos; obtenidos {metrica.ConsultasSql} para N={metrica.Ordenes}.");

        Console.WriteLine(
            $"Reto 4.4 OK | Ordenes={metrica.Ordenes} | Detalles={metrica.Detalles} | Consultas SQL={metrica.ConsultasSql}");
        Console.WriteLine(
            "Comparacion conceptual: una carga anticipada o una proyeccion puede evitar la consulta adicional por orden; se implementa en 4.5.");
    }
    */

}


// EJEMPLO DEL PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class NMasUnoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");
//         var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();
//
//         if (metrica.ConsultasSql != metrica.Ordenes + 1)
//             throw new InvalidOperationException(
//                 $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");
//
//         Console.WriteLine(
//             $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");
//         Console.WriteLine(
//             "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 03
// PASO: Paso 4: Crear el caso de uso del problema N+1
// UBICACIÓN INDICADA: Crear el archivo src/AceriaData.Application/UseCases/ProblemaN1UseCase.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ProblemaN1UseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ProblemaN1UseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PROBLEMA N+1 ===");
//
//         DemostrarConN1();
//         DemostrarSinN1();
//         CompararRendimiento();
//     }
//
//     private void DemostrarConN1()
//     {
//         Console.WriteLine("\n--- Con N+1 (planchas) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasN1();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         Console.WriteLine($"Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas");
//         }
//     }
//
//     private void DemostrarSinN1()
//     {
//         Console.WriteLine("\n--- Sin N+1 (planchas) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasSinN1();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         Console.WriteLine($"Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas");
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroN1 = Stopwatch.StartNew();
//         var ordenesN1 = _unidad.Ordenes.ObtenerOrdenesConPlanchasN1();
//         cronometroN1.Stop();
//
//         var cronometroSinN1 = Stopwatch.StartNew();
//         var ordenesSinN1 = _unidad.Ordenes.ObtenerOrdenesConPlanchasSinN1();
//         cronometroSinN1.Stop();
//
//         Console.WriteLine($"Con N+1: {cronometroN1.ElapsedMilliseconds} ms | Órdenes: {ordenesN1.Count}");
//         Console.WriteLine($"Sin N+1: {cronometroSinN1.ElapsedMilliseconds} ms | Órdenes: {ordenesSinN1.Count}");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 10
// PASO: Paso 3: Añadir la demostración en el caso de uso:
// UBICACIÓN INDICADA: Paso 3: Añadir la demostración en el caso de uso:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// private void DemostrarDetalleN1()
// {
//     Console.WriteLine("\n--- Detalle con N+1 ---");
//     var cronometroN1 = Stopwatch.StartNew();
//     var ordenesN1 = _unidad.Ordenes.ObtenerOrdenesConDetalleN1();
//     cronometroN1.Stop();
//     Console.WriteLine($"Tiempo con N+1: {cronometroN1.ElapsedMilliseconds} ms");
//     foreach (var orden in ordenesN1)
//     {
//         var detalle = orden.Detalle?.ComposicionQuimica ?? "Sin detalle";
//         Console.WriteLine($"  {orden.NumeroOrden}: {detalle}");
//     }
//
//     Console.WriteLine("\n--- Detalle sin N+1 ---");
//     var cronometroSinN1 = Stopwatch.StartNew();
//     var ordenesSinN1 = _unidad.Ordenes.ObtenerOrdenesConDetalleSinN1();
//     cronometroSinN1.Stop();
//     Console.WriteLine($"Tiempo sin N+1: {cronometroSinN1.ElapsedMilliseconds} ms");
//     foreach (var orden in ordenesSinN1)
//     {
//         var detalle = orden.Detalle?.ComposicionQuimica ?? "Sin detalle";
//         Console.WriteLine($"  {orden.NumeroOrden}: {detalle}");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 11
// PASO: Paso 4: Llamar al método desde Ejecutar:
// UBICACIÓN INDICADA: Paso 4: Llamar al método desde Ejecutar:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// DemostrarDetalleN1();
// ========================================================================
