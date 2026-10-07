// ========================================================================
// M04 4.8 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de Split Queries a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerConVariasColeccionesSingleQuery();
//     List<OrdenFabricacion> ObtenerConVariasColeccionesSplitQuery();
//     string ObtenerSqlSingleQuery();
//     string ObtenerSqlSplitQuery();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 02
// SECTION: Paso 4: Crear el caso de uso de Split Queries
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/SplitQueriesUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SplitQueriesUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class SplitQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public SplitQueriesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== SPLIT QUERIES ===");
//
//         DemostrarSingleQuery();
//         DemostrarSplitQuery();
//         CompararRendimiento();
//         MostrarSql();
//     }
//
//     private void DemostrarSingleQuery()
//     {
//         Console.WriteLine("\n--- Single Query (producto cartesiano) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConVariasColeccionesSingleQuery();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas | {orden.OrdenesAleaciones.Count} aleaciones");
//         }
//     }
//
//     private void DemostrarSplitQuery()
//     {
//         Console.WriteLine("\n--- Split Query (sin producto cartesiano) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConVariasColeccionesSplitQuery();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas | {orden.OrdenesAleaciones.Count} aleaciones");
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroSingle = Stopwatch.StartNew();
//         var ordenesSingle = _unidad.Ordenes.ObtenerConVariasColeccionesSingleQuery();
//         cronometroSingle.Stop();
//
//         var cronometroSplit = Stopwatch.StartNew();
//         var ordenesSplit = _unidad.Ordenes.ObtenerConVariasColeccionesSplitQuery();
//         cronometroSplit.Stop();
//
//         Console.WriteLine($"Single Query: {cronometroSingle.ElapsedMilliseconds} ms | Órdenes: {ordenesSingle.Count}");
//         Console.WriteLine($"Split Query: {cronometroSplit.ElapsedMilliseconds} ms | Órdenes: {ordenesSplit.Count}");
//     }
//
//     private void MostrarSql()
//     {
//         Console.WriteLine("\n--- SQL de Single Query ---");
//         var sqlSingle = _unidad.Ordenes.ObtenerSqlSingleQuery();
//         Console.WriteLine(sqlSingle);
//
//         Console.WriteLine("\n--- SQL de Split Query ---");
//         var sqlSplit = _unidad.Ordenes.ObtenerSqlSplitQuery();
//         Console.WriteLine(sqlSplit);
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 03
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SplitQueriesUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<SplitQueriesUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 04
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SplitQueriesUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<SplitQueriesUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 05
// SECTION: Paso 7: Insertar datos de prueba con varias planchas y aleaciones
// SOURCE TARGET: Paso 7: Insertar datos de prueba con varias planchas y aleaciones
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
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     var orden3 = new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2, orden3);
//     context.SaveChanges();
//
//     var planchas = new List<PlanchaAcero>
//     {
//         new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true },
//         new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true },
//         new PlanchaAcero { OrdenId = orden1.Id, Espesor = 9.0, Ancho = 1100, Largo = 2200, Peso = 180.0m, Activa = true },
//         new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true },
//         new PlanchaAcero { OrdenId = orden2.Id, Espesor = 11.0, Ancho = 1300, Largo = 2700, Peso = 290.0m, Activa = true },
//         new PlanchaAcero { OrdenId = orden3.Id, Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m, Activa = true }
//     };
//     context.PlanchasAcero.AddRange(planchas);
//
//     var ordenAleaciones = new List<OrdenAleacion>
//     {
//         new OrdenAleacion { OrdenFabricacionId = orden1.Id, AleacionId = aleacion1.Id, CantidadUtilizada = 1500.5m, EstadoRelacion = "Activa" },
//         new OrdenAleacion { OrdenFabricacionId = orden1.Id, AleacionId = aleacion2.Id, CantidadUtilizada = 800.0m, EstadoRelacion = "Activa" },
//         new OrdenAleacion { OrdenFabricacionId = orden2.Id, AleacionId = aleacion1.Id, CantidadUtilizada = 1200.0m, EstadoRelacion = "Activa" },
//         new OrdenAleacion { OrdenFabricacionId = orden3.Id, AleacionId = aleacion2.Id, CantidadUtilizada = 900.0m, EstadoRelacion = "Activa" }
//     };
//     context.OrdenesAleaciones.AddRange(ordenAleaciones);
//
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 06
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerConVariasColeccionesSingleQuery para eliminar AsNoTrackingWithIdentityResolution y usar AsNoTracking:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConVariasColeccionesSingleQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSingleQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConVariasColeccionesSingleQuery()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSingleQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// Resultado esperado con la solución: las entidades relacionadas se resuelven a instancias únicas.
//
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 08
// SECTION: Paso 1: Modificar el método OnConfiguring del AceriaDbContext:
// SOURCE TARGET: Paso 1: Modificar el método OnConfiguring del AceriaDbContext:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Persistence/AceriaDbContext.cs
// ------------------------------------------------------------------------
// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// {
//     if (!optionsBuilder.IsConfigured)
//     {
//         optionsBuilder
//             .UseSqlServer(
//                 "Server=(localdb)\\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;",
//                 sqlOptions =>
//                 {
//                     sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//                     sqlOptions.CommandTimeout(60);
//                     sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
//                 })
//             .ConfigureWarnings(warnings =>
//                 warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 09
// SECTION: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// SOURCE TARGET: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SplitQueriesUseCase.cs
// ------------------------------------------------------------------------
// var ordenes = context.OrdenesFabricacion
//     .Include(o => o.Planchas)
//     .Include(o => o.OrdenesAleaciones)
//     .ToList();
// ========================================================================

// CANONICAL PDF M04 4.8 - BLOCK 10
// SECTION: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// SOURCE TARGET: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// TARGET_EQUIVALENTE: src/AceriaData.Application/SplitQueriesUseCase.cs
// ------------------------------------------------------------------------
// var ordenes = context.OrdenesFabricacion
//     .Include(o => o.Planchas)
//     .Include(o => o.OrdenesAleaciones)
//     .AsSplitQuery()
//     .ToList();
// Resultado esperado con la solución: la consulta se ejecuta sin error porque AsSplitQuery está aplicado explícitamente.
//
// ========================================================================

