// ========================================================================
// M04 4.12 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos del checklist a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerSinOptimizar();
//     List<OrdenFabricacion> ObtenerOptimizado();
//     List<OrdenResumenDto> ObtenerResumenOptimizado();
//     List<OrdenFabricacion> ObtenerConRelacionesOptimizado();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos del checklist en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento412.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerSinOptimizar()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// /// <summary>
// /// Obtiene las órdenes con optimización de tracking.
// /// Usa AsNoTracking porque es una consulta de solo lectura.
// /// </summary>
// public List<OrdenFabricacion> ObtenerOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// /// <summary>
// /// Obtiene los resúmenes de las órdenes.
// /// Usa proyección para reducir el volumen de datos transferidos.
// /// Usa AsNoTracking porque es una consulta de solo lectura.
// /// </summary>
// public List<OrdenResumenDto> ObtenerResumenOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
//
// /// <summary>
// /// Obtiene las órdenes con planchas y detalle.
// /// Usa Include para evitar N+1.
// /// Usa AsSplitQuery para evitar el producto cartesiano.
// /// Usa AsNoTrackingWithIdentityResolution para evitar instancias duplicadas.
// /// </summary>
// public List<OrdenFabricacion> ObtenerConRelacionesOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso del checklist de rendimiento
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ChecklistRendimientoUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/ChecklistRendimientoUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ChecklistRendimientoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ChecklistRendimientoUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CHECKLIST DE RENDIMIENTO ===");
//
//         MostrarChecklist();
//         CompararSinOptimizarVsOptimizado();
//         CompararEntidadesVsProyeccion();
//         CompararSinIncludeVsConInclude();
//     }
//
//     private void MostrarChecklist()
//     {
//         Console.WriteLine("\n--- Checklist de rendimiento ---");
//         Console.WriteLine("1. ¿Se usa AsNoTracking en consultas de solo lectura?");
//         Console.WriteLine("2. ¿Se proyectan solo las columnas necesarias?");
//         Console.WriteLine("3. ¿Se evita el problema N+1 con Include?");
//         Console.WriteLine("4. ¿Se evita el producto cartesiano con AsSplitQuery?");
//         Console.WriteLine("5. ¿Se aplican filtros y paginación en el servidor?");
//         Console.WriteLine("6. ¿Se evitan funciones en Where que impidan índices?");
//         Console.WriteLine("7. ¿Se usan Compiled Queries en consultas frecuentes?");
//         Console.WriteLine("8. ¿Se miden los tiempos y se cuentan las consultas?");
//     }
//
//     private void CompararSinOptimizarVsOptimizado()
//     {
//         Console.WriteLine("\n--- Sin optimizar vs optimizado (tracking) ---");
//
//         var cronometroSin = Stopwatch.StartNew();
//         var ordenesSin = _unidad.Ordenes.ObtenerSinOptimizar();
//         cronometroSin.Stop();
//
//         var cronometroCon = Stopwatch.StartNew();
//         var ordenesCon = _unidad.Ordenes.ObtenerOptimizado();
//         cronometroCon.Stop();
//
//         Console.WriteLine($"Sin optimizar: {cronometroSin.ElapsedMilliseconds} ms | Órdenes: {ordenesSin.Count}");
//         Console.WriteLine($"Optimizado: {cronometroCon.ElapsedMilliseconds} ms | Órdenes: {ordenesCon.Count}");
//     }
//
//     private void CompararEntidadesVsProyeccion()
//     {
//         Console.WriteLine("\n--- Entidades completas vs proyección ---");
//
//         var cronometroEntidades = Stopwatch.StartNew();
//         var ordenesEntidades = _unidad.Ordenes.ObtenerOptimizado();
//         cronometroEntidades.Stop();
//
//         var cronometroProyeccion = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenOptimizado();
//         cronometroProyeccion.Stop();
//
//         Console.WriteLine($"Entidades completas: {cronometroEntidades.ElapsedMilliseconds} ms | Órdenes: {ordenesEntidades.Count}");
//         Console.WriteLine($"Proyección: {cronometroProyeccion.ElapsedMilliseconds} ms | Resúmenes: {resumenes.Count}");
//     }
//
//     private void CompararSinIncludeVsConInclude()
//     {
//         Console.WriteLine("\n--- Sin Include vs con Include y AsSplitQuery ---");
//
//         var cronometroSinInclude = Stopwatch.StartNew();
//         var ordenesSinInclude = _unidad.Ordenes.ObtenerOptimizado();
//         cronometroSinInclude.Stop();
//
//         var cronometroConInclude = Stopwatch.StartNew();
//         var ordenesConInclude = _unidad.Ordenes.ObtenerConRelacionesOptimizado();
//         cronometroConInclude.Stop();
//
//         Console.WriteLine($"Sin Include: {cronometroSinInclude.ElapsedMilliseconds} ms | Órdenes: {ordenesSinInclude.Count}");
//         Console.WriteLine($"Con Include y AsSplitQuery: {cronometroConInclude.ElapsedMilliseconds} ms | Órdenes: {ordenesConInclude.Count}");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/ChecklistRendimientoUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<ChecklistRendimientoUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/ChecklistRendimientoUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ChecklistRendimientoUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 06
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
//
//     var planchas = new List<PlanchaAcero>();
//     foreach (var orden in ordenes)
//     {
//         planchas.Add(new PlanchaAcero { OrdenId = orden.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true });
//     }
//
//     context.PlanchasAcero.AddRange(planchas);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerConRelacionesOptimizado para eliminar AsSplitQuery:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConRelacionesOptimizado()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConRelacionesOptimizado()
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

