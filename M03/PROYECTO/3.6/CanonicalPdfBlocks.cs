// ============================================================================
// M03 3.6 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.6 BLOCK 1 =====
// Paso: Paso 2: Crear el DTO ResumenPorClienteConOrdenesDto
// Sección: Paso 2: Crear el DTO ResumenPorClienteConOrdenesDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/ResumenPorClienteConOrdenesDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class ResumenPorClienteConOrdenesDto
// {
//     public string Cliente { get; set; } = string.Empty;
//     public int TotalOrdenes { get; set; }
//     public List<OrdenResumenDto> Ordenes { get; set; } = new();
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 2 =====
// Paso: Paso 3: Crear el DTO ResumenPorClienteYEstadoDto
// Sección: Paso 3: Crear el DTO ResumenPorClienteYEstadoDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/ResumenPorClienteYEstadoDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class ResumenPorClienteYEstadoDto
// {
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public int TotalOrdenes { get; set; }
//     public decimal PesoTotal { get; set; }
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 3 =====
// Paso: Paso 4: Añadir los métodos de agrupación a la interfaz del repositorio
// Sección: Paso 4: Añadir los métodos de agrupación a la interfaz del repositorio
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
//     List<OrdenConPlanchasDto> ObtenerOrdenesConPlanchas();
//     List<OrdenConDetalleDto> ObtenerOrdenesConDetalle();
//     List<OrdenConTotalesDto> ObtenerOrdenesConTotalesDetallados();
//     List<OrdenCompletaDto> ObtenerOrdenesCompletas();
// 
//     int ContarOrdenes();
//     int ContarOrdenesPorEstado(string estado);
//     bool ExisteAlgunaOrden();
//     bool TodasLasOrdenesPendientes();
//     decimal ObtenerPesoTotalDePlanchas();
//     double ObtenerPesoPromedioDePlanchas();
//     decimal ObtenerPesoMinimoDePlanchas();
//     decimal ObtenerPesoMaximoDePlanchas();
//     DateTime ObtenerFechaMasAntigua();
//     DateTime ObtenerFechaMasReciente();
// 
//     List<ResumenPorClienteDto> ObtenerResumenPorCliente();
//     List<ResumenPorEstadoDto> ObtenerResumenPorEstado();
//     List<ResumenPorClienteDto> ObtenerClientesConMasDeUnaOrden();
//     List<ResumenMensualDto> ObtenerResumenMensual();
// 
//     List<ResumenPorClienteConOrdenesDto> ObtenerResumenPorClienteConOrdenes();
//     List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstado();
//     List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstadoConFiltro();
//     List<ResumenPorClienteDto> ObtenerResumenPorClienteConMultiplesAgregaciones();
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 4 =====
// Paso: Paso 5: Implementar los métodos de agrupación en el repositorio
// Sección: Paso 5: Implementar los métodos de agrupación en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public List<ResumenPorClienteConOrdenesDto> ObtenerResumenPorClienteConOrdenes()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => o.Cliente)
//         .Select(g => new ResumenPorClienteConOrdenesDto
//         {
//             Cliente = g.Key,
//             TotalOrdenes = g.Count(),
//             Ordenes = g.Select(o => new OrdenResumenDto
//             {
//                 NumeroOrden = o.NumeroOrden,
//                 Cliente = o.Cliente,
//                 Estado = o.Estado,
//                 FechaCreacion = o.FechaCreacion
//             }).ToList()
//         })
//         .OrderBy(r => r.Cliente)
//         .ToList();
// }
// 
// public List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstado()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => new { o.Cliente, o.Estado })
//         .Select(g => new ResumenPorClienteYEstadoDto
//         {
//             Cliente = g.Key.Cliente,
//             Estado = g.Key.Estado,
//             TotalOrdenes = g.Count(),
//             PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso))
//         })
//         .OrderBy(r => r.Cliente)
//         .ThenBy(r => r.Estado)
//         .ToList();
// }
// 
// public List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstadoConFiltro()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => new { o.Cliente, o.Estado })
//         .Where(g => g.Count() > 1)
//         .Select(g => new ResumenPorClienteYEstadoDto
//         {
//             Cliente = g.Key.Cliente,
//             Estado = g.Key.Estado,
//             TotalOrdenes = g.Count(),
//             PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso))
//         })
//         .OrderBy(r => r.Cliente)
//         .ThenBy(r => r.Estado)
//         .ToList();
// }
// 
// public List<ResumenPorClienteDto> ObtenerResumenPorClienteConMultiplesAgregaciones()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => o.Cliente)
//         .Select(g => new ResumenPorClienteDto
//         {
//             Cliente = g.Key,
//             TotalOrdenes = g.Count(),
//             TotalPlanchas = g.Sum(o => o.Planchas.Count()),
//             PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso)),
//             FechaMasReciente = g.Max(o => o.FechaCreacion)
//         })
//         .OrderBy(r => r.Cliente)
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 5 =====
// Paso: Paso 6: Crear el caso de uso de agrupaciones con proyección
// Sección: Paso 6: Crear el caso de uso de agrupaciones con proyección
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/AgrupacionesUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class AgrupacionesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public AgrupacionesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== AGRUPACIONES CON PROYECCIÓN ===");
// 
//         DemostrarResumenPorClienteConOrdenes();
//         DemostrarResumenPorClienteYEstado();
//         DemostrarResumenPorClienteYEstadoConFiltro();
//         DemostrarResumenConMultiplesAgregaciones();
//         DemostrarSqlAgrupacionMultiple();
//     }
// 
//     private void DemostrarResumenPorClienteConOrdenes()
//     {
//         Console.WriteLine("\n--- Resumen por cliente con órdenes ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenPorClienteConOrdenes();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Cliente} | Total: {resumen.TotalOrdenes}");
//             foreach (var orden in resumen.Ordenes)
//             {
//                 Console.WriteLine($"    {orden.NumeroOrden} | {orden.Estado} | {orden.FechaCreacion:dd/MM/yyyy}");
//             }
//         }
//     }
// 
//     private void DemostrarResumenPorClienteYEstado()
//     {
//         Console.WriteLine("\n--- Resumen por cliente y estado ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenPorClienteYEstado();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Cliente} | {resumen.Estado} | Órdenes: {resumen.TotalOrdenes} | Peso: {resumen.PesoTotal} kg");
//         }
//     }
// 
//     private void DemostrarResumenPorClienteYEstadoConFiltro()
//     {
//         Console.WriteLine("\n--- Resumen por cliente y estado (con más de una orden) ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Cliente} | {resumen.Estado} | Órdenes: {resumen.TotalOrdenes} | Peso: {resumen.PesoTotal} kg");
//         }
//     }
// 
//     private void DemostrarResumenConMultiplesAgregaciones()
//     {
//         Console.WriteLine("\n--- Resumen por cliente con múltiples agregaciones ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenPorClienteConMultiplesAgregaciones();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Cliente} | Órdenes: {resumen.TotalOrdenes} | Planchas: {resumen.TotalPlanchas} | Peso: {resumen.PesoTotal} kg | Última: {resumen.FechaMasReciente:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarSqlAgrupacionMultiple()
//     {
//         Console.WriteLine("\n--- SQL generado por una agrupación por múltiples claves ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .GroupBy(o => new { o.Cliente, o.Estado })
//             .Select(g => new
//             {
//                 Cliente = g.Key.Cliente,
//                 Estado = g.Key.Estado,
//                 Total = g.Count(),
//                 PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso))
//             });
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 6 =====
// Paso: Paso 7: Registrar el caso de uso en el contenedor
// Sección: Paso 7: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<AgrupacionesUseCase>();
// ===== END CANONICAL PDF M03 3.6 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 7 =====
// Paso: Paso 8: Llamar al caso de uso desde la consola
// Sección: Paso 8: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<AgrupacionesUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 8 =====
// Paso: Paso 9: Insertar datos de prueba con varias órdenes y planchas
// Sección: Paso 9: Insertar datos de prueba con varias órdenes y planchas
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
//     var orden4 = new OrdenFabricacion { NumeroOrden = "OF-2024-0004", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 4, 5) };
// 
//     context.OrdenesFabricacion.AddRange(orden1, orden2, orden3, orden4);
//     context.SaveChanges();
// 
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
//     var plancha4 = new PlanchaAcero { OrdenId = orden3.Id, Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m, Activa = true };
// 
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3, plancha4);
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 9 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 11: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// Ordenes = g.Select(o => new OrdenResumenDto
// {
//     NumeroOrden = o.NumeroOrden,
//     Cliente = o.Cliente,
//     Estado = o.Estado,
//     FechaCreacion = o.FechaCreacion
// }).ToList()
// Resultado esperado con la solución: la consulta compila y la colección se materializa correctamente.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// Colección interna sin ToList	Se olvidó materializar la colección	Añadir .ToList() dentro de la proyección
// Where antes del GroupBy	Se aplica el filtro a las filas	Usar Where después del GroupBy para HAVING
// Propiedad no agrupada	Se proyecta una propiedad que no está en el GroupBy	Usar la clave del grupo o funciones de agregación
// N+1 en agrupaciones	Se proyecta una colección interna	Revisar el SQL generado y considerar alternativas
// SelectMany sin proyección	Se aplana sin proyectar	Añadir una proyección clara
// Agrupación anidada	EF Core ejecuta múltiples consultas	Cargar los datos en una sola consulta
// Reto resuelto: Agrupación por mes con proyección de órdenes
// Reto: Crear un método en el repositorio que agrupe las órdenes por mes y año, y proyecte cada grupo a un DTO con el año, el mes, el total de órdenes y la lista de números de orden. Añadir el DTO, el método a la interfaz, la implementación y una demostración en el caso de uso.
// 
// Solución paso a paso:
// 
// Paso 1: Crear el DTO ResumenMensualConOrdenesDto:
// 
// csharp
// namespace AceriaData.Application.Dtos;
// 
// public class ResumenMensualConOrdenesDto
// {
//     public int Anio { get; set; }
//     public int Mes { get; set; }
//     public int TotalOrdenes { get; set; }
//     public List<string> NumerosOrden { get; set; } = new();
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 10 =====
// Paso: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Sección: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// List<ResumenMensualConOrdenesDto> ObtenerResumenMensualConOrdenes();
// ===== END CANONICAL PDF M03 3.6 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 11 =====
// Paso: Paso 3: Implementar el método en OrdenRepositorio:
// Sección: Paso 3: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<ResumenMensualConOrdenesDto> ObtenerResumenMensualConOrdenes()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })
//         .Select(g => new ResumenMensualConOrdenesDto
//         {
//             Anio = g.Key.Year,
//             Mes = g.Key.Month,
//             TotalOrdenes = g.Count(),
//             NumerosOrden = g.Select(o => o.NumeroOrden).ToList()
//         })
//         .OrderBy(r => r.Anio)
//         .ThenBy(r => r.Mes)
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 11 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 12 =====
// Paso: Paso 4: Añadir la demostración en el caso de uso:
// Sección: Paso 4: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarResumenMensualConOrdenes()
// {
//     Console.WriteLine("\n--- Resumen mensual con órdenes ---");
// 
//     var resumenes = _unidad.Ordenes.ObtenerResumenMensualConOrdenes();
//     foreach (var resumen in resumenes)
//     {
//         Console.WriteLine($"  {resumen.Anio}-{resumen.Mes:D2} | Total: {resumen.TotalOrdenes}");
//         foreach (var numero in resumen.NumerosOrden)
//         {
//             Console.WriteLine($"    {numero}");
//         }
//     }
// }
// ===== END CANONICAL PDF M03 3.6 BLOCK 12 =====
// ===== CANONICAL PDF M03 3.6 BLOCK 13 =====
// Paso: Paso 5: Llamar al método desde Ejecutar:
// Sección: Paso 5: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarResumenMensualConOrdenes();
// Paso 6: Ejecutar dotnet run y verificar que las órdenes se muestran agrupadas por mes y año con la lista de números.
// ===== END CANONICAL PDF M03 3.6 BLOCK 13 =====
