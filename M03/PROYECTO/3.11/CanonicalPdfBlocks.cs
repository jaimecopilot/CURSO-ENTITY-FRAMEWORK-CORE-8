// ============================================================================
// M03 3.11 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.11 BLOCK 1 =====
// Paso: Paso 2: Crear el DTO FiltroOrdenesDto
// Sección: Paso 2: Crear el DTO FiltroOrdenesDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/FiltroOrdenesDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class FiltroOrdenesDto
// {
//     public string? Cliente { get; set; }
//     public string? Estado { get; set; }
//     public DateTime? FechaDesde { get; set; }
//     public DateTime? FechaHasta { get; set; }
//     public string? OrdenarPor { get; set; }
//     public bool Descendente { get; set; }
//     public int? Pagina { get; set; }
//     public int? TamanoPagina { get; set; }
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 2 =====
// Paso: Paso 3: Añadir el método de composición a la interfaz del repositorio
// Sección: Paso 3: Añadir el método de composición a la interfaz del repositorio
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
//     List<OrdenFabricacion> ObtenerConFiltros(FiltroOrdenesDto filtro);
//     int ContarConFiltros(FiltroOrdenesDto filtro);
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 3 =====
// Paso: Paso 4: Implementar el método de composición en el repositorio
// Sección: Paso 4: Implementar el método de composición en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public List<OrdenFabricacion> ObtenerConFiltros(FiltroOrdenesDto filtro)
// {
//     var consulta = ConstruirConsultaConFiltros(filtro);
//     return consulta.ToList();
// }
// 
// public int ContarConFiltros(FiltroOrdenesDto filtro)
// {
//     var consulta = ConstruirConsultaConFiltros(filtro);
//     return consulta.Count();
// }
// 
// private IQueryable<OrdenFabricacion> ConstruirConsultaConFiltros(FiltroOrdenesDto filtro)
// {
//     var consulta = _context.OrdenesFabricacion.AsQueryable();
// 
//     if (!string.IsNullOrEmpty(filtro.Cliente))
//     {
//         consulta = consulta.Where(o => o.Cliente == filtro.Cliente);
//     }
// 
//     if (!string.IsNullOrEmpty(filtro.Estado))
//     {
//         consulta = consulta.Where(o => o.Estado == filtro.Estado);
//     }
// 
//     if (filtro.FechaDesde.HasValue)
//     {
//         consulta = consulta.Where(o => o.FechaCreacion >= filtro.FechaDesde.Value);
//     }
// 
//     if (filtro.FechaHasta.HasValue)
//     {
//         consulta = consulta.Where(o => o.FechaCreacion <= filtro.FechaHasta.Value);
//     }
// 
//     if (!string.IsNullOrEmpty(filtro.OrdenarPor))
//     {
//         consulta = filtro.OrdenarPor switch
//         {
//             "Cliente" => filtro.Descendente ? consulta.OrderByDescending(o => o.Cliente) : consulta.OrderBy(o => o.Cliente),
//             "Estado" => filtro.Descendente ? consulta.OrderByDescending(o => o.Estado) : consulta.OrderBy(o => o.Estado),
//             "FechaCreacion" => filtro.Descendente ? consulta.OrderByDescending(o => o.FechaCreacion) : consulta.OrderBy(o => o.FechaCreacion),
//             "NumeroOrden" => filtro.Descendente ? consulta.OrderByDescending(o => o.NumeroOrden) : consulta.OrderBy(o => o.NumeroOrden),
//             _ => consulta.OrderBy(o => o.NumeroOrden)
//         };
//     }
//     else
//     {
//         consulta = consulta.OrderBy(o => o.NumeroOrden);
//     }
// 
//     if (filtro.Pagina.HasValue && filtro.TamanoPagina.HasValue &&
//         filtro.Pagina.Value > 0 && filtro.TamanoPagina.Value > 0)
//     {
//         consulta = consulta
//             .Skip((filtro.Pagina.Value - 1) * filtro.TamanoPagina.Value)
//             .Take(filtro.TamanoPagina.Value);
//     }
// 
//     return consulta;
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 4 =====
// Paso: Paso 5: Crear el caso de uso de composición de consultas
// Sección: Paso 5: Crear el caso de uso de composición de consultas
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/ComposicionConsultasUseCase.cs
// using AceriaData.Application.Dtos;
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class ComposicionConsultasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public ComposicionConsultasUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== COMPOSICIÓN DE CONSULTAS ===");
// 
//         DemostrarFiltroPorCliente();
//         DemostrarFiltroPorEstado();
//         DemostrarFiltroPorRangoDeFechas();
//         DemostrarFiltrosCombinados();
//         DemostrarOrdenacionYPaginacion();
//     }
// 
//     private void DemostrarFiltroPorCliente()
//     {
//         Console.WriteLine("\n--- Filtro por cliente ---");
// 
//         var filtro = new FiltroOrdenesDto { Cliente = "Constructora del Norte" };
//         var ordenes = _unidad.Ordenes.ObtenerConFiltros(filtro);
// 
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//         }
//     }
// 
//     private void DemostrarFiltroPorEstado()
//     {
//         Console.WriteLine("\n--- Filtro por estado ---");
// 
//         var filtro = new FiltroOrdenesDto { Estado = "Pendiente" };
//         var ordenes = _unidad.Ordenes.ObtenerConFiltros(filtro);
// 
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//         }
//     }
// 
//     private void DemostrarFiltroPorRangoDeFechas()
//     {
//         Console.WriteLine("\n--- Filtro por rango de fechas ---");
// 
//         var filtro = new FiltroOrdenesDto
//         {
//             FechaDesde = new DateTime(2024, 1, 1),
//             FechaHasta = new DateTime(2024, 6, 30)
//         };
//         var ordenes = _unidad.Ordenes.ObtenerConFiltros(filtro);
// 
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarFiltrosCombinados()
//     {
//         Console.WriteLine("\n--- Filtros combinados ---");
// 
//         var filtro = new FiltroOrdenesDto
//         {
//             Cliente = "Constructora del Norte",
//             Estado = "Pendiente",
//             FechaDesde = new DateTime(2024, 1, 1)
//         };
//         var ordenes = _unidad.Ordenes.ObtenerConFiltros(filtro);
// 
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarOrdenacionYPaginacion()
//     {
//         Console.WriteLine("\n--- Ordenación y paginación ---");
// 
//         var filtro = new FiltroOrdenesDto
//         {
//             OrdenarPor = "FechaCreacion",
//             Descendente = true,
//             Pagina = 1,
//             TamanoPagina = 2
//         };
//         var ordenes = _unidad.Ordenes.ObtenerConFiltros(filtro);
//         var total = _unidad.Ordenes.ContarConFiltros(filtro);
// 
//         Console.WriteLine($"Total de órdenes: {total} | Página 1 con 2 elementos: {ordenes.Count}");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 5 =====
// Paso: Paso 6: Registrar el caso de uso en el contenedor
// Sección: Paso 6: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<ComposicionConsultasUseCase>();
// ===== END CANONICAL PDF M03 3.11 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 6 =====
// Paso: Paso 7: Llamar al caso de uso desde la consola
// Sección: Paso 7: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ComposicionConsultasUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 7 =====
// Paso: Paso 8: Insertar datos de prueba con varias órdenes
// Sección: Paso 8: Insertar datos de prueba con varias órdenes
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
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0005", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 5, 12) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0006", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 6, 18) }
//     };
// 
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 8 =====
// Paso: Paso 10: Analizar la salida
// Sección: Paso 10: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenFabricacion> ObtenerConFiltros(FiltroOrdenesDto filtro)
// {
//     var consulta = ConstruirConsultaConFiltros(filtro);
//     return consulta.ToList();
// }
// Resultado esperado con la solución: la consulta se construye de forma incremental y se materializa solo al final. Los filtros se aplican en SQL.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// Materialización prematura	Se llamó a ToList antes de aplicar todos los filtros	Materializar solo al final
// Filtros en memoria	Se materializó la consulta antes de filtrar	Construir la consulta con IQueryable
// Múltiples consultas	Se materializó varias veces	Materializar una sola vez al final
// ThenBy sin OrderBy	Se llamó a ThenBy sin OrderBy previo	Llamar a OrderBy primero
// Skip sin OrderBy	Se aplicó Skip sin ordenar	Aplicar OrderBy antes de Skip
// Paginación incorrecta	Se calculó mal el Skip	Usar (pagina - 1) * tamanoPagina
// Reto resuelto: Consulta compuesta con filtros dinámicos
// Reto: Crear un método en el repositorio que construya una consulta con filtros dinámicos basados en una lista de predicados. Añadir el método a la interfaz, la implementación y una demostración en el caso de uso.
// 
// Solución paso a paso:
// 
// Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// 
// csharp
// List<OrdenFabricacion> ObtenerConPredicados(List<System.Linq.Expressions.Expression<Func<OrdenFabricacion, bool>>> predicados);
// ===== END CANONICAL PDF M03 3.11 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 9 =====
// Paso: Paso 2: Implementar el método en OrdenRepositorio:
// Sección: Paso 2: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenFabricacion> ObtenerConPredicados(List<System.Linq.Expressions.Expression<Func<OrdenFabricacion, bool>>> predicados)
// {
//     var consulta = _context.OrdenesFabricacion.AsQueryable();
// 
//     foreach (var predicado in predicados)
//     {
//         consulta = consulta.Where(predicado);
//     }
// 
//     return consulta.OrderBy(o => o.NumeroOrden).ToList();
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 10 =====
// Paso: Paso 3: Añadir la demostración en el caso de uso:
// Sección: Paso 3: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarPredicadosDinamicos()
// {
//     Console.WriteLine("\n--- Predicados dinámicos ---");
// 
//     var predicados = new List<System.Linq.Expressions.Expression<Func<AceriaData.Domain.Entities.OrdenFabricacion, bool>>>
//     {
//         o => o.Estado == "Pendiente",
//         o => o.Cliente == "Constructora del Norte"
//     };
// 
//     var ordenes = _unidad.Ordenes.ObtenerConPredicados(predicados);
//     Console.WriteLine($"Órdenes: {ordenes.Count}");
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//     }
// }
// ===== END CANONICAL PDF M03 3.11 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.11 BLOCK 11 =====
// Paso: Paso 4: Llamar al método desde Ejecutar:
// Sección: Paso 4: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarPredicadosDinamicos();
// Paso 5: Ejecutar dotnet run y verificar que las órdenes se muestran filtradas por los predicados dinámicos.
// ===== END CANONICAL PDF M03 3.11 BLOCK 11 =====
