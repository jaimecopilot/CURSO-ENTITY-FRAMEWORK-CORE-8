/*
 // RETO M04 4.11 - OBSERVADOR DIAGNOSTICLISTENER
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AceriaData.Infrastructure;

public sealed class EfCommandDiagnosticObserver :
    IObserver<DiagnosticListener>,
    IObserver<KeyValuePair<string, object?>>,
    IDisposable
{
    private readonly TimeSpan _threshold;
    private IDisposable? _allListenersSubscription;
    private IDisposable? _efCoreSubscription;

    private EfCommandDiagnosticObserver(TimeSpan threshold)
    {
        _threshold = threshold;
    }

    public int CommandExecutedCount { get; private set; }
    public int SlowQueryCount { get; private set; }

    public static EfCommandDiagnosticObserver Start(TimeSpan threshold)
    {
        var observer = new EfCommandDiagnosticObserver(threshold);
        observer._allListenersSubscription = DiagnosticListener.AllListeners.Subscribe(observer);
        return observer;
    }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Microsoft.EntityFrameworkCore")
        {
            _efCoreSubscription?.Dispose();
            _efCoreSubscription = listener.Subscribe(this);
        }
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key == RelationalEventId.CommandExecuted.Name &&
            value.Value is CommandExecutedEventData data)
        {
            CommandExecutedCount++;
            if (data.Duration >= _threshold)
                SlowQueryCount++;
        }
    }

    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void Dispose()
    {
        _efCoreSubscription?.Dispose();
        _allListenersSubscription?.Dispose();
    }
}
*/

// CANONICAL INLINE M04 4.11 - BLOCK 01
// SECTION: Paso 2: Crear el observador de diagnóstico
// SOURCE TARGET: Crear el archivo src/AceriaData.Infrastructure/Diagnostics/EfCoreDiagnosticObserver.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
//
// namespace AceriaData.Infrastructure.Diagnostics;
//
// public class EfCoreDiagnosticObserver : IObserver<DiagnosticListener>
// {
//     private readonly Action<string> _log;
//
//     public EfCoreDiagnosticObserver(Action<string> log)
//     {
//         _log = log;
//     }
//
//     public void OnNext(DiagnosticListener listener)
//     {
//         if (listener.Name == "Microsoft.EntityFrameworkCore")
//         {
//             listener.Subscribe(new EfCoreCommandObserver(_log));
//         }
//     }
//
//     public void OnError(Exception error) { }
//     public void OnCompleted() { }
// }
//
// public class EfCoreCommandObserver : IObserver<KeyValuePair<string, object>>
// {
//     private readonly Action<string> _log;
//
//     public EfCoreCommandObserver(Action<string> log)
//     {
//         _log = log;
//     }
//
//     public void OnNext(KeyValuePair<string, object> value)
//     {
//         if (value.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")
//         {
//             _log($"Comando ejecutado: {value.Value}");
//         }
//     }
//
//     public void OnError(Exception error) { }
//     public void OnCompleted() { }
// }
// ========================================================================

