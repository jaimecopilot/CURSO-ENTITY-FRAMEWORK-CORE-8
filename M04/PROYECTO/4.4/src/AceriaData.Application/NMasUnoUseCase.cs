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


// FRAGMENTO PDF M04 4.4 - PASO 5
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

// CANONICAL INLINE M04 4.4 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso del problema N+1
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ProblemaN1UseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
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

// CANONICAL INLINE M04 4.4 - BLOCK 10
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
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

// CANONICAL INLINE M04 4.4 - BLOCK 11
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// DemostrarDetalleN1();
// ========================================================================
