// ========================================================================
// M04 4.11 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 01
// SECTION: Paso 2: Crear el observador de diagnóstico
// SOURCE TARGET: Crear el archivo src/AceriaData.Infrastructure/Diagnostics/EfCoreDiagnosticObserver.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/EfCommandDiagnosticObserver.cs
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

// CANONICAL PDF M04 4.11 - BLOCK 02
// SECTION: Paso 3: Añadir los métodos de diagnóstico a la interfaz del repositorio
// SOURCE TARGET: Modificar src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
//
// namespace AceriaData.Application.Interfaces;
//
// public interface IOrdenRepositorio
// {
//     // ... métodos existentes ...
//
//     List<OrdenFabricacion> ObtenerTodasConDiagnostico();
//     List<OrdenFabricacion> ObtenerPendientesConDiagnostico();
//     int ContarOrdenesConDiagnostico();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 03
// SECTION: Paso 4: Implementar los métodos de diagnóstico en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento411.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerTodasConDiagnostico()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPendientesConDiagnostico()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ToList();
// }
//
// public int ContarOrdenesConDiagnostico()
// {
//     return _context.OrdenesFabricacion.Count();
// }
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 04
// SECTION: Paso 5: Crear el caso de uso de diagnóstico
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/DiagnosticoUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/DiagnosticoRendimientoUseCase.cs
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

// CANONICAL PDF M04 4.11 - BLOCK 05
// SECTION: Paso 6: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/DiagnosticoRendimientoUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<DiagnosticoUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 06
// SECTION: Paso 7: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/DiagnosticoRendimientoUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<DiagnosticoUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 07
// SECTION: Paso 8: Insertar datos de prueba con varias órdenes
// SOURCE TARGET: Paso 8: Insertar datos de prueba con varias órdenes
// TARGET_EQUIVALENTE: src/AceriaData.Console/Program.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var ordenes = new List<OrdenFabricacion>();
//     for (int i = 1; i <= 20; i++)
//     {
//         ordenes.Add(new OrdenFabricacion
//         {
//             NumeroOrden = $"OF-2024-{i:D4}",
//             Cliente = i % 2 == 0 ? "Constructora del Norte" : "Constructora del Sur",
//             Estado = i % 3 == 0 ? "EnProceso" : "Pendiente",
//             FechaCreacion = new DateTime(2024, 1, 1).AddDays(i)
//         });
//     }
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 08
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Modificar el método DemostrarConteoDeConsultas para no incrementar el contador manualmente:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
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

// CANONICAL PDF M04 4.11 - BLOCK 09
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Paso 11: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
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

// CANONICAL PDF M04 4.11 - BLOCK 10
// SECTION: Paso 1: Modificar el observador para detectar consultas lentas:
// SOURCE TARGET: Paso 1: Modificar el observador para detectar consultas lentas:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/EfCommandDiagnosticObserver.cs
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

// CANONICAL PDF M04 4.11 - BLOCK 11
// SECTION: Paso 2: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 2: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/DiagnosticoRendimientoUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarDeteccionConObservador()
// {
//     Console.WriteLine("\n--- Detección de consultas lentas con observador ---");
//
//     var observer = new EfCoreDiagnosticObserver(message => Console.WriteLine(message));
//     DiagnosticListener.AllListeners.Subscribe(observer);
//
//     var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//     var pendientes = _unidad.Ordenes.ObtenerPendientesConDiagnostico();
// }
// ========================================================================

// CANONICAL PDF M04 4.11 - BLOCK 12
// SECTION: Paso 3: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 3: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/DiagnosticoRendimientoUseCase.cs
// ------------------------------------------------------------------------
// DemostrarDeteccionConObservador();
// ========================================================================

