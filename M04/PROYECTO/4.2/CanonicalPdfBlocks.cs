// ========================================================================
// M04 4.2 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de tracking a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerConTracking();
//     List<OrdenFabricacion> ObtenerSinTracking();
//     int ContarEntidadesRastreadas();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de tracking en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento42.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConTracking()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerSinTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public int ContarEntidadesRastreadas()
// {
//     return _context.ChangeTracker.Entries().Count();
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de tracking
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/TrackingUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TrackingUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;public class TrackingUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public TrackingUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== TRACKING Y NO TRACKING ===");
//
//         DemostrarConTracking();
//         DemostrarSinTracking();
//         CompararRendimiento();
//     }
//
//     private void DemostrarConTracking()
//     {
//         Console.WriteLine("\n--- Con Tracking ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerConTracking();
//         var rastreadas = _unidad.Ordenes.ContarEntidadesRastreadas();
//
//         Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//         Console.WriteLine($"Entidades rastreadas: {rastreadas}");
//     }
//
//     private void DemostrarSinTracking()
//     {
//         Console.WriteLine("\n--- Sin Tracking ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerSinTracking();
//         var rastreadas = _unidad.Ordenes.ContarEntidadesRastreadas();
//
//         Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//         Console.WriteLine($"Entidades rastreadas: {rastreadas}");
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroCon = Stopwatch.StartNew();
//         var ordenesCon = _unidad.Ordenes.ObtenerConTracking();
//         cronometroCon.Stop();
//
//         var cronometroSin = Stopwatch.StartNew();
//         var ordenesSin = _unidad.Ordenes.ObtenerSinTracking();
//         cronometroSin.Stop();
//
//         Console.WriteLine($"Con Tracking: {cronometroCon.ElapsedMilliseconds} ms | Órdenes: {ordenesCon.Count}");
//         Console.WriteLine($"Sin Tracking: {cronometroSin.ElapsedMilliseconds} ms | Órdenes: {ordenesSin.Count}");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TrackingUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<TrackingUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TrackingUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<TrackingUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con varias órdenes
// SOURCE TARGET: Paso 7: Insertar datos de prueba con varias órdenes
// TARGET_EQUIVALENTE: src/AceriaData.Console/Program.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var ordenes = new List<OrdenFabricacion>
//     {
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0004", Cliente = "Constructora del Oeste", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 4, 5) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método DemostrarSinTracking para usar AsNoTracking en un DbContext distinto:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// private void DemostrarSinTrackingAislado()
// {
//     Console.WriteLine("\n--- Sin Tracking (DbContext aislado) ---");
//
//     using var scope = _provider.CreateScope();
//     var repositorio = scope.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
//
//     var ordenes = repositorio.ObtenerSinTracking();
//     var rastreadas = repositorio.ContarEntidadesRastreadas();
//
//     Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//     Console.WriteLine($"Entidades rastreadas: {rastreadas}");
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 08
// SECTION: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenFabricacion> ObtenerConPlanchasConTracking();
// List<OrdenFabricacion> ObtenerConPlanchasSinTracking();
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 09
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento42.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasConTracking()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConPlanchasSinTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 10
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TrackingUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarTrackingConInclude()
// {
//     Console.WriteLine("\n--- Tracking con Include ---");
//
//     using var scopeCon = _provider.CreateScope();
//     var repoCon = scopeCon.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
//     var ordenesCon = repoCon.ObtenerConPlanchasConTracking();
//     var rastreadasCon = repoCon.ContarEntidadesRastreadas();
//     Console.WriteLine($"Con Tracking: órdenes: {ordenesCon.Count} | entidades rastreadas: {rastreadasCon}");
//
//     using var scopeSin = _provider.CreateScope();
//     var repoSin = scopeSin.ServiceProvider.GetRequiredService<IOrdenRepositorio>();
//     var ordenesSin = repoSin.ObtenerConPlanchasSinTracking();
//     var rastreadasSin = repoSin.ContarEntidadesRastreadas();
//     Console.WriteLine($"Sin Tracking: órdenes: {ordenesSin.Count} | entidades rastreadas: {rastreadasSin}");
// }
// ========================================================================

// CANONICAL PDF M04 4.2 - BLOCK 11
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TrackingUseCase.cs
// ------------------------------------------------------------------------
// DemostrarTrackingConInclude();
// ========================================================================

