// ========================================================================
// M04 4.5 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de solución al problema N+1 a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerConPlanchasInclude();
//     List<OrdenFabricacion> ObtenerConAleacionesThenInclude();
//     List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery();
//     List<OrdenConPlanchasYDetalleDto> ObtenerResumenConPlanchasYDetalleProyeccion();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de solución en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento45.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasInclude()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConAleacionesThenInclude()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenConPlanchasYDetalleDto> ObtenerResumenConPlanchasYDetalleProyeccion()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConPlanchasYDetalleDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             Planchas = o.Planchas.Select(p => new PlanchaDto
//             {
//                 Id = p.Id,
//                 Espesor = p.Espesor,
//                 Ancho = p.Ancho,
//                 Largo = p.Largo,
//                 Peso = p.Peso,
//                 Activa = p.Activa
//             }).ToList(),
//             Detalle = o.Detalle == null ? null : new DetalleDto
//             {
//                 ComposicionQuimica = o.Detalle.ComposicionQuimica,
//                 TemperaturaColada = o.Detalle.TemperaturaColada,
//                 Notas = o.Detalle.Notas
//             }
//         })
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de solución al problema N+1
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/SolucionN1UseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SolucionesNMasUnoUseCase.cs
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

// CANONICAL PDF M04 4.5 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SolucionesNMasUnoUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<SolucionN1UseCase>();
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SolucionesNMasUnoUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<SolucionN1UseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con planchas, detalle y aleaciones
// SOURCE TARGET: Paso 7: Insertar datos de prueba con planchas, detalle y aleaciones
// TARGET_EQUIVALENTE: src/AceriaData.Console/Program.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var aleacion1 = new Aleacion { Nombre = "AISI 1045", Codigo = "A1045", PorcentajeCarbono = 0.45m, PorcentajeManganeso = 0.75m, Activo = true };
//     var aleacion2 = new Aleacion { Nombre = "AISI 4140", Codigo = "A4140", PorcentajeCarbono = 0.40m, PorcentajeManganeso = 0.85m, Activo = true };
//     context.Aleaciones.AddRange(aleacion1, aleacion2);
//     context.SaveChanges();
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
//     context.PlanchasAcero.AddRange(planchas);
//
//     var detalle1 = new DetalleOrden { OrdenId = ordenes[0].Id, ComposicionQuimica = "C: 0.45%, Mn: 0.75%", TemperaturaColada = 1550.5 };
//     context.DetallesOrden.Add(detalle1);
//
//     var ordenAleacion1 = new OrdenAleacion { OrdenFabricacionId = ordenes[0].Id, AleacionId = aleacion1.Id, CantidadUtilizada = 1500.5m, EstadoRelacion = "Activa" };
//     var ordenAleacion2 = new OrdenAleacion { OrdenFabricacionId = ordenes[0].Id, AleacionId = aleacion2.Id, CantidadUtilizada = 800.0m, EstadoRelacion = "Activa" };
//     var ordenAleacion3 = new OrdenAleacion { OrdenFabricacionId = ordenes[1].Id, AleacionId = aleacion1.Id, CantidadUtilizada = 1200.0m, EstadoRelacion = "Activa" };
//     context.OrdenesAleaciones.AddRange(ordenAleacion1, ordenAleacion2, ordenAleacion3);
//
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerConPlanchasYDetalleSplitQuery para eliminar AsSplitQuery:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// Resultado esperado con la solución: la consulta se divide en varias y no se produce el producto cartesiano.
//
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 09
// SECTION: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenFabricacion> ObtenerOrdenesCompletasConSplitQuery();
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento45.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesCompletasConSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.5 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SolucionesNMasUnoUseCase.cs
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

// CANONICAL PDF M04 4.5 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SolucionesNMasUnoUseCase.cs
// ------------------------------------------------------------------------
// DemostrarCompletaConSplitQuery();
// ========================================================================

