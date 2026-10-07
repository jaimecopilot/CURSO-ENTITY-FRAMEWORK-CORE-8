// ============================================================================
// M03 3.4 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.4 BLOCK 1 =====
// Paso: Paso 2: Crear el DTO PlanchaDto
// Sección: Paso 2: Crear el DTO PlanchaDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/PlanchaDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class PlanchaDto
// {
//     public int Id { get; set; }
//     public double Espesor { get; set; }
//     public double Ancho { get; set; }
//     public double Largo { get; set; }
//     public decimal Peso { get; set; }
//     public bool Activa { get; set; }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 2 =====
// Paso: Paso 3: Crear el DTO OrdenConPlanchasDto
// Sección: Paso 3: Crear el DTO OrdenConPlanchasDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenConPlanchasDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConPlanchasDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public List<PlanchaDto> Planchas { get; set; } = new();
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 3 =====
// Paso: Paso 4: Crear el DTO DetalleDto
// Sección: Paso 4: Crear el DTO DetalleDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/DetalleDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class DetalleDto
// {
//     public string ComposicionQuimica { get; set; } = string.Empty;
//     public double TemperaturaColada { get; set; }
//     public string? Notas { get; set; }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 4 =====
// Paso: Paso 5: Crear el DTO OrdenConDetalleDto
// Sección: Paso 5: Crear el DTO OrdenConDetalleDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenConDetalleDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConDetalleDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public DetalleDto? Detalle { get; set; }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 5 =====
// Paso: Paso 6: Crear el DTO OrdenConTotalesDto
// Sección: Paso 6: Crear el DTO OrdenConTotalesDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenConTotalesDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConTotalesDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public int TotalPlanchas { get; set; }
//     public decimal PesoTotal { get; set; }
//     public double PesoPromedio { get; set; }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 6 =====
// Paso: Paso 7: Añadir los métodos de proyección a la interfaz del repositorio
// Sección: Paso 7: Añadir los métodos de proyección a la interfaz del repositorio
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
//     List<OrdenConPlanchasDto> ObtenerOrdenesConPlanchas();
//     List<OrdenConDetalleDto> ObtenerOrdenesConDetalle();
//     List<OrdenConTotalesDto> ObtenerOrdenesConTotalesDetallados();
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 7 =====
// Paso: Paso 8: Implementar los métodos de proyección en el repositorio
// Sección: Paso 8: Implementar los métodos de proyección en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public List<OrdenConPlanchasDto> ObtenerOrdenesConPlanchas()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConPlanchasDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             Planchas = o.Planchas.Select(p => new PlanchaDto
//             {
//                 Id = p.Id,
//                 Espesor = p.Espesor,
//                 Ancho = p.Ancho,
//                 Largo = p.Largo,
//                 Peso = p.Peso,
//                 Activa = p.Activa
//             }).ToList()
//         })
//         .ToList();
// }
// 
// public List<OrdenConDetalleDto> ObtenerOrdenesConDetalle()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConDetalleDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             Detalle = o.Detalle == null ? null : new DetalleDto
//             {
//                 ComposicionQuimica = o.Detalle.ComposicionQuimica,
//                 TemperaturaColada = o.Detalle.TemperaturaColada,
//                 Notas = o.Detalle.Notas
//             }
//         })
//         .ToList();
// }
// 
// public List<OrdenConTotalesDto> ObtenerOrdenesConTotalesDetallados()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConTotalesDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             TotalPlanchas = o.Planchas.Count(),
//             PesoTotal = o.Planchas.Sum(p => p.Peso),
//             PesoPromedio = o.Planchas.Average(p => (double)p.Peso)
//         })
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 8 =====
// Paso: Paso 9: Crear el caso de uso de proyecciones a DTOs
// Sección: Paso 9: Crear el caso de uso de proyecciones a DTOs
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/ProyeccionesDtoUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class ProyeccionesDtoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public ProyeccionesDtoUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PROYECCIONES A DTOS ===");
// 
//         DemostrarOrdenesConPlanchas();
//         DemostrarOrdenesConDetalle();
//         DemostrarOrdenesConTotalesDetallados();
//         DemostrarSqlProyeccionConPlanchas();
//     }
// 
//     private void DemostrarOrdenesConPlanchas()
//     {
//         Console.WriteLine("\n--- Órdenes con planchas ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchas();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | Planchas: {orden.Planchas.Count}");
//             foreach (var plancha in orden.Planchas)
//             {
//                 Console.WriteLine($"    Plancha {plancha.Id} | Espesor: {plancha.Espesor} | Peso: {plancha.Peso} kg");
//             }
//         }
//     }
// 
//     private void DemostrarOrdenesConDetalle()
//     {
//         Console.WriteLine("\n--- Órdenes con detalle ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConDetalle();
//         foreach (var orden in ordenes)
//         {
//             var detalle = orden.Detalle == null ? "Sin detalle" : $"{orden.Detalle.ComposicionQuimica} | {orden.Detalle.TemperaturaColada}°C";
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado} | Detalle: {detalle}");
//         }
//     }
// 
//     private void DemostrarOrdenesConTotalesDetallados()
//     {
//         Console.WriteLine("\n--- Órdenes con totales detallados ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConTotalesDetallados();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | Planchas: {orden.TotalPlanchas} | Peso total: {orden.PesoTotal} kg | Peso promedio: {orden.PesoPromedio:F2} kg");
//         }
//     }
// 
//     private void DemostrarSqlProyeccionConPlanchas()
//     {
//         Console.WriteLine("\n--- SQL generado por una proyección con colección de navegación ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .OrderBy(o => o.NumeroOrden)
//             .Select(o => new
//             {
//                 o.NumeroOrden,
//                 o.Cliente,
//                 Planchas = o.Planchas.Select(p => new { p.Id, p.Espesor, p.Peso }).ToList()
//             });
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 9 =====
// Paso: Paso 10: Registrar el caso de uso en el contenedor
// Sección: Paso 10: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<ProyeccionesDtoUseCase>();
// ===== END CANONICAL PDF M03 3.4 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 10 =====
// Paso: Paso 11: Llamar al caso de uso desde la consola
// Sección: Paso 11: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<ProyeccionesDtoUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 11 =====
// Paso: Paso 12: Insertar datos de prueba con planchas y detalle
// Sección: Paso 12: Insertar datos de prueba con planchas y detalle
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
// 
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
// 
//     context.OrdenesFabricacion.AddRange(orden1, orden2);
//     context.SaveChanges();
// 
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
// 
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
// 
//     var detalle1 = new DetalleOrden { OrdenId = orden1.Id, ComposicionQuimica = "C: 0.45%, Mn: 0.75%", TemperaturaColada = 1550.5 };
//     context.DetallesOrden.Add(detalle1);
// 
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 11 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 12 =====
// Paso: Paso 14: Analizar la salida
// Sección: Paso 1: Crear el DTO OrdenCompletaDto:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenCompletaDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public List<PlanchaDto> Planchas { get; set; } = new();
//     public DetalleDto? Detalle { get; set; }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 12 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 13 =====
// Paso: Paso 14: Analizar la salida
// Sección: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// List<OrdenCompletaDto> ObtenerOrdenesCompletas();
// ===== END CANONICAL PDF M03 3.4 BLOCK 13 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 14 =====
// Paso: Paso 14: Analizar la salida
// Sección: Paso 3: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenCompletaDto> ObtenerOrdenesCompletas()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenCompletaDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             Planchas = o.Planchas.Select(p => new PlanchaDto
//             {
//                 Id = p.Id,
//                 Espesor = p.Espesor,
//                 Ancho = p.Ancho,
//                 Largo = p.Largo,
//                 Peso = p.Peso,
//                 Activa = p.Activa
//             }).ToList(),
//             Detalle = o.Detalle == null ? null : new DetalleDto
//             {
//                 ComposicionQuimica = o.Detalle.ComposicionQuimica,
//                 TemperaturaColada = o.Detalle.TemperaturaColada,
//                 Notas = o.Detalle.Notas
//             }
//         })
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 14 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 15 =====
// Paso: Paso 14: Analizar la salida
// Sección: Paso 4: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarOrdenesCompletas()
// {
//     Console.WriteLine("\n--- Órdenes completas ---");
// 
//     var ordenes = _unidad.Ordenes.ObtenerOrdenesCompletas();
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//         Console.WriteLine($"    Planchas: {orden.Planchas.Count}");
//         var detalle = orden.Detalle == null ? "Sin detalle" : $"{orden.Detalle.ComposicionQuimica} | {orden.Detalle.TemperaturaColada}°C";
//         Console.WriteLine($"    Detalle: {detalle}");
//     }
// }
// ===== END CANONICAL PDF M03 3.4 BLOCK 15 =====
// ===== CANONICAL PDF M03 3.4 BLOCK 16 =====
// Paso: Paso 14: Analizar la salida
// Sección: Paso 5: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarOrdenesCompletas();
// Paso 6: Ejecutar dotnet run y verificar que las órdenes se muestran con sus planchas y detalle.
// ===== END CANONICAL PDF M03 3.4 BLOCK 16 =====
