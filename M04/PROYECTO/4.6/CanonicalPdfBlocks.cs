// ========================================================================
// M04 4.6 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de over-fetching a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerOrdenesCompletas();
//     List<OrdenResumenDto> ObtenerResumenesProyectados();
//     List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina);
//     List<OrdenResumenDto> ObtenerResumenesFiltradosYProyectados(string estado);void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de over-fetching en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento46.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerOrdenesCompletas()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenResumenDto> ObtenerResumenesProyectados()
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
// public List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
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
// public List<OrdenResumenDto> ObtenerResumenesFiltradosYProyectados(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.FechaCreacion)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de over-fetching
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/OverFetchingUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/OverFetchingUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class OverFetchingUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public OverFetchingUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== OVER-FETCHING ===");
//
//         DemostrarEntidadesCompletas();
//         DemostrarProyeccion();
//         DemostrarPaginacion();
//         DemostrarFiltroYProyeccion();
//         CompararRendimiento();
//     }
//
//     private void DemostrarEntidadesCompletas()
//     {
//         Console.WriteLine("\n--- Entidades completas (over-fetching) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesCompletas();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//         }
//     }
//
//     private void DemostrarProyeccion()
//     {
//         Console.WriteLine("\n--- Proyección (sin over-fetching de columnas) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenesProyectados();
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado}");
//         }
//     }
//
//     private void DemostrarPaginacion()
//     {
//         Console.WriteLine("\n--- Paginación (sin over-fetching de filas) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenesPaginados(1, 3);
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado}");
//         }
//     }
//
//     private void DemostrarFiltroYProyeccion()
//     {
//         Console.WriteLine("\n--- Filtro y proyección ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var resumenes = _unidad.Ordenes.ObtenerResumenesFiltradosYProyectados("Pendiente");
//         cronometro.Stop();
//
//         Console.WriteLine($"Resúmenes pendientes: {resumenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado}");
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroCompletas = Stopwatch.StartNew();
//         var ordenesCompletas = _unidad.Ordenes.ObtenerOrdenesCompletas();
//         cronometroCompletas.Stop();
//
//         var cronometroProyectadas = Stopwatch.StartNew();
//         var resumenesProyectados = _unidad.Ordenes.ObtenerResumenesProyectados();
//         cronometroProyectadas.Stop();
//
//         Console.WriteLine($"Entidades completas: {cronometroCompletas.ElapsedMilliseconds} ms | Registros: {ordenesCompletas.Count}");
//         Console.WriteLine($"Proyección: {cronometroProyectadas.ElapsedMilliseconds} ms | Registros: {resumenesProyectados.Count}");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/OverFetchingUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<OverFetchingUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/OverFetchingUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<OverFetchingUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 06
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0006", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 6, 18) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0007", Cliente = "Constructora del Este", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 7, 22) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0008", Cliente = "Constructora del Oeste", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 8, 30) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerResumenesPaginados para aplicar ToList antes del Skip y Take:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList()
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerResumenesPaginados(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// Resultado esperado con la solución: el SQL incluye OFFSET y FETCH y solo se transfieren las filas de la página.
//
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 09
// SECTION: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// string ObtenerSqlEntidadesCompletas();
// string ObtenerSqlProyeccionResumen();
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 10
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento46.cs
// ------------------------------------------------------------------------
// public string ObtenerSqlEntidadesCompletas()
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden);
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlProyeccionResumen()
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         });
//
//     return consulta.ToQueryString();
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/OverFetchingUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarSqlComparativo()
// {
//     Console.WriteLine("\n--- SQL de entidades completas ---");
//     var sqlCompletas = _unidad.Ordenes.ObtenerSqlEntidadesCompletas();
//     Console.WriteLine(sqlCompletas);
//
//     Console.WriteLine("\n--- SQL de proyección ---");
//     var sqlProyeccion = _unidad.Ordenes.ObtenerSqlProyeccionResumen();
//     Console.WriteLine(sqlProyeccion);
// }
// ========================================================================

// CANONICAL PDF M04 4.6 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/OverFetchingUseCase.cs
// ------------------------------------------------------------------------
// DemostrarSqlComparativo();
// ========================================================================

