// ========================================================================
// M04 4.10 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de paginación a la interfaz del repositorio
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
//     List<OrdenResumenDto> ObtenerPaginadoOffset(int pagina, int tamanoPagina);
//     List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina);
//     int ContarOrdenesPaginadas();
//     string ObtenerSqlPaginadoOffset(int pagina, int tamanoPagina);
//     string ObtenerSqlPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina);
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de paginación en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento410.cs
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPaginadoOffset(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
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
// public List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha || (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
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
// public int ContarOrdenesPaginadas()
// {
//     return _context.OrdenesFabricacion.Count();
// }
//
// public string ObtenerSqlPaginadoOffset(int pagina, int tamanoPagina)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Skip((pagina - 1) * tamanoPagina)
//         .Take(tamanoPagina)
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
//
// public string ObtenerSqlPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha || (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
//         .Take(tamanoPagina)
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

// CANONICAL PDF M04 4.10 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de paginación eficiente
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/PaginacionUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/PaginacionUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class PaginacionUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public PaginacionUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PAGINACIÓN EFICIENTE ===");
//
//         DemostrarOffsetPagination();
//         DemostrarKeysetPagination();
//         CompararRendimiento();
//         MostrarSql();
//     }
//
//     private void DemostrarOffsetPagination()
//     {
//         Console.WriteLine("\n--- Offset pagination ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var pagina1 = _unidad.Ordenes.ObtenerPaginadoOffset(1, 3);
//         var pagina2 = _unidad.Ordenes.ObtenerPaginadoOffset(2, 3);
//         cronometro.Stop();
//
//         Console.WriteLine($"Página 1: {pagina1.Count} órdenes | Página 2: {pagina2.Count} órdenes | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in pagina1)
//         {
//             Console.WriteLine($"  P1: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//         foreach (var orden in pagina2)
//         {
//             Console.WriteLine($"  P2: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
//
//     private void DemostrarKeysetPagination()
//     {
//         Console.WriteLine("\n--- Keyset pagination ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var pagina1 = _unidad.Ordenes.ObtenerPaginadoKeyset(DateTime.MinValue, 0, 3);
//         if (pagina1.Count > 0)
//         {
//             var ultima = pagina1.Last();
//             var pagina2 = _unidad.Ordenes.ObtenerPaginadoKeyset(ultima.FechaCreacion, 0, 3);
//             cronometro.Stop();
//
//             Console.WriteLine($"Página 1: {pagina1.Count} órdenes | Página 2: {pagina2.Count} órdenes | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//             foreach (var orden in pagina1)
//             {
//                 Console.WriteLine($"  P1: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//             }
//             foreach (var orden in pagina2)
//             {
//                 Console.WriteLine($"  P2: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//             }
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroOffset = Stopwatch.StartNew();
//         for (int i = 1; i <= 10; i++)
//         {
//             _unidad.Ordenes.ObtenerPaginadoOffset(i, 3);
//         }
//         cronometroOffset.Stop();
//
//         var cronometroKeyset = Stopwatch.StartNew();
//         var ultimaFecha = DateTime.MinValue;
//         var ultimoId = 0;
//         for (int i = 0; i < 10; i++)
//         {
//             var pagina = _unidad.Ordenes.ObtenerPaginadoKeyset(ultimaFecha, ultimoId, 3);
//             if (pagina.Count == 0) break;
//             var ultima = pagina.Last();
//             ultimaFecha = ultima.FechaCreacion;
//             ultimoId = 0;
//         }
//         cronometroKeyset.Stop();
//
//         Console.WriteLine($"Offset pagination (10 páginas): {cronometroOffset.ElapsedMilliseconds} ms");
//         Console.WriteLine($"Keyset pagination (10 páginas): {cronometroKeyset.ElapsedMilliseconds} ms");
//     }
//
//     private void MostrarSql()
//     {
//         Console.WriteLine("\n--- SQL de offset pagination ---");
//         var sqlOffset = _unidad.Ordenes.ObtenerSqlPaginadoOffset(2, 3);
//         Console.WriteLine(sqlOffset);
//
//         Console.WriteLine("\n--- SQL de keyset pagination ---");
//         var sqlKeyset = _unidad.Ordenes.ObtenerSqlPaginadoKeyset(new DateTime(2024, 3, 10), 3, 3);
//         Console.WriteLine(sqlKeyset);
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/PaginacionUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<PaginacionUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/PaginacionUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<PaginacionUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 06
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

// CANONICAL PDF M04 4.10 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerPaginadoKeyset para usar solo la fecha como clave:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha)
//         .OrderBy(o => o.FechaCreacion)
//         .Take(tamanoPagina)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion \        })
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPaginadoKeyset(DateTime ultimaFecha, int ultimoId, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.FechaCreacion > ultimaFecha || (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
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
// Resultado esperado con la solución: la paginación devuelve las filas correctas aunque haya fechas duplicadas.
//
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 09
// SECTION: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenResumenDto> ObtenerPendientesPaginado(int pagina, int tamanoPagina);
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento410.cs
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPendientesPaginado(int pagina, int tamanoPagina)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .ThenBy(o => o.Id)
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

// CANONICAL PDF M04 4.10 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/PaginacionUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarPaginadoConFiltro()
// {
//     Console.WriteLine("\n--- Paginación con filtro y proyección ---");
//
//     var pagina1 = _unidad.Ordenes.ObtenerPendientesPaginado(1, 3);
//     var pagina2 = _unidad.Ordenes.ObtenerPendientesPaginado(2, 3);
//
//     Console.WriteLine($"Página 1: {pagina1.Count} órdenes | Página 2: {pagina2.Count} órdenes");
//     foreach (var orden in pagina1)
//     {
//         Console.WriteLine($"  P1: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//     }
//     foreach (var orden in pagina2)
//     {
//         Console.WriteLine($"  P2: {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.10 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/PaginacionUseCase.cs
// ------------------------------------------------------------------------
// DemostrarPaginadoConFiltro();
// ========================================================================