// CANONICAL INLINE M04 4.11 - BLOCK 04
// SECTION: Paso 5: Crear el caso de uso de diagnóstico
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/DiagnosticoUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class DiagnosticoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public DiagnosticoUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== DIAGNÓSTICO ===");
//
//         DemostrarMedicionDeTiempo();
//         DemostrarConteoDeConsultas();
//         DemostrarDeteccionDeConsultasLentas();
//     }
//
//     private void DemostrarMedicionDeTiempo()
//     {
//         Console.WriteLine("\n--- Medición de tiempo ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarConteoDeConsultas()
//     {
//         Console.WriteLine("\n--- Conteo de consultas ---");
//
//         var contador = 0;
//         var cronometro = Stopwatch.StartNew();
//
//         contador++;
//         var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//
//         contador++;
//         var pendientes = _unidad.Ordenes.ObtenerPendientesConDiagnostico();
//
//         contador++;
//         var total = _unidad.Ordenes.ContarOrdenesConDiagnostico();
//
//         cronometro.Stop();
//
//         Console.WriteLine($"Consultas ejecutadas: {contador} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Pendientes: {pendientes.Count} | Total: {total}");
//     }
//
//     private void DemostrarDeteccionDeConsultasLentas()
//     {
//         Console.WriteLine("\n--- Detección de consultas lentas ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//         cronometro.Stop();
//
//         var tiempoMs = cronometro.ElapsedMilliseconds;
//         var umbralMs = 50;
//
//         if (tiempoMs > umbralMs)
//         {
//             Console.WriteLine($"¡ALERTA! La consulta tardó {tiempoMs} ms, superando el umbral de {umbralMs} ms.");
//         }
//         else
//         {
//             Console.WriteLine($"Consulta dentro del umbral: {tiempoMs} ms (umbral: {umbralMs} ms).");
//         }
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.11 - BLOCK 08
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Modificar el método DemostrarConteoDeConsultas para no incrementar el contador manualmente:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarConteoDeConsultas()
// {
//     Console.WriteLine("\n--- Conteo de consultas ---");
//
//     var contador = 0;
//     var cronometro = Stopwatch.StartNew();
//
//     var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//     var pendientes = _unidad.Ordenes.ObtenerPendientesConDiagnostico();
//     var total = _unidad.Ordenes.ContarOrdenesConDiagnostico();
//
//     cronometro.Stop();
//
//     Console.WriteLine($"Consultas ejecutadas: {contador} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     Console.WriteLine($"Órdenes: {ordenes.Count} | Pendientes: {pendientes.Count} | Total: {total}");
// }
// ========================================================================

// CANONICAL INLINE M04 4.11 - BLOCK 09
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Paso 11: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private int _contadorConsultas;
//
// private void DemostrarConteoDeConsultasConObservador()
// {
//     Console.WriteLine("\n--- Conteo de consultas con observador ---");
//
//     _contadorConsultas = 0;
//     var observer = new EfCoreDiagnosticObserver(message =>
//     {
//         if (message.StartsWith("Comando ejecutado")) _contadorConsultas++;
//     });
//     DiagnosticListener.AllListeners.Subscribe(observer);
//
//     var cronometro = Stopwatch.StartNew();
//     var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//     var pendientes = _unidad.Ordenes.ObtenerPendientesConDiagnostico();
//     var total = _unidad.Ordenes.ContarOrdenesConDiagnostico();
//     cronometro.Stop();
//
//     Console.WriteLine($"Consultas ejecutadas: {_contadorConsultas} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
// }
// Resultado esperado con la solución: el contador muestra el número real de consultas ejecutadas.
//
// ========================================================================

// CANONICAL INLINE M04 4.11 - BLOCK 10
// SECTION: Paso 1: Modificar el observador para detectar consultas lentas:
// SOURCE TARGET: Paso 1: Modificar el observador para detectar consultas lentas:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public class EfCoreCommandObserver : IObserver<KeyValuePair<string, object>>
// {
//     private readonly Action<string> _log;
//     private readonly long _umbralMs;
//
//     public EfCoreCommandObserver(Action<string> log, long umbralMs)
//     {
//         _log = log;
//         _umbralMs = umbralMs;
//     }
//
//     public void OnNext(KeyValuePair<string, object> value)
//     {
//         if (value.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")
//         {
//             var tipo = value.Value.GetType();
//             var propiedadDuracion = tipo.GetProperty("Duration");
//             if (propiedadDuracion != null)
//             {
//                 var duracion = (TimeSpan)propiedadDuracion.GetValue(value.Value)!;
//                 if (duracion.TotalMilliseconds > _umbralMs)
//                 {
//                     _log($"Consulta lenta detectada: {duracion.TotalMilliseconds} ms");
//                 }
//             }
//         }
//     }
//
//     public void OnError(Exception error) { }
//     public void OnCompleted() { }
// }
// ========================================================================
