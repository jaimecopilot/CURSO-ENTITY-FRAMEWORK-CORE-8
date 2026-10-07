// ========================================================================
// M04 4.4 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos del problema N+1 a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasN1();
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasSinN1();
//     List<OrdenFabricacion> ObtenerOrdenesConDetalleN1();
//     List<OrdenFabricacion> ObtenerOrdenesConDetalleSinN1();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos del problema N+1 en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento44.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasN1()
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     foreach (var orden in ordenes)
//     {
//         var planchas = _context.PlanchasAcero
//             .AsNoTracking()
//             .Where(p => p.OrdenId == orden.Id)
//             .ToList();
//
//         orden.Planchas = planchas;
//     }
//
//     return ordenes;
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasSinN1()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConDetalleN1()
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     foreach (var orden in ordenes)
//     {
//         var detalle = _context.DetallesOrden
//             .AsNoTracking()
//             .FirstOrDefault(d => d.OrdenId == orden.Id);
//
//         orden.Detalle = detalle;
//     }
//
//     return ordenes;
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConDetalleSinN1()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso del problema N+1
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ProblemaN1UseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/NMasUnoUseCase.cs
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

// CANONICAL PDF M04 4.4 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/NMasUnoUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<ProblemaN1UseCase>();
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/NMasUnoUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ProblemaN1UseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con varias órdenes y planchas
// SOURCE TARGET: Paso 7: Insertar datos de prueba con varias órdenes y planchas
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0006", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 6, 18) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
//
//     var planchas = new List<PlanchaAcero>
//     {
//         new PlanchaAcero { OrdenId = ordenes[0].Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true },
//         new PlanchaAcero { OrdenId = ordenes[0].Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true },
//         new PlanchaAcero { OrdenId = ordenes[1].Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true },
//         new PlanchaAcero { OrdenId = ordenes[2].Id, Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m, Activa = true },
//         new PlanchaAcero { OrdenId = ordenes[3].Id, Espesor = 9.0, Ancho = 1100, Largo = 2200, Peso = 180.0m, Activa = true },
//         new PlanchaAcero { OrdenId = ordenes[4].Id, Espesor = 11.0, Ancho = 1300, Largo = 2700, Peso = 290.0m, Activa = true }
//     };
//
//     context.PlanchasAcero.AddRange(planchas);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerOrdenesConPlanchasSinN1 para usar AsNoTracking después del Include:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasSinN1()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 08
// SECTION: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenFabricacion> ObtenerOrdenesConDetalleN1();
// List<OrdenFabricacion> ObtenerOrdenesConDetalleSinN1();
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 09
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento44.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesConDetalleN1()
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     foreach (var orden in ordenes)
//     {
//         var detalle = _context.DetallesOrden
//             .AsNoTracking()
//             .FirstOrDefault(d => d.OrdenId == orden.Id);
//
//         orden.Detalle = detalle;
//     }
//
//     return ordenes;
// }
//
// public List<OrdenFabricacion> ObtenerOrdenesConDetalleSinN1()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.4 - BLOCK 10
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/NMasUnoUseCase.cs
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

// CANONICAL PDF M04 4.4 - BLOCK 11
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/NMasUnoUseCase.cs
// ------------------------------------------------------------------------
// DemostrarDetalleN1();
// ========================================================================

