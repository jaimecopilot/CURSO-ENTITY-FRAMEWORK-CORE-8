// ============================================================================
// M03 3.8 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.8 BLOCK 1 =====
// Paso: Paso 2: Añadir los métodos de carga Eager a la interfaz del repositorio
// Sección: Paso 2: Añadir los métodos de carga Eager a la interfaz del repositorio
// Ruta indicada por la práctica: src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
// 
// namespace AceriaData.Application.Interfaces;
// 
// public interface IOrdenRepositorio
// {
//     // ... métodos existentes ...
// 
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasInclude();
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleInclude();
//     List<OrdenFabricacion> ObtenerOrdenesConAleacionesInclude();
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasActivas();
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleSplitQuery();
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 2 =====
// Paso: Paso 3: Implementar los métodos de carga Eager en el repositorio
// Sección: Paso 3: Implementar los métodos de carga Eager en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasInclude()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// 
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleInclude()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// 
// public List<OrdenFabricacion> ObtenerOrdenesConAleacionesInclude()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// 
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasActivas()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas.Where(p => p.Activa))
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// 
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleSplitQuery()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 3 =====
// Paso: Paso 4: Crear el caso de uso de carga Eager
// Sección: Paso 4: Crear el caso de uso de carga Eager
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/CargaEagerUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class CargaEagerUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public CargaEagerUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CARGA EAGER CON INCLUDE ===");
// 
//         DemostrarIncludePlanchas();
//         DemostrarIncludePlanchasYDetalle();
//         DemostrarThenIncludeAleaciones();
//         DemostrarFilteredInclude();
//         DemostrarSplitQuery();
//         DemostrarSqlInclude();
//     }
// 
//     private void DemostrarIncludePlanchas()
//     {
//         Console.WriteLine("\n--- Include: órdenes con planchas ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | Planchas: {orden.Planchas.Count}");
//         }
//     }
// 
//     private void DemostrarIncludePlanchasYDetalle()
//     {
//         Console.WriteLine("\n--- Include: órdenes con planchas y detalle ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleInclude();
//         foreach (var orden in ordenes)
//         {
//             var detalle = orden.Detalle == null ? "Sin detalle" : orden.Detalle.ComposicionQuimica;
//             Console.WriteLine($"  {orden.NumeroOrden} | Planchas: {orden.Planchas.Count} | Detalle: {detalle}");
//         }
//     }
// 
//     private void DemostrarThenIncludeAleaciones()
//     {
//         Console.WriteLine("\n--- ThenInclude: órdenes con aleaciones ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConAleacionesInclude();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | Aleaciones: {orden.OrdenesAleaciones.Count}");
//             foreach (var ordenAleacion in orden.OrdenesAleaciones)
//             {
//                 Console.WriteLine($"    {ordenAleacion.Aleacion.Nombre} | Cantidad: {ordenAleacion.CantidadUtilizada} kg");
//             }
//         }
//     }
// 
//     private void DemostrarFilteredInclude()
//     {
//         Console.WriteLine("\n--- Filtered Include: órdenes con planchas activas ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasActivas();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | Planchas activas: {orden.Planchas.Count}");
//         }
//     }
// 
//     private void DemostrarSplitQuery()
//     {
//         Console.WriteLine("\n--- AsSplitQuery: órdenes con planchas y detalle ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSplitQuery();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | Planchas: {orden.Planchas.Count}");
//         }
//     }
// 
//     private void DemostrarSqlInclude()
//     {
//         Console.WriteLine("\n--- SQL generado por un Include ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .Include(o => o.Planchas)
//             .Include(o => o.Detalle);
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 4 =====
// Paso: Paso 5: Registrar el caso de uso en el contenedor
// Sección: Paso 5: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<CargaEagerUseCase>();
// ===== END CANONICAL PDF M03 3.8 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 5 =====
// Paso: Paso 6: Llamar al caso de uso desde la consola
// Sección: Paso 6: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<CargaEagerUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 6 =====
// Paso: Paso 7: Insertar datos de prueba con planchas, detalle y aleaciones
// Sección: Paso 7: Insertar datos de prueba con planchas, detalle y aleaciones
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
// 
//     var aleacion1 = new Aleacion { Nombre = "AISI 1045", Codigo = "A1045", PorcentajeCarbono = 0.45m, PorcentajeManganeso = 0.75m, Activo = true };
//     var aleacion2 = new Aleacion { Nombre = "AISI 4140", Codigo = "A4140", PorcentajeCarbono = 0.40m, PorcentajeManganeso = 0.85m, Activo = true };
//     context.Aleaciones.AddRange(aleacion1, aleacion2);
//     context.SaveChanges();
// 
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2);
//     context.SaveChanges();
// 
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = false };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
// 
//     var detalle1 = new DetalleOrden { OrdenId = orden1.Id, ComposicionQuimica = "C: 0.45%, Mn: 0.75%", TemperaturaColada = 1550.5 };
//     context.DetallesOrden.Add(detalle1);
// 
//     var ordenAleacion1 = new OrdenAleacion { OrdenFabricacionId = orden1.Id, AleacionId = aleacion1.Id, CantidadUtilizada = 1500.5m, EstadoRelacion = "Activa" };
//     var ordenAleacion2 = new OrdenAleacion { OrdenFabricacionId = orden1.Id, AleacionId = aleacion2.Id, CantidadUtilizada = 800.0m, EstadoRelacion = "Activa" };
//     var ordenAleacion3 = new OrdenAleacion { OrdenFabricacionId = orden2.Id, AleacionId = aleacion1.Id, CantidadUtilizada = 1200.0m, EstadoRelacion = "Activa" };
//     context.OrdenesAleaciones.AddRange(ordenAleacion1, ordenAleacion2, ordenAleacion3);
// 
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 7 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 9: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasInclude()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// Resultado esperado con la solución: el código ejecuta una sola consulta con un LEFT JOIN para cargar las órdenes y sus planchas. No hay problema N+1.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// Producto cartesiano	Se incluyeron varias colecciones sin AsSplitQuery	Usar AsSplitQuery
// N+1	Se llamó a ToList antes del Include	Aplicar Include antes de ToList
// ThenInclude sin Include	Se usó ThenInclude sin Include previo	Usar Include antes de ThenInclude
// NullReferenceException en referencia	La propiedad de navegación es null	Comprobar si la propiedad es null
// Filtro no aplicado	Se aplicó el filtro fuera del Include	Aplicar el filtro dentro del Include
// AutoInclude excesivo	Se configuró en muchas propiedades	Usar con moderación
// Reto resuelto: Consulta con Include, ThenInclude y Filtered Include
// Reto: Crear un método en el repositorio que cargue las órdenes con sus planchas activas y sus aleaciones, usando Include con filtro y ThenInclude. Añadir el método a la interfaz, la implementación y una demostración en el caso de uso.
// 
// Solución paso a paso:
// 
// Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// 
// csharp
// List<OrdenFabricacion> ObtenerOrdenesConPlanchasActivasYAleaciones();
// ===== END CANONICAL PDF M03 3.8 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 8 =====
// Paso: Paso 2: Implementar el método en OrdenRepositorio:
// Sección: Paso 2: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenFabricacion> ObtenerOrdenesConPlanchasActivasYAleaciones()
// {
//     return _context.OrdenesFabricacion
//         .Include(o => o.Planchas.Where(p => p.Activa))
//         .Include(o => o.OrdenesAleaciones)
//         .ThenInclude(oa => oa.Aleacion)
//         .AsSplitQuery()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 9 =====
// Paso: Paso 3: Añadir la demostración en el caso de uso:
// Sección: Paso 3: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarPlanchasActivasYAleaciones()
// {
//     Console.WriteLine("\n--- Planchas activas y aleaciones ---");
// 
//     var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasActivasYAleaciones();
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden} | Planchas activas: {orden.Planchas.Count} | Aleaciones: {orden.OrdenesAleaciones.Count}");
//     }
// }
// ===== END CANONICAL PDF M03 3.8 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.8 BLOCK 10 =====
// Paso: Paso 4: Llamar al método desde Ejecutar:
// Sección: Paso 4: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarPlanchasActivasYAleaciones();
// Paso 5: Ejecutar dotnet run y verificar que las órdenes se muestran con sus planchas activas y sus aleaciones.
// ===== END CANONICAL PDF M03 3.8 BLOCK 10 =====
