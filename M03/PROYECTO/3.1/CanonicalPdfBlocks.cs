// ============================================================================
// M03 3.1 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.1 BLOCK 1 =====
// Paso: Paso 2: Añadir un caso de uso para consultas LINQ
// Sección: Paso 2: Añadir un caso de uso para consultas LINQ
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/ConsultasLinqUseCase.cs
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class ConsultasLinqUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public ConsultasLinqUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");
// 
//         DemostrarIEnumerableVsIQueryable();
//         DemostrarEjecucionDiferida();
//         DemostrarMaterializacion();
//         DemostrarToQueryString();
//     }
// 
//     private void DemostrarIEnumerableVsIQueryable()
//     {
//         Console.WriteLine("\n--- IEnumerable vs IQueryable ---");
// 
//         // IEnumerable: carga todo en memoria y filtra en C#
//         var ordenesEnumerable = _unidad.Ordenes.ObtenerTodas()
//             .Where(o => o.Cliente == "Constructora del Norte")
//             .ToList();
// 
//         // IQueryable: filtra en SQL
//         var ordenesQueryable = _unidad.Ordenes.ObtenerQueryable()
//             .Where(o => o.Cliente == "Constructora del Norte")
//             .ToList();
// 
//         Console.WriteLine($"Enumerable: {ordenesEnumerable.Count} órdenes");
//         Console.WriteLine($"Queryable: {ordenesQueryable.Count} órdenes");
//     }
// 
//     private void DemostrarEjecucionDiferida()
//     {
//         Console.WriteLine("\n--- Ejecución diferida ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable();
// 
//         Console.WriteLine("Consulta construida, sin ejecutar.");
// 
//         var cliente = "Constructora del Norte";
//         if (!string.IsNullOrEmpty(cliente))
//         {
//             consulta = consulta.Where(o => o.Cliente == cliente);
//         }
// 
//         Console.WriteLine("Filtro añadido, sin ejecutar.");
// 
//         var resultados = consulta.ToList();
// 
//         Console.WriteLine($"Consulta materializada: {resultados.Count} órdenes");
//     }
// 
//     private void DemostrarMaterializacion()
//     {
//         Console.WriteLine("\n--- Materialización ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable();
// 
//         var lista = consulta.ToList();
//         Console.WriteLine($"ToList: {lista.Count} órdenes");
// 
//         var primera = consulta.FirstOrDefault();
//         Console.WriteLine($"FirstOrDefault: {primera?.NumeroOrden}");
// 
//         var total = consulta.Count();
//         Console.WriteLine($"Count: {total} órdenes");
// 
//         var existe = consulta.Any();
//         Console.WriteLine($"Any: {existe}");
//     }
// 
//     private void DemostrarToQueryString()
//     {
//         Console.WriteLine("\n--- ToQueryString ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .Where(o => o.Cliente == "Constructora del Norte")
//             .OrderBy(o => o.FechaCreacion);
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.1 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 2 =====
// Paso: Paso 3: Añadir el método ObtenerQueryable a la interfaz del repositorio
// Sección: Paso 3: Añadir el método ObtenerQueryable a la interfaz del repositorio
// Ruta indicada por la práctica: src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs
// using AceriaData.Domain.Entities;
// 
// namespace AceriaData.Application.Interfaces;
// 
// public interface IOrdenRepositorio
// {
//     OrdenFabricacion? ObtenerPorId(int id);
//     List<OrdenFabricacion> ObtenerTodas();
//     IQueryable<OrdenFabricacion> ObtenerQueryable();
//     OrdenFabricacion? ObtenerPorNumeroOrden(string numeroOrden);
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.1 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 3 =====
// Paso: Paso 4: Implementar ObtenerQueryable en el repositorio
// Sección: Paso 4: Implementar ObtenerQueryable en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public IQueryable<OrdenFabricacion> ObtenerQueryable()
// {
//     return _context.OrdenesFabricacion.AsQueryable();
// }
// ===== END CANONICAL PDF M03 3.1 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 4 =====
// Paso: Paso 5: Registrar el caso de uso en el contenedor
// Sección: Paso 5: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<ConsultasLinqUseCase>();
// ===== END CANONICAL PDF M03 3.1 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 5 =====
// Paso: Paso 6: Llamar al caso de uso desde la consola
// Sección: Paso 6: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ConsultasLinqUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.1 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 6 =====
// Paso: Paso 7: Insertar datos de prueba
// Sección: Paso 7: Insertar datos de prueba
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
// 
//     var ordenes = new List<OrdenFabricacion>
//     {
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = DateTime.Now },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = DateTime.Now },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Norte", Estado = "EnProceso", FechaCreacion = DateTime.Now },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0004", Cliente = "Constructora del Este", Estado = "Pendiente", FechaCreacion = DateTime.Now }
//     };
// 
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.1 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 7 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 1: Añadir el método AnalizarSql al caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void AnalizarSql()
// {
//     Console.WriteLine("\n--- Análisis del SQL generado ---");
// 
//     var consulta = _unidad.Ordenes.ObtenerQueryable()
//         .Where(o => o.Cliente == "Constructora del Norte" && o.Estado == "Pendiente")
//         .OrderByDescending(o => o.FechaCreacion)
//         .Select(o => new { o.NumeroOrden, o.Cliente, o.Estado, o.FechaCreacion });
// 
//     var sql = consulta.ToQueryString();
//     Console.WriteLine(sql);
// }
// ===== END CANONICAL PDF M03 3.1 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.1 BLOCK 8 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 2: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// AnalizarSql();
// Paso 3: Ejecutar dotnet run y analizar el SQL generado.
// ===== END CANONICAL PDF M03 3.1 BLOCK 8 =====