// CANONICAL PDF M04 4.12 - BLOCK 09
// SECTION: Paso 1: Consulta sin optimizar:
// SOURCE TARGET: Paso 1: Consulta sin optimizar:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerSinOptimizarCompleta()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 10
// SECTION: Paso 2: Consulta optimizada con el checklist completo:
// SOURCE TARGET: Paso 2: Consulta optimizada con el checklist completo:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// /// <summary>
// /// Obtiene las órdenes pendientes con planchas y detalle.
// /// Usa AsNoTrackingWithIdentityResolution para evitar tracking y duplicados.
// /// Usa Include para evitar N+1.
// /// Usa AsSplitQuery para evitar el producto cartesiano.
// /// Usa filtro por estado para reducir el número de filas.
// /// Usa paginación para limitar el número de resultados.
// /// </summary>
// public List<OrdenFabricacion> ObtenerOptimizadoCompleta(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 11
// SECTION: Paso 3: Medir el tiempo antes y después:
// SOURCE TARGET: Paso 3: Medir el tiempo antes y después:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// private void CompararCompleta()
// {
//     Console.WriteLine("\n--- Completa sin optimizar vs optimizada ---");
//
//     var cronometroSin = Stopwatch.StartNew();
//     var ordenesSin = _unidad.Ordenes.ObtenerSinOptimizarCompleta();
//     cronometroSin.Stop();
//
//     var cronometroCon = Stopwatch.StartNew();
//     var ordenesCon = _unidad.Ordenes.ObtenerOptimizadoCompleta(1, 10);
//     cronometroCon.Stop();
//
//     Console.WriteLine($"Sin optimizar: {cronometroSin.ElapsedMilliseconds} ms | Órdenes: {ordenesSin.Count}");
//     Console.WriteLine($"Optimizado: {cronometroCon.ElapsedMilliseconds} ms | Órdenes: {ordenesCon.Count}");
// }
// ========================================================================

// CANONICAL PDF M04 4.12 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/ChecklistRendimientoUseCase.cs
// ------------------------------------------------------------------------
// CompararCompleta();
// ========================================================================

