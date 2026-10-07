// ========================================================================
// M04 4.7 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de consultas ineficientes a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerPorClienteConFuncion(string cliente);
//     List<OrdenFabricacion> ObtenerPorClienteSinFuncion(string cliente);
//     List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado);
//     List<OrdenFabricacion> ObtenerPorEstadoDirecto(string estado);
//     string ObtenerSqlConFuncion(string cliente);
//     string ObtenerSqlSinFuncion(string cliente);
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de consultas ineficientes en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento47.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorClienteConFuncion(string cliente)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente.ToLower() == cliente.ToLower())
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorClienteSinFuncion(string cliente)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente == cliente)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado)
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     return ordenes.Where(o => EsEstadoValido(o.Estado, estado)).ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorEstadoDirecto(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public string ObtenerSqlConFuncion(string cliente)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente.ToLower() == cliente.ToLower());
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlSinFuncion(string cliente)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente == cliente);
//
//     return consulta.ToQueryString();
// }
//
// private static bool EsEstadoValido(string estadoActual, string estadoBuscado)
// {
//     return string.Equals(estadoActual, estadoBuscado, StringComparison.OrdinalIgnoreCase);
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de consultas ineficientes
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ConsultasIneficientesUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TraduccionConsultasUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ConsultasIneficientesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ConsultasIneficientesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CONSULTAS INEFICIENTES ===");
//
//         DemostrarConFuncion();
//         DemostrarSinFuncion();
//         DemostrarMetodoPersonalizado();
//         DemostrarEstadoDirecto();
//         CompararSql();
//     }
//
//     private void DemostrarConFuncion()
//     {
//         Console.WriteLine("\n--- Consulta con función en Where ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorClienteConFuncion("Constructora del Norte");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarSinFuncion()
//     {
//         Console.WriteLine("\n--- Consulta sin función en Where ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorClienteSinFuncion("Constructora del Norte");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarMetodoPersonalizado()
//     {
//         Console.WriteLine("\n--- Consulta con método personalizado ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorMetodoPersonalizado("Pendiente");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarEstadoDirecto()
//     {
//         Console.WriteLine("\n--- Consulta con estado directo ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorEstadoDirecto("Pendiente");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void CompararSql()
//     {
//         Console.WriteLine("\n--- SQL con función ---");
//         var sqlConFuncion = _unidad.Ordenes.ObtenerSqlConFuncion("Constructora del Norte");
//         Console.WriteLine(sqlConFuncion);
//
//         Console.WriteLine("\n--- SQL sin función ---");
//         var sqlSinFuncion = _unidad.Ordenes.ObtenerSqlSinFuncion("Constructora del Norte");
//         Console.WriteLine(sqlSinFuncion);
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TraduccionConsultasUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<ConsultasIneficientesUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TraduccionConsultasUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ConsultasIneficientesUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 06
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0006", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 6, 18) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerPorMetodoPersonalizado para usar el método personalizado directamente en Where:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => EsEstadoValido(o.Estado, estado))
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado)
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     return ordenes.Where(o => EsEstadoValido(o.Estado, estado)).ToList();
// }
// Resultado esperado con la solución: el método funciona y filtra en memoria. La desventaja es que carga todas las órdenes en memoria.
//
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 09
// SECTION: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenFabricacion> ObtenerPorFormatoNumeroOrden();
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento47.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorFormatoNumeroOrden()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.NumeroOrden.StartsWith("OF-") && o.NumeroOrden.Length == 12)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TraduccionConsultasUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarFormatoNumeroOrden()
// {
//     Console.WriteLine("\n--- Formato de número de orden ---");
//
//     var ordenes = _unidad.Ordenes.ObtenerPorFormatoNumeroOrden();
//     Console.WriteLine($"Órdenes con formato válido: {ordenes.Count}");
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden}");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.7 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/TraduccionConsultasUseCase.cs
// ------------------------------------------------------------------------
// DemostrarFormatoNumeroOrden();
// ========================================================================

