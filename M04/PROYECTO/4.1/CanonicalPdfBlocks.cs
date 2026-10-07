// ========================================================================
// M04 4.1 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 01
// SECTION: Paso 2: Añadir métodos de análisis de SQL a la interfaz del repositorio
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
//     string ObtenerSqlConsultasPendientes();
//     string ObtenerSqlConsultasConInclude();
//     string ObtenerSqlConsultasConProyeccion();
//     string ObtenerSqlConsultasConFiltroGlobal();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de análisis de SQL en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento41.cs
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasPendientes()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion);
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlConsultasConInclude()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Where(o => o.Cliente == "Constructora del Norte");
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlConsultasConProyeccion()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .Select(o => new { o.NumeroOrden, o.Cliente });
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlConsultasConFiltroGlobal()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Include(o => o.Planchas);
//
//     return consulta.ToQueryString();
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de análisis de SQL
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/AnalisisSqlUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/AnalisisSqlUseCase.cs
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class AnalisisSqlUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public AnalisisSqlUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== ANÁLISIS DEL SQL GENERADO ===");
//
//         DemostrarSqlConsultasPendientes();
//         DemostrarSqlConsultasConInclude();
//         DemostrarSqlConsultasConProyeccion();
//         DemostrarSqlConsultasConFiltroGlobal();
//     }
//
//     private void DemostrarSqlConsultasPendientes()
//     {
//         Console.WriteLine("\n--- SQL de consultas pendientes ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasPendientes();
//         Console.WriteLine(sql);
//     }
//
//     private void DemostrarSqlConsultasConInclude()
//     {
//         Console.WriteLine("\n--- SQL de consultas con Include ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasConInclude();
//         Console.WriteLine(sql);
//     }
//
//     private void DemostrarSqlConsultasConProyeccion()
//     {
//         Console.WriteLine("\n--- SQL de consultas con proyección ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasConProyeccion();
//         Console.WriteLine(sql);
//     }
//
//     private void DemostrarSqlConsultasConFiltroGlobal()
//     {
//         Console.WriteLine("\n--- SQL de consultas con filtro global de Soft Delete ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasConFiltroGlobal();
//         Console.WriteLine(sql);
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/AnalisisSqlUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<AnalisisSqlUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/AnalisisSqlUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<AnalisisSqlUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con planchas
// SOURCE TARGET: Paso 7: Insertar datos de prueba con planchas
// TARGET_EQUIVALENTE: src/AceriaData.Console/Program.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2);
//     context.SaveChanges();
//
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerSqlConsultasConProyeccion para llamar a ToList antes de ToQueryString:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasConProyeccion()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .Select(o => new { o.NumeroOrden, o.Cliente })
//         .ToList();
//
//     return "La consulta ya se ha ejecutado, no se puede obtener el SQL.";
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasConProyeccion()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Where(o => o.Estado == "Pendiente")
//         .Select(o => new { o.NumeroOrden, o.Cliente });
//
//     return consulta.ToQueryString();
// }
// Resultado esperado con la solución: el método devuelve el SQL sin ejecutar la consulta.
//
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 09
// SECTION: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// string ObtenerSqlConsultasConMultiplesInclude();
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento41.cs
// ------------------------------------------------------------------------
// public string ObtenerSqlConsultasConMultiplesInclude()
// {
//     var consulta = _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .Where(o => o.Estado == "Pendiente");
//
//     return consulta.ToQueryString();
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/AnalisisSqlUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarSqlConsultasConMultiplesInclude()
// {
//     Console.WriteLine("\n--- SQL de consultas con múltiples Include ---");
//     var sql = _unidad.Ordenes.ObtenerSqlConsultasConMultiplesInclude();
//     Console.WriteLine(sql);
// }
// ========================================================================

// CANONICAL PDF M04 4.1 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/AnalisisSqlUseCase.cs
// ------------------------------------------------------------------------
// DemostrarSqlConsultasConMultiplesInclude();
// ========================================================================

