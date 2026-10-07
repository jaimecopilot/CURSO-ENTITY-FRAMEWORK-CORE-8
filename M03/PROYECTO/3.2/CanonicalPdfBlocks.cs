// ============================================================================
// M03 3.2 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.2 BLOCK 1 =====
// Paso: Paso 2: Añadir los métodos de consulta a la interfaz del repositorio
// Sección: Paso 2: Añadir los métodos de consulta a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerPendientesPorCliente(string cliente);
//     List<OrdenFabricacion> ObtenerPorEstadoOrdenadasPorFecha(string estado);
//     List<OrdenFabricacion> ObtenerPorRangoDeFechas(DateTime desde, DateTime hasta);
//     List<OrdenFabricacion> ObtenerPorClienteOrdenadas(string cliente);
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 2 =====
// Paso: Paso 3: Implementar los métodos en el repositorio
// Sección: Paso 3: Implementar los métodos en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// 
// namespace AceriaData.Infrastructure.Repositories;
// 
// public class OrdenRepositorio : IOrdenRepositorio
// {
//     private readonly AceriaDbContext _context;
// 
//     public OrdenRepositorio(AceriaDbContext context)
//     {
//         _context = context;
//     }
// 
//     public OrdenFabricacion? ObtenerPorId(int id)
//     {
//         return _context.OrdenesFabricacion.Find(id);
//     }
// 
//     public List<OrdenFabricacion> ObtenerTodas()
//     {
//         return _context.OrdenesFabricacion.ToList();
//     }
// 
//     public IQueryable<OrdenFabricacion> ObtenerQueryable()
//     {
//         return _context.OrdenesFabricacion.AsQueryable();
//     }
// 
//     public OrdenFabricacion? ObtenerPorNumeroOrden(string numeroOrden)
//     {
//         return _context.OrdenesFabricacion.FirstOrDefault(o => o.NumeroOrden == numeroOrden);
//     }
// 
//     public List<OrdenFabricacion> ObtenerPendientesPorCliente(string cliente)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Cliente == cliente && o.Estado == "Pendiente")
//             .OrderBy(o => o.FechaCreacion)
//             .ToList();
//     }
// 
//     public List<OrdenFabricacion> ObtenerPorEstadoOrdenadasPorFecha(string estado)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Estado == estado)
//             .OrderByDescending(o => o.FechaCreacion)
//             .ToList();
//     }
// 
//     public List<OrdenFabricacion> ObtenerPorRangoDeFechas(DateTime desde, DateTime hasta)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
//             .OrderBy(o => o.FechaCreacion)
//             .ToList();
//     }
// 
//     public List<OrdenFabricacion> ObtenerPorClienteOrdenadas(string cliente)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Cliente == cliente)
//             .OrderBy(o => o.Cliente)
//             .ThenByDescending(o => o.FechaCreacion)
//             .ToList();
//     }
// 
//     public void Agregar(OrdenFabricacion orden)
//     {
//         _context.OrdenesFabricacion.Add(orden);
//     }
// 
//     public void Eliminar(OrdenFabricacion orden)
//     {
//         _context.OrdenesFabricacion.Remove(orden);
//     }
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 3 =====
// Paso: Paso 4: Crear el caso de uso de consultas básicas
// Sección: Paso 4: Crear el caso de uso de consultas básicas
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/ConsultasBasicasUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class ConsultasBasicasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public ConsultasBasicasUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CONSULTAS BÁSICAS CON LINQ ===");
// 
//         DemostrarWhereSimple();
//         DemostrarWhereCompuesto();
//         DemostrarOrdenacionSimple();
//         DemostrarOrdenacionMultiple();
//         DemostrarCombinacion();
//     }
// 
//     private void DemostrarWhereSimple()
//     {
//         Console.WriteLine("\n--- Where simple: órdenes pendientes del cliente 'Constructora del Norte' ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerPendientesPorCliente("Constructora del Norte");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarWhereCompuesto()
//     {
//         Console.WriteLine("\n--- Where compuesto: órdenes en estado 'Pendiente' ordenadas por fecha descendente ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerPorEstadoOrdenadasPorFecha("Pendiente");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarOrdenacionSimple()
//     {
//         Console.WriteLine("\n--- Ordenación simple: órdenes por rango de fechas ---");
// 
//         var desde = new DateTime(2024, 1, 1);
//         var hasta = new DateTime(2025, 12, 31);
// 
//         var ordenes = _unidad.Ordenes.ObtenerPorRangoDeFechas(desde, hasta);
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarOrdenacionMultiple()
//     {
//         Console.WriteLine("\n--- Ordenación múltiple: órdenes del cliente 'Constructora del Norte' ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerPorClienteOrdenadas("Constructora del Norte");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarCombinacion()
//     {
//         Console.WriteLine("\n--- Combinación: SQL generado por una consulta con Where, OrderBy y ThenBy ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .Where(o => o.Estado == "Pendiente")
//             .OrderBy(o => o.Cliente)
//             .ThenByDescending(o => o.FechaCreacion);
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 4 =====
// Paso: Paso 5: Registrar el caso de uso en el contenedor
// Sección: Paso 5: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<ConsultasBasicasUseCase>();
// ===== END CANONICAL PDF M03 3.2 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 5 =====
// Paso: Paso 6: Llamar al caso de uso desde la consola
// Sección: Paso 6: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ConsultasBasicasUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 6 =====
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Norte", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0004", Cliente = "Constructora del Este", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 4, 5) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) }
//     };
// 
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 7 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// List<OrdenFabricacion> ObtenerPorClienteYRangoDeFechas(string cliente, DateTime desde, DateTime hasta);
// ===== END CANONICAL PDF M03 3.2 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 8 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 2: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenFabricacion> ObtenerPorClienteYRangoDeFechas(string cliente, DateTime desde, DateTime hasta)
// {
//     return _context.OrdenesFabricacion
//         .Where(o => o.Cliente == cliente && o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
//         .OrderBy(o => o.Estado)
//         .ThenByDescending(o => o.FechaCreacion)
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 9 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 3: Usar el método desde el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarConsultaCombinada()
// {
//     Console.WriteLine("\n--- Consulta combinada: cliente y rango de fechas ---");
// 
//     var desde = new DateTime(2024, 1, 1);
//     var hasta = new DateTime(2024, 12, 31);
// 
//     var ordenes = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas("Constructora del Norte", desde, hasta);
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | {orden.FechaCreacion:dd/MM/yyyy}");
//     }
// }
// ===== END CANONICAL PDF M03 3.2 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.2 BLOCK 10 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 4: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarConsultaCombinada();
// Paso 5: Ejecutar dotnet run y verificar que las órdenes se muestran ordenadas por estado y fecha descendente.
// ===== END CANONICAL PDF M03 3.2 BLOCK 10 =====
