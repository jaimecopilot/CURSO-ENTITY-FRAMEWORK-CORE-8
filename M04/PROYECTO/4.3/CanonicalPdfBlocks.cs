// ========================================================================
// M04 4.3 · BLOQUES C# LITERALES DEL M04_PRACTICA CANÓNICO
// Fuente editorial: M04/PRACTICA/M04_PRACTICA.md
// Blob fuente: 20d27323122798c7ac9ef4a85ed61d9ced4f0cb6
// Bloques comentados: no alteran el estado ejecutable final.
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de resolución de identidad a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerConPlanchasAsNoTracking();
//     List<OrdenFabricacion> ObtenerConPlanchasConResolucionIdentidad();
//     int ContarPlanchasInstanciadas(List<OrdenFabricacion> ordenes);
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento43.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConPlanchasAsNoTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConPlanchasConResolucionIdentidad()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public int ContarPlanchasInstanciadas(List<OrdenFabricacion> ordenes)
// {
//     var instancias = new HashSet<PlanchaAcero>(ReferenceEqualityComparer.Instance);
//     foreach (var orden in ordenes)
//     {
//         foreach (var plancha in orden.Planchas)
//         {
//             instancias.Add(plancha);
//         }
//     }
//     return instancias.Count;
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de resolución de identidad
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ResolucionIdentidadUseCase.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/IdentityResolutionUseCase.cs
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ResolucionIdentidadUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ResolucionIdentidadUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== ASNOTRACKING VS ASNOTRACKINGWITHIDENTITYRESOLUTION ===");
//
//         DemostrarAsNoTracking();
//         DemostrarConResolucionIdentidad();
//         CompararRendimiento();
//     }
//
//     private void DemostrarAsNoTracking()
//     {
//         Console.WriteLine("\n--- AsNoTracking ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerConPlanchasAsNoTracking();
//         var instancias = _unidad.Ordenes.ContarPlanchasInstanciadas(ordenes);
//
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         Console.WriteLine($"Instancias de planchas: {instancias}");
//     }
//
//     private void DemostrarConResolucionIdentidad()
//     {
//         Console.WriteLine("\n--- AsNoTrackingWithIdentityResolution ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerConPlanchasConResolucionIdentidad();
//         var instancias = _unidad.Ordenes.ContarPlanchasInstanciadas(ordenes);
//
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         Console.WriteLine($"Instancias de planchas: {instancias}");
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroNoTracking = Stopwatch.StartNew();
//         var ordenesNoTracking = _unidad.Ordenes.ObtenerConPlanchasAsNoTracking();
//         cronometroNoTracking.Stop();
//
//         var cronometroResolucion = Stopwatch.StartNew();
//         var ordenesResolucion = _unidad.Ordenes.ObtenerConPlanchasConResolucionIdentidad();
//         cronometroResolucion.Stop();
//
//         Console.WriteLine($"AsNoTracking: {cronometroNoTracking.ElapsedMilliseconds} ms | Órdenes: {ordenesNoTracking.Count}");
//         Console.WriteLine($"AsNoTrackingWithIdentityResolution: {cronometroResolucion.ElapsedMilliseconds} ms | Órdenes: {ordenesResolucion.Count}");
//     }
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 04
// SECTION: Paso 5: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// TARGET_EQUIVALENTE: src/AceriaData.Application/IdentityResolutionUseCase.cs
// ------------------------------------------------------------------------
// services.AddScoped<ResolucionIdentidadUseCase>();
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 05
// SECTION: Paso 6: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// TARGET_EQUIVALENTE: src/AceriaData.Application/IdentityResolutionUseCase.cs
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ResolucionIdentidadUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 06
// SECTION: Paso 7: Insertar datos de prueba con planchas compartidas
// SOURCE TARGET: Paso 7: Insertar datos de prueba con planchas compartidas
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
//     var orden3 = new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2, orden3);
//     context.SaveChanges();
//
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
//     var plancha4 = new PlanchaAcero { OrdenId = orden3.Id, Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m, Activa = true };
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3, plancha4);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ContarPlanchasInstanciadas para usar HashSet<PlanchaAcero> sin ReferenceEqualityComparer:
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// public int ContarPlanchasInstanciadas(List<OrdenFabricacion> ordenes)
// {
//     var instancias = new HashSet<PlanchaAcero>();
//     foreach (var orden in ordenes)
//     {
//         foreach (var plancha in orden.Planchas)
//         {
//             instancias.Add(plancha);
//         }
//     }
//     return instancias.Count;
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// TARGET_EQUIVALENTE: CanonicalPdfBlocks.cs; equivalente funcional validado por Test-M04.ps1
// ------------------------------------------------------------------------
// var instancias = new HashSet<PlanchaAcero>(ReferenceEqualityComparer.Instance);
// Resultado esperado con la solución: el HashSet compara por referencia y cuenta las instancias correctamente.
//
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 09
// SECTION: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Application/Interfaces.cs
// ------------------------------------------------------------------------
// List<OrdenFabricacion> ObtenerConDosColeccionesAsNoTracking();
// List<OrdenFabricacion> ObtenerConDosColeccionesConResolucionIdentidad();
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 10
// SECTION: Paso 2: Implementar los métodos en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar los métodos en OrdenRepositorio:
// TARGET_EQUIVALENTE: src/AceriaData.Infrastructure/Repositories/Rendimiento43.cs
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerConDosColeccionesAsNoTracking()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerConDosColeccionesConResolucionIdentidad()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTrackingWithIdentityResolution()
//         .Include(o => o.Planchas)
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// TARGET_EQUIVALENTE: src/AceriaData.Application/IdentityResolutionUseCase.cs
// ------------------------------------------------------------------------
// private void DemostrarDosColecciones()
// {
//     Console.WriteLine("\n--- Dos colecciones: AsNoTracking ---");
//     var ordenes1 = _unidad.Ordenes.ObtenerConDosColeccionesAsNoTracking();
//     var aleaciones1 = new HashSet<Aleacion>(ReferenceEqualityComparer.Instance);
//     foreach (var orden in ordenes1)
//     {
//         foreach (var oa in orden.OrdenesAleaciones)
//         {
//             aleaciones1.Add(oa.Aleacion);
//         }
//     }
//     Console.WriteLine($"Instancias de aleaciones: {aleaciones1.Count}");
//
//     Console.WriteLine("\n--- Dos colecciones: AsNoTrackingWithIdentityResolution ---");
//     var ordenes2 = _unidad.Ordenes.ObtenerConDosColeccionesConResolucionIdentidad();
//     var aleaciones2 = new HashSet<Aleacion>(ReferenceEqualityComparer.Instance);
//     foreach (var orden in ordenes2)
//     {
//         foreach (var oa in orden.OrdenesAleaciones)
//         {
//             aleaciones2.Add(oa.Aleacion);
//         }
//     }
//     Console.WriteLine($"Instancias de aleaciones: {aleaciones2.Count}");
// }
// ========================================================================

// CANONICAL PDF M04 4.3 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// TARGET_EQUIVALENTE: src/AceriaData.Application/IdentityResolutionUseCase.cs
// ------------------------------------------------------------------------
// DemostrarDosColecciones();
// ========================================================================

