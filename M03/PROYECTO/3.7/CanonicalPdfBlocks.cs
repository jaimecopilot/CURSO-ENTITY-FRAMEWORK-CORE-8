// ============================================================================
// M03 3.7 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.7 BLOCK 1 =====
// Paso: Paso 2: Crear el DTO OrdenConPlanchasYDetalleDto
// Sección: Paso 2: Crear el DTO OrdenConPlanchasYDetalleDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenConPlanchasYDetalleDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConPlanchasYDetalleDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public List<PlanchaDto> Planchas { get; set; } = new();
//     public DetalleDto? Detalle { get; set; }
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 2 =====
// Paso: Paso 3: Crear el DTO OrdenConAleacionesDto
// Sección: Paso 3: Crear el DTO OrdenConAleacionesDto
// Ruta indicada por la práctica: src/AceriaData.Application/Dtos/OrdenConAleacionesDto.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenConAleacionesDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public List<AleacionDto> Aleaciones { get; set; } = new();
// }
// 
// public class AleacionDto
// {
//     public string Nombre { get; set; } = string.Empty;
//     public string Codigo { get; set; } = string.Empty;
//     public decimal PorcentajeCarbono { get; set; }
//     public decimal PorcentajeManganeso { get; set; }
//     public decimal CantidadUtilizada { get; set; }
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 3 =====
// Paso: Paso 4: Añadir los métodos de join a la interfaz del repositorio
// Sección: Paso 4: Añadir los métodos de join a la interfaz del repositorio
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
//     List<ResumenPorClienteConOrdenesDto> ObtenerResumenPorClienteConOrdenes();
//     List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstado();
//     List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstadoConFiltro();
//     List<ResumenPorClienteDto> ObtenerResumenPorClienteConMultiplesAgregaciones();
// 
//     List<OrdenConPlanchasYDetalleDto> ObtenerOrdenesConPlanchasYDetalle();
//     List<OrdenConAleacionesDto> ObtenerOrdenesConAleaciones();
//     List<OrdenConPlanchasYDetalleDto> ObtenerOrdenesConPlanchasYDetalleConJoinExplicito();
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 4 =====
// Paso: Paso 5: Implementar los métodos de join en el repositorio
// Sección: Paso 5: Implementar los métodos de join en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public List<OrdenConPlanchasYDetalleDto> ObtenerOrdenesConPlanchasYDetalle()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConPlanchasYDetalleDto
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
// 
// public List<OrdenConAleacionesDto> ObtenerOrdenesConAleaciones()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenConAleacionesDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Aleaciones = o.OrdenesAleaciones.Select(oa => new AleacionDto
//             {
//                 Nombre = oa.Aleacion.Nombre,
//                 Codigo = oa.Aleacion.Codigo,
//                 PorcentajeCarbono = oa.Aleacion.PorcentajeCarbono,
//                 PorcentajeManganeso = oa.Aleacion.PorcentajeManganeso,
//                 CantidadUtilizada = oa.CantidadUtilizada
//             }).ToList()
//         })
//         .ToList();
// }
// 
// public List<OrdenConPlanchasYDetalleDto> ObtenerOrdenesConPlanchasYDetalleConJoinExplicito()
// {
//     return _context.OrdenesFabricacion
//         .GroupJoin(
//             _context.PlanchasAcero,
//             o => o.Id,
//             p => p.OrdenId,
//             (o, planchas) => new { Orden = o, Planchas = planchas })
//         .GroupJoin(
//             _context.DetallesOrden,
//             op => op.Orden.Id,
//             d => d.OrdenId,
//             (op, detalles) => new { op.Orden, op.Planchas, Detalle = detalles.FirstOrDefault() })
//         .OrderBy(x => x.Orden.NumeroOrden)
//         .Select(x => new OrdenConPlanchasYDetalleDto
//         {
//             NumeroOrden = x.Orden.NumeroOrden,
//             Cliente = x.Orden.Cliente,
//             Estado = x.Orden.Estado,
//             Planchas = x.Planchas.Select(p => new PlanchaDto
//             {
//                 Id = p.Id,
//                 Espesor = p.Espesor,
//                 Ancho = p.Ancho,
//                 Largo = p.Largo,
//                 Peso = p.Peso,
//                 Activa = p.Activa
//             }).ToList(),
//             Detalle = x.Detalle == null ? null : new DetalleDto
//             {
//                 ComposicionQuimica = x.Detalle.ComposicionQuimica,
//                 TemperaturaColada = x.Detalle.TemperaturaColada,
//                 Notas = x.Detalle.Notas
//             }
//         })
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 5 =====
// Paso: Paso 6: Crear el caso de uso de joins
// Sección: Paso 6: Crear el caso de uso de joins
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/JoinsUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class JoinsUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public JoinsUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== JOINS Y NAVEGACIÓN ===");
// 
//         DemostrarOrdenesConPlanchasYDetalle();
//         DemostrarOrdenesConAleaciones();
//         DemostrarJoinExplicito();
//         DemostrarSqlJoin();
//     }
// 
//     private void DemostrarOrdenesConPlanchasYDetalle()
//     {
//         Console.WriteLine("\n--- Órdenes con planchas y detalle (navegación por propiedades) ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalle();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//             Console.WriteLine($"    Planchas: {orden.Planchas.Count}");
//             var detalle = orden.Detalle == null ? "Sin detalle" : $"{orden.Detalle.ComposicionQuimica} | {orden.Detalle.TemperaturaColada}°C";
//             Console.WriteLine($"    Detalle: {detalle}");
//         }
//     }
// 
//     private void DemostrarOrdenesConAleaciones()
//     {
//         Console.WriteLine("\n--- Órdenes con aleaciones (navegación por propiedades) ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConAleaciones();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente}");
//             foreach (var aleacion in orden.Aleaciones)
//             {
//                 Console.WriteLine($"    {aleacion.Nombre} ({aleacion.Codigo}) | C: {aleacion.PorcentajeCarbono}% | Mn: {aleacion.PorcentajeManganeso}% | Cantidad: {aleacion.CantidadUtilizada} kg");
//             }
//         }
//     }
// 
//     private void DemostrarJoinExplicito()
//     {
//         Console.WriteLine("\n--- Órdenes con planchas y detalle (Join explícito) ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleConJoinExplicito();
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//             Console.WriteLine($"    Planchas: {orden.Planchas.Count}");
//         }
//     }
// 
//     private void DemostrarSqlJoin()
//     {
//         Console.WriteLine("\n--- SQL generado por una consulta con navegación ---");
// 
//         var consulta = _unidad.Ordenes.ObtenerQueryable()
//             .OrderBy(o => o.NumeroOrden)
//             .Select(o => new
//             {
//                 o.NumeroOrden,
//                 o.Cliente,
//                 Planchas = o.Planchas.Select(p => new { p.Espesor, p.Peso }).ToList(),
//                 Detalle = o.Detalle == null ? null : new { o.Detalle.ComposicionQuimica, o.Detalle.TemperaturaColada }
//             });
// 
//         var sql = consulta.ToQueryString();
//         Console.WriteLine(sql);
//     }
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 6 =====
// Paso: Paso 7: Registrar el caso de uso en el contenedor
// Sección: Paso 7: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<JoinsUseCase>();
// ===== END CANONICAL PDF M03 3.7 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 7 =====
// Paso: Paso 8: Llamar al caso de uso desde la consola
// Sección: Paso 8: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<JoinsUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 8 =====
// Paso: Paso 9: Insertar datos de prueba con planchas, detalle y aleaciones
// Sección: Paso 9: Insertar datos de prueba con planchas, detalle y aleaciones
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
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = true };
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
// ===== END CANONICAL PDF M03 3.7 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 9 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 1: Crear el DTO OrdenCompletaConAleacionesDto:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// namespace AceriaData.Application.Dtos;
// 
// public class OrdenCompletaConAleacionesDto
// {
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public string Estado { get; set; } = string.Empty;
//     public List<PlanchaDto> Planchas { get; set; } = new();
//     public DetalleDto? Detalle { get; set; }
//     public List<AleacionDto> Aleaciones { get; set; } = new();
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 10 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// List<OrdenCompletaConAleacionesDto> ObtenerOrdenesCompletasConAleaciones();
// ===== END CANONICAL PDF M03 3.7 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 11 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 3: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenCompletaConAleacionesDto> ObtenerOrdenesCompletasConAleaciones()
// {
//     return _context.OrdenesFabricacion
//         .OrderBy(o => o.NumeroOrden)
//         .Select(o => new OrdenCompletaConAleacionesDto
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
//             },
//             Aleaciones = o.OrdenesAleaciones.Select(oa => new AleacionDto
//             {
//                 Nombre = oa.Aleacion.Nombre,
//                 Codigo = oa.Aleacion.Codigo,
//                 PorcentajeCarbono = oa.Aleacion.PorcentajeCarbono,
//                 PorcentajeManganeso = oa.Aleacion.PorcentajeManganeso,
//                 CantidadUtilizada = oa.CantidadUtilizada
//             }).ToList()
//         })
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 11 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 12 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 4: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarOrdenesCompletasConAleaciones()
// {
//     Console.WriteLine("\n--- Órdenes completas con aleaciones ---");
// 
//     var ordenes = _unidad.Ordenes.ObtenerOrdenesCompletasConAleaciones();
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden} | {orden.Cliente} | {orden.Estado}");
//         Console.WriteLine($"    Planchas: {orden.Planchas.Count}");
//         var detalle = orden.Detalle == null ? "Sin detalle" : $"{orden.Detalle.ComposicionQuimica}";
//         Console.WriteLine($"    Detalle: {detalle}");
//         Console.WriteLine($"    Aleaciones: {orden.Aleaciones.Count}");
//     }
// }
// ===== END CANONICAL PDF M03 3.7 BLOCK 12 =====
// ===== CANONICAL PDF M03 3.7 BLOCK 13 =====
// Paso: Paso 11: Analizar la salida
// Sección: Paso 5: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarOrdenesCompletasConAleaciones();
// Paso 6: Ejecutar dotnet run y verificar que las órdenes se muestran con sus planchas, detalle y aleaciones.
// ===== END CANONICAL PDF M03 3.7 BLOCK 13 =====
