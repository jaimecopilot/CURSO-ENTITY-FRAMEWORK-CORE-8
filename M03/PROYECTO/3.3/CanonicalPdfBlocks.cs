// ============================================================================
// M03 3.3 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.3 BLOCK 1 =====
// Paso: Paso 2: Crear el DTO OrdenResumenDto
// Sección: Paso 2: Crear el DTO OrdenResumenDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenResumenDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenResumenDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public DateTime FechaCreacion { get; set; }
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 2 =====
// Paso: Paso 3: Crear el DTO OrdenConTotalesDto
// Sección: Paso 3: Crear el DTO OrdenConTotalesDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenConTotalesDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConTotalesDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public int TotalPlanchas { get; set; }
//     public decimal PesoTotal { get; set; }
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 3 =====
// Paso: Paso 4: Añadir los métodos de proyección a la interfaz del repositorio
// Sección: Paso 4: Añadir los métodos de proyección a la interfaz del repositorio
// Ruta indicada por la práctica: src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs
// using AceriaData.Application.Dtos;
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
//     List<OrdenFabricacion> ObtenerPorClienteYRangoDeFechas(string cliente, DateTime desde, DateTime hasta);
// 
//     List<string> ObtenerClientesUnicos();
//     List<OrdenResumenDto> ObtenerResumenes();
//     List<OrdenResumenDto> ObtenerResumenesPorEstado(string estado);
//     List<OrdenConTotalesDto> ObtenerOrdenesConTotales();
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 4 =====
// Paso: Paso 5: Implementar los métodos de proyección en el repositorio
// Sección: Paso 5: Implementar los métodos de proyección en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// using AceriaData.Application.Dtos;
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
//     public List<OrdenFabricacion> ObtenerPorClienteYRangoDeFechas(string cliente, DateTime desde, DateTime hasta)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Cliente == cliente && o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
//             .OrderBy(o => o.Estado)
//             .ThenByDescending(o => o.FechaCreacion)
//             .ToList();
//     }
// 
//     public List<string> ObtenerClientesUnicos()
//     {
//         return _context.OrdenesFabricacion
//             .Select(o => o.Cliente)
//             .Distinct()
//             .OrderBy(c => c)
//             .ToList();
//     }
// 
//     public List<OrdenResumenDto> ObtenerResumenes()
//     {
//         return _context.OrdenesFabricacion
//             .OrderBy(o => o.FechaCreacion)
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .ToList();
//     }
// 
//     public List<OrdenResumenDto> ObtenerResumenesPorEstado(string estado)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Estado == estado)
//             .OrderBy(o => o.FechaCreacion)
//             .Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             })
//             .ToList();
//     }
// 
//     public List<OrdenConTotalesDto> ObtenerOrdenesConTotales()
//     {
//         return _context.OrdenesFabricacion
//             .OrderBy(o => o.NumeroOrden)
//             .Select(o => new OrdenConTotalesDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 TotalPlanchas = o.Planchas.Count(),
//                 PesoTotal = o.Planchas.Sum(p => p.Peso)
//             })
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
// ===== END CANONICAL PDF M03 3.3 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 5 =====
// Paso: Paso 6: Crear el caso de uso de proyecciones
// Sección: Paso 6: Crear el caso de uso de proyecciones
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/ProyeccionesUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class ProyeccionesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public ProyeccionesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PROYECCIONES CON SELECT ===");
// 
//         DemostrarClientesUnicos();
//         DemostrarResumenes();
//         DemostrarResumenesPorEstado();
//         DemostrarOrdenesConTotales();
//         DemostrarSqlProyeccion();
//     }
// 
//     private void DemostrarClientesUnicos()
//     {
//         Console.WriteLine("\n--- Clientes únicos ---");
// 
//         var clientes = _unidad.Ordenes.ObtenerClientesUnicos();
//         foreach (var cliente in clientes)
//         {
//             Console.WriteLine($"  {cliente}");
//         }
//     }
// 
//     private void DemostrarResumenes()
//     {
//         Console.WriteLine("\n--- Resúmenes de órdenes ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenes();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado} | {resumen.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarResumenesPorEstado()
//     {
//         Console.WriteLine("\n--- Resúmenes de órdenes pendientes ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenesPorEstado("Pendiente");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado} | {resumen.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarOrdenesConTotales()
//     {
//         Console.WriteLine("\n--- Órdenes con totales de planchas y peso ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConTotales();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | Planchas: {orden.TotalPlanchas} | Peso total: {orden.PesoTotal} kg");
//         }
//     }
// 
//     private void DemostrarSqlProyeccion()
//     {
//         Console.WriteLine("\n--- SQL generado por una proyección ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .Where(o => o.Estado == "Pendiente")
//             .OrderBy(o => o.FechaCreacion)
//             .Select(o => new
//             {
//                 o.NumeroOrden,
//                 o.Cliente,
//                 o.Estado
//             });
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 6 =====
// Paso: Paso 7: Registrar el caso de uso en el contenedor
// Sección: Paso 7: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<ProyeccionesUseCase>();
// ===== END CANONICAL PDF M03 3.3 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 7 =====
// Paso: Paso 8: Llamar al caso de uso desde la consola
// Sección: Paso 8: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ProyeccionesUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 8 =====
// Paso: Paso 9: Insertar datos de prueba con planchas
// Sección: Paso 9: Insertar datos de prueba con planchas
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
// 
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     var orden3 = new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Norte", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 3, 10) };
// 
//     context.OrdenesFabricacion.AddRange(orden1, orden2, orden3);
//     context.SaveChanges();
// 
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
// 
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 9 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 1: Crear el DTO OrdenConNumeroPlanchasDto:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConNumeroPlanchasDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public int NumeroPlanchas { get; set; }
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 10 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// List<OrdenConNumeroPlanchasDto> ObtenerOrdenesConNumeroPlanchas();
// ===== END CANONICAL PDF M03 3.3 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 11 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 3: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenConNumeroPlanchasDto> ObtenerOrdenesConNumeroPlanchas()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConNumeroPlanchasDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             NumeroPlanchas = o.Planchas.Count()
//         })
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 11 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 12 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 4: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarOrdenesConNumeroPlanchas()
// {
//     Console.WriteLine("\n--- Órdenes con número de planchas ---");
// 
//     var ordenes = _unidad.Ordenes.ObtenerOrdenesConNumeroPlanchas();
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | Planchas: {orden.NumeroPlanchas}");
//     }
// }
// ===== END CANONICAL PDF M03 3.3 BLOCK 12 =====
// ===== CANONICAL PDF M03 3.3 BLOCK 13 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 5: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarOrdenesConNumeroPlanchas();
// Paso 6: Ejecutar dotnet run y verificar que las órdenes se muestran con su número de planchas.
// ===== END CANONICAL PDF M03 3.3 BLOCK 13 =====
