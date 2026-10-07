// ========================================================================
// M04 4.9 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 01
// SECTION: Paso 2: Crear la clase de consultas compiladas
// SOURCE TARGET: Crear el archivo src/AceriaData.Infrastructure/Repositories/OrdenConsultasCompiladas.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento49.cs
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public static class OrdenConsultasCompiladas
// {
//     public static readonly Func<AceriaDbContext, string, List<OrdenFabricacion>> ObtenerPorEstado =
//         EF.CompileQuery((AceriaDbContext context, string estado) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => o.Estado == estado)
//                 .OrderBy(o => o.FechaCreacion)
//                 .ToList());
//
//     public static readonly Func<AceriaDbContext, string, List<OrdenFabricacion>> ObtenerPorCliente =
//         EF.CompileQuery((AceriaDbContext context, string cliente) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => o.Cliente == cliente)
//                 .OrderBy(o => o.FechaCreacion)
//                 .ToList());
//
//     public static readonly Func<AceriaDbContext, DateTime, DateTime, List<OrdenFabricacion>> ObtenerPorRangoDeFechas =
//         EF.CompileQuery((AceriaDbContext context, DateTime desde, DateTime hasta) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
//                 .OrderBy(o => o.FechaCreacion)
//                 .ToList());
//
//     public static readonly Func<AceriaDbContext, int, OrdenFabricacion?> ObtenerPorId =
//         EF.CompileQuery((AceriaDbContext context, int id) =>
//             context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .FirstOrDefault(o => o.Id == id));
//
//     public static readonly Func<AceriaDbContext, int> ContarOrdenes =
//         EF.CompileQuery((AceriaDbContext context) =>
//             context.OrdenesFabricacion.Count());
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 02
// SECTION: Paso 3: Añadir los métodos de Compiled Queries a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerPorEstadoCompilada(string estado);
//     List<OrdenFabricacion> ObtenerPorEstadoNoCompilada(string estado);
//     List<OrdenFabricacion> ObtenerPorClienteCompilada(string cliente);
//     OrdenFabricacion? ObtenerPorIdCompilada(int id);
//     int ContarOrdenesCompilada();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 03
// SECTION: Paso 4: Implementar los métodos de Compiled Queries en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento49.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorEstadoCompilada(string estado)
// {
//     return OrdenConsultasCompiladas.ObtenerPorEstado(_context, estado);
// }
//
// public List<OrdenFabricacion> ObtenerPorEstadoNoCompilada(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.FechaCreacion)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorClienteCompilada(string cliente)
// {
//     return OrdenConsultasCompiladas.ObtenerPorCliente(_context, cliente);
// }
//
// public OrdenFabricacion? ObtenerPorIdCompilada(int id)
// {
//     return OrdenConsultasCompiladas.ObtenerPorId(_context, id);
// }
//
// public int ContarOrdenesCompilada()
// {
//     return OrdenConsultasCompiladas.ContarOrdenes(_context);
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 04
// SECTION: Paso 5: Crear el caso de uso de Compiled Queries
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/CompiledQueriesUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/CompiledQueriesUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class CompiledQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public CompiledQueriesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== COMPILED QUERIES ===");
//
//         DemostrarConsultaCompilada();
//         DemostrarConsultaNoCompilada();
//         CompararRendimiento();
//     }
//
//     private void DemostrarConsultaCompilada()
//     {
//         Console.WriteLine("\n--- Consulta compilada ---");
//
//         var cronometro = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             var ordenes = _unidad.Ordenes.ObtenerPorEstadoCompilada("Pendiente");
//         }
//         cronometro.Stop();
//
//         Console.WriteLine($"100 ejecuciones: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarConsultaNoCompilada()
//     {
//         Console.WriteLine("\n--- Consulta no compilada ---");
//
//         var cronometro = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             var ordenes = _unidad.Ordenes.ObtenerPorEstadoNoCompilada("Pendiente");
//         }
//         cronometro.Stop();
//
//         Console.WriteLine($"100 ejecuciones: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroCompilada = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             _unidad.Ordenes.ObtenerPorEstadoCompilada("Pendiente");
//         }
//         cronometroCompilada.Stop();
//
//         var cronometroNoCompilada = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             _unidad.Ordenes.ObtenerPorEstadoNoCompilada("Pendiente");
//         }
//         cronometroNoCompilada.Stop();
//
//         Console.WriteLine($"Compilada: {cronometroCompilada.ElapsedMilliseconds} ms");
//         Console.WriteLine($"No compilada: {cronometroNoCompilada.ElapsedMilliseconds} ms");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 05
// SECTION: Paso 6: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/CompiledQueriesUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<CompiledQueriesUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 06
// SECTION: Paso 7: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/CompiledQueriesUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<CompiledQueriesUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 07
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
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 08
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerPorEstadoCompilada para compilar la consulta cada vez que se llama:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorEstadoCompilada(string estado)
// {
//     var consultaCompilada = EF.CompileQuery((AceriaDbContext context, string e) =>
//         context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == e)
//             .OrderBy(o => o.FechaCreacion)
//             .ToList());
//
//     return consultaCompilada(_context, estado);
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 09
// SECTION: Paso 11: Diagnosticar un error común
// SOURCE TARGET: Paso 11: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// return OrdenConsultasCompiladas.ObtenerPorEstado(_context, estado);
// Resultado esperado con la solución: la consulta se compila una sola vez y se reutiliza en todas las llamadas.
//
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 10
// SECTION: Paso 1: Añadir las consultas compiladas a OrdenConsultasCompiladas:
// SOURCE TARGET: Paso 1: Añadir las consultas compiladas a OrdenConsultasCompiladas:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento49.cs
// ------------------------------------------------------------------------
// public static readonly Func<AceriaDbContext, string, List<OrdenFabricacion>> ObtenerPorEstadoCompleta =
//     EF.CompileQuery((AceriaDbContext context, string estado) =>
//         context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == estado)
//             .OrderBy(o => o.FechaCreacion)
//             .ToList());
//
// public static readonly Func<AceriaDbContext, string, List<OrdenResumenDto>> ObtenerPorEstadoProyectada =
//     EF.CompileQuery((AceriaDbContext context, string estado) =>
//         context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Estado == estado)
//             .OrderBy(o => o.FechaCreacion)
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .ToList());
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 11
// SECTION: Paso 2: Añadir los métodos a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 2: Añadir los métodos a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenResumenDto> ObtenerPorEstadoProyectadaCompilada(string estado);
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 12
// SECTION: Paso 3: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 3: Implementar el método en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento49.cs
// ------------------------------------------------------------------------
// public List<OrdenResumenDto> ObtenerPorEstadoProyectadaCompilada(string estado)
// {
//     return OrdenConsultasCompiladas.ObtenerPorEstadoProyectada(_context, estado);
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 13
// SECTION: Paso 4: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 4: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/CompiledQueriesUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarProyectadaCompilada()
// {
//     Console.WriteLine("\n--- Consulta compilada con proyección ---");
//
//     var cronometro = Stopwatch.StartNew();
//     for (int i = 0; i < 100; i++)
//     {
//         _unidad.Ordenes.ObtenerPorEstadoProyectadaCompilada("Pendiente");
//     }
//     cronometro.Stop();
//
//     Console.WriteLine($"100 ejecuciones: {cronometro.ElapsedMilliseconds} ms");
// }
// ========================================================================

// CANONICAL PDF M04 4.9 - BLOCK 14
// SECTION: Paso 5: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 5: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/CompiledQueriesUseCase.cs
// ------------------------------------------------------------------------
// DemostrarProyectadaCompilada();
// ========================================================================

