// ============================================================================
// M03 3.5 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.5 BLOCK 1 =====
// Paso: Paso 2: Crear el DTO ResumenPorClienteDto
// Sección: Paso 2: Crear el DTO ResumenPorClienteDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/ResumenPorClienteDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class ResumenPorClienteDto
// {
//     public string Cliente { get; set; } = string.Empty;
//     public int TotalOrdenes { get; set; }
//     public int TotalPlanchas { get; set; }
//     public decimal PesoTotal { get; set; }
//     public DateTime FechaMasReciente { get; set; }
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 2 =====
// Paso: Paso 3: Crear el DTO ResumenPorEstadoDto
// Sección: Paso 3: Crear el DTO ResumenPorEstadoDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/ResumenPorEstadoDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class ResumenPorEstadoDto
// {
//     public string Estado { get; set; } = string.Empty;
//     public int TotalOrdenes { get; set; }
//     public decimal PesoTotal { get; set; }
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 3 =====
// Paso: Paso 4: Añadir los métodos de agregación a la interfaz del repositorio
// Sección: Paso 4: Añadir los métodos de agregación a la interfaz del repositorio
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
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 4 =====
// Paso: Paso 5: Implementar los métodos de agregación en el repositorio
// Sección: Paso 5: Implementar los métodos de agregación en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public int ContarOrdenes()
// {
//     return _context.OrdenesFabricacion.Count();
// }
// 
// public int ContarOrdenesPorEstado(string estado)
// {
//     return _context.OrdenesFabricacion.Count(o => o.Estado == estado);
// }
// 
// public bool ExisteAlgunaOrden()
// {
//     return _context.OrdenesFabricacion.Any();
// }
// 
// public bool TodasLasOrdenesPendientes()
// {
//     return _context.OrdenesFabricacion.All(o => o.Estado == "Pendiente");
// }
// 
// public decimal ObtenerPesoTotalDePlanchas()
// {
//     return _context.PlanchasAcero.Sum(p => p.Peso);
// }
// 
// public double ObtenerPesoPromedioDePlanchas()
// {
//     return _context.PlanchasAcero.Average(p => (double)p.Peso);
// }
// 
// public decimal ObtenerPesoMinimoDePlanchas()
// {
//     return _context.PlanchasAcero.Min(p => p.Peso);
// }
// 
// public decimal ObtenerPesoMaximoDePlanchas()
// {
//     return _context.PlanchasAcero.Max(p => p.Peso);
// }
// 
// public DateTime ObtenerFechaMasAntigua()
// {
//     return _context.OrdenesFabricacion.Min(o => o.FechaCreacion);
// }
// 
// public DateTime ObtenerFechaMasReciente()
// {
//     return _context.OrdenesFabricacion.Max(o => o.FechaCreacion);
// }
// 
// public List<ResumenPorClienteDto> ObtenerResumenPorCliente()
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
// 
// public List<ResumenPorEstadoDto> ObtenerResumenPorEstado()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => o.Estado)
//         .Select(g => new ResumenPorEstadoDto
//         {
//             Estado = g.Key,
//             TotalOrdenes = g.Count(),
//             PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso))
//         })
//         .OrderBy(r => r.Estado)
//         .ToList();
// }
// 
// public List<ResumenPorClienteDto> ObtenerClientesConMasDeUnaOrden()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => o.Cliente)
//         .Where(g => g.Count() > 1)
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
// ===== END CANONICAL PDF M03 3.5 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 5 =====
// Paso: Paso 6: Crear el caso de uso de agregaciones
// Sección: Paso 6: Crear el caso de uso de agregaciones
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/AgregacionesUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class AgregacionesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public AgregacionesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CONSULTAS DE AGREGACIÓN ===");
// 
//         DemostrarConteos();
//         DemostrarAnyAll();
//         DemostrarSumAverageMinMax();
//         DemostrarFechas();
//         DemostrarResumenPorCliente();
//         DemostrarResumenPorEstado();
//         DemostrarClientesConMasDeUnaOrden();
//         DemostrarSqlAgregacion();
//     }
// 
//     private void DemostrarConteos()
//     {
//         Console.WriteLine("\n--- Conteos ---");
// 
//         var total = _unidad.Ordenes.ContarOrdenes();
//         var pendientes = _unidad.Ordenes.ContarOrdenesPorEstado("Pendiente");
// 
//         Console.WriteLine($"Total de órdenes: {total}");
//         Console.WriteLine($"Órdenes pendientes: {pendientes}");
//     }
// 
//     private void DemostrarAnyAll()
//     {
//         Console.WriteLine("\n--- Any y All ---");
// 
//         var existeAlguna = _unidad.Ordenes.ExisteAlgunaOrden();
//         var todasPendientes = _unidad.Ordenes.TodasLasOrdenesPendientes();
// 
//         Console.WriteLine($"Existe alguna orden: {existeAlguna}");
//         Console.WriteLine($"Todas las órdenes pendientes: {todasPendientes}");
//     }
// 
//     private void DemostrarSumAverageMinMax()
//     {
//         Console.WriteLine("\n--- Sum, Average, Min y Max ---");
// 
//         var pesoTotal = _unidad.Ordenes.ObtenerPesoTotalDePlanchas();
//         var pesoPromedio = _unidad.Ordenes.ObtenerPesoPromedioDePlanchas();
//         var pesoMinimo = _unidad.Ordenes.ObtenerPesoMinimoDePlanchas();
//         var pesoMaximo = _unidad.Ordenes.ObtenerPesoMaximoDePlanchas();
// 
//         Console.WriteLine($"Peso total: {pesoTotal} kg");
//         Console.WriteLine($"Peso promedio: {pesoPromedio:F2} kg");
//         Console.WriteLine($"Peso mínimo: {pesoMinimo} kg");
//         Console.WriteLine($"Peso máximo: {pesoMaximo} kg");
//     }
// 
//     private void DemostrarFechas()
//     {
//         Console.WriteLine("\n--- Fechas ---");
// 
//         var fechaAntigua = _unidad.Ordenes.ObtenerFechaMasAntigua();
//         var fechaReciente = _unidad.Ordenes.ObtenerFechaMasReciente();
// 
//         Console.WriteLine($"Fecha más antigua: {fechaAntigua:dd/MM/yyyy}");
//         Console.WriteLine($"Fecha más reciente: {fechaReciente:dd/MM/yyyy}");
//     }
// 
//     private void DemostrarResumenPorCliente()
//     {
//         Console.WriteLine("\n--- Resumen por cliente ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenPorCliente();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Cliente} | Órdenes: {resumen.TotalOrdenes} | Planchas: {resumen.TotalPlanchas} | Peso: {resumen.PesoTotal} kg | Última: {resumen.FechaMasReciente:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarResumenPorEstado()
//     {
//         Console.WriteLine("\n--- Resumen por estado ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenPorEstado();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Estado} | Órdenes: {resumen.TotalOrdenes} | Peso: {resumen.PesoTotal} kg");
//         }
//     }
// 
//     private void DemostrarClientesConMasDeUnaOrden()
//     {
//         Console.WriteLine("\n--- Clientes con más de una orden ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerClientesConMasDeUnaOrden();
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.Cliente} | Órdenes: {resumen.TotalOrdenes}");
//         }
//     }
// 
//     private void DemostrarSqlAgregacion()
//     {
//         Console.WriteLine("\n--- SQL generado por una agregación agrupada ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .GroupBy(o => o.Cliente)
//             .Select(g => new
//             {
//                 Cliente = g.Key,
//                 Total = g.Count(),
//                 PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso))
//             });
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 6 =====
// Paso: Paso 7: Registrar el caso de uso en el contenedor
// Sección: Paso 7: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<AgregacionesUseCase>();
// ===== END CANONICAL PDF M03 3.5 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 7 =====
// Paso: Paso 8: Llamar al caso de uso desde la consola
// Sección: Paso 8: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<AgregacionesUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 8 =====
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
// ===== END CANONICAL PDF M03 3.5 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 9 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 11: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public double ObtenerPesoPromedioDePlanchas()
// {
//     return _context.PlanchasAcero
//         .Where(p => p.OrdenId == 999)
//         .Select(p => (double)p.Peso)
//         .DefaultIfEmpty(0)
//         .Average();
// }
// Resultado esperado con la solución: el promedio es cero cuando no hay planchas.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// Average sobre colección vacía	No se comprobó si hay elementos	Usar DefaultIfEmpty(0)
// Min o Max sobre colección vacía	No se comprobó si hay elementos	Usar DefaultIfEmpty o comprobar
// GroupBy sin agregación	Se proyecta una propiedad que no está en el GroupBy	Usar la clave del grupo o funciones de agregación
// Where después del GroupBy	Se usa Where en lugar de Having	Usar Where después del GroupBy para HAVING
// Count después de ToList	Se materializó la consulta antes de contar	Llamar a Count directamente sobre IQueryable
// Sum sobre tipo anulable	Devuelve null si la colección está vacía	Usar DefaultIfEmpty o COALESCE
// Any con Count() > 0	Se usa Count en lugar de Any	Usar Any para mejor rendimiento
// Reto resuelto: Resumen por rango de fechas con agregaciones
// Reto: Crear un método en el repositorio que agrupe las órdenes por mes y año, y calcule el total de órdenes, el total de planchas y el peso total por cada grupo. Añadir un DTO, el método a la interfaz, la implementación y una demostración en el caso de uso.
// 
// Solución paso a paso:
// 
// Paso 1: Crear el DTO ResumenMensualDto:
// 
// csharp
// namespace AceriaData.Application.Dtos;
// 
// public class ResumenMensualDto
// {
//     public int Anio { get; set; }
//     public int Mes { get; set; }
//     public int TotalOrdenes { get; set; }
//     public int TotalPlanchas { get; set; }
//     public decimal PesoTotal { get; set; }
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 10 =====
// Paso: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Sección: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// List<ResumenMensualDto> ObtenerResumenMensual();
// ===== END CANONICAL PDF M03 3.5 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 11 =====
// Paso: Paso 3: Implementar el método en OrdenRepositorio:
// Sección: Paso 3: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<ResumenMensualDto> ObtenerResumenMensual()
// {
//     return _context.OrdenesFabricacion
//         .GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })
//         .Select(g => new ResumenMensualDto
//         {
//             Anio = g.Key.Year,
//             Mes = g.Key.Month,
//             TotalOrdenes = g.Count(),
//             TotalPlanchas = g.Sum(o => o.Planchas.Count()),
//             PesoTotal = g.Sum(o => o.Planchas.Sum(p => p.Peso))
//         })
//         .OrderBy(r => r.Anio)
//         .ThenBy(r => r.Mes)
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 11 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 12 =====
// Paso: Paso 4: Añadir la demostración en el caso de uso:
// Sección: Paso 4: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarResumenMensual()
// {
//     Console.WriteLine("\n--- Resumen mensual ---");
// 
//     var resumenes = _unidad.Ordenes.ObtenerResumenMensual();
//     foreach (var resumen in resumenes)
//     {
//         Console.WriteLine($"  {resumen.Anio}-{resumen.Mes:D2} | Órdenes: {resumen.TotalOrdenes} | Planchas: {resumen.TotalPlanchas} | Peso: {resumen.PesoTotal} kg");
//     }
// }
// ===== END CANONICAL PDF M03 3.5 BLOCK 12 =====
// ===== CANONICAL PDF M03 3.5 BLOCK 13 =====
// Paso: Paso 5: Llamar al método desde Ejecutar:
// Sección: Paso 5: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarResumenMensual();
// Paso 6: Ejecutar dotnet run y verificar que las órdenes se muestran agrupadas por mes y año.
// ===== END CANONICAL PDF M03 3.5 BLOCK 13 =====
