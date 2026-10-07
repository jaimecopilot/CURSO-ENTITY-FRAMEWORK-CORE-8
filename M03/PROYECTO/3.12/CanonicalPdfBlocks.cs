// ============================================================================
// M03 3.12 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.12 BLOCK 1 =====
// Paso: Paso 2: Refactorizar los métodos del repositorio con buenas prácticas
// Sección: Paso 2: Refactorizar los métodos del repositorio con buenas prácticas
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// /// <summary>
// /// Obtiene los resúmenes de las órdenes pendientes.
// /// Usa proyección para reducir el volumen de datos transferidos.
// /// Usa AsNoTracking porque es una consulta de solo lectura.
// /// </summary>
// public List<OrdenResumenDto> ObtenerResumenesPendientes()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// 
// /// <summary>
// /// Cuenta las órdenes pendientes.
// /// Usa Any en lugar de Count cuando solo se quiere saber si hay elementos.
// /// </summary>
// public bool ExisteAlgunaOrdenPendiente()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Any(o => o.Estado == "Pendiente");
// }
// 
// /// <summary>
// /// Obtiene las órdenes con planchas y detalle usando AsSplitQuery.
// /// Usa AsSplitQuery para evitar el producto cartesiano.
// /// </summary>
// public List<OrdenConPlanchasYDetalleDto> ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Include(o => o.Planchas)
//         .Include(o => o.Detalle)
//         .AsSplitQuery()
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
// ===== END CANONICAL PDF M03 3.12 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 2 =====
// Paso: Paso 3: Crear el caso de uso de buenas prácticas
// Sección: Paso 3: Crear el caso de uso de buenas prácticas
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/BuenasPracticasUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class BuenasPracticasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public BuenasPracticasUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== BUENAS PRÁCTICAS EN EL ACCESO A DATOS ===");
// 
//         DemostrarProyeccionConAsNoTracking();
//         DemostrarAnyEnLugarDeCount();
//         DemostrarAsSplitQuerySinProductoCartesiano();
//         DemostrarFirstOrDefault();
//         DemostrarDocumentacionDeDecisiones();
//     }
// 
//     private void DemostrarProyeccionConAsNoTracking()
//     {
//         Console.WriteLine("\n--- Proyección con AsNoTracking ---");
// 
//         var resumenes = _unidad.Ordenes.ObtenerResumenesPendientes();
//         Console.WriteLine($"Resúmenes pendientes: {resumenes.Count}");
//         foreach (var resumen in resumenes)
//         {
//             Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.Estado} | {resumen.FechaCreacion:dd/MM/yyyy}");
//         }
//     }
// 
//     private void DemostrarAnyEnLugarDeCount()
//     {
//         Console.WriteLine("\n--- Any en lugar de Count ---");
// 
//         var existe = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();
//         Console.WriteLine($"Existe alguna orden pendiente: {existe}");
//     }
// 
//     private void DemostrarAsSplitQuerySinProductoCartesiano()
//     {
//         Console.WriteLine("\n--- AsSplitQuery sin producto cartesiano ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();
//         foreach (var orden in ordenes)
//         {
//             var detalle = orden.Detalle == null ? "Sin detalle" : orden.Detalle.ComposicionQuimica;
//             Console.WriteLine($"  {orden.NumeroOrden} | Planchas: {orden.Planchas.Count} | Detalle: {detalle}");
//         }
//     }
// 
//     private void DemostrarFirstOrDefault()
//     {
//         Console.WriteLine("\n--- FirstOrDefault en lugar de First ---");
// 
//         var orden = _unidad.Ordenes.ObtenerPorNumeroOrden("OF-2024-9999");
//         var resultado = orden is null ? "No encontrada" : orden.NumeroOrden;
//         Console.WriteLine($"Orden OF-2024-9999: {resultado}");
//     }
// 
//     private void DemostrarDocumentacionDeDecisiones()
//     {
//         Console.WriteLine("\n--- Documentación de decisiones ---");
//         Console.WriteLine("Los métodos del repositorio incluyen comentarios XML que documentan las decisiones de acceso a datos.");
//         Console.WriteLine("Consultar el código fuente de OrdenRepositorio para más detalles.");
//     }
// }
// ===== END CANONICAL PDF M03 3.12 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 3 =====
// Paso: Paso 4: Registrar el caso de uso en el contenedor
// Sección: Paso 4: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<BuenasPracticasUseCase>();
// ===== END CANONICAL PDF M03 3.12 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 4 =====
// Paso: Paso 5: Llamar al caso de uso desde la consola
// Sección: Paso 5: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<BuenasPracticasUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.12 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 5 =====
// Paso: Paso 6: Insertar datos de prueba con planchas y detalle
// Sección: Paso 6: Insertar datos de prueba con planchas y detalle
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
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
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
// 
//     var detalle1 = new DetalleOrden { OrdenId = orden1.Id, ComposicionQuimica = "C: 0.45%, Mn: 0.75%", TemperaturaColada = 1550.5 };
//     context.DetallesOrden.Add(detalle1);
// 
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.12 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 6 =====
// Paso: Paso 8: Analizar la salida
// Sección: Paso 8: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public List<OrdenResumenDto> ObtenerResumenesPendientes()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == "Pendiente")
//         .OrderBy(o => o.FechaCreacion)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// Resultado esperado con la solución: las entidades no se registran en el Change Tracker y se libera memoria.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// AsNoTracking en consulta de escritura	Se aplicó AsNoTracking a una consulta que modifica datos	Usar AsNoTracking solo en consultas de solo lectura
// Count() > 0 en lugar de Any	Se usó Count para saber si hay elementos	Usar Any
// First en lugar de FirstOrDefault	Se usó First cuando puede no haber resultados	Usar FirstOrDefault
// Producto cartesiano	Se incluyeron varias colecciones sin AsSplitQuery	Usar AsSplitQuery
// Materialización prematura	Se llamó a ToList antes de aplicar todos los filtros	Materializar solo al final
// N+1	Se accede a propiedades de navegación en un bucle sin Include	Usar Include
// Falta de documentación	No se documentaron las decisiones	Añadir comentarios XML
// Reto resuelto: Refactorizar un método con todas las buenas prácticas
// Reto: Refactorizar el método ObtenerPorEstadoOrdenadasPorFecha del repositorio para aplicar todas las buenas prácticas: proyección, AsNoTracking, OrderBy, ToList al final y documentación XML. Añadir el método refactorizado, el DTO correspondiente y una demostración en el caso de uso.
// 
// Solución paso a paso:
// 
// Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// 
// csharp
// List<OrdenResumenDto> ObtenerResumenesPorEstadoOrdenados(string estado);
// ===== END CANONICAL PDF M03 3.12 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 7 =====
// Paso: Paso 2: Implementar el método en OrdenRepositorio:
// Sección: Paso 2: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// /// <summary>
// /// Obtiene los resúmenes de las órdenes de un estado concreto, ordenados por fecha.
// /// Usa proyección para reducir el volumen de datos transferidos.
// /// Usa AsNoTracking porque es una consulta de solo lectura.
// /// Materializa con ToList al final para ejecutar una sola consulta.
// /// </summary>
// public List<OrdenResumenDto> ObtenerResumenesPorEstadoOrdenados(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.FechaCreacion)
//         .Select(o => new OrdenResumenDto
//         {
//             NumeroOrden = o.NumeroOrden,
//             Cliente = o.Cliente,
//             Estado = o.Estado,
//             FechaCreacion = o.FechaCreacion
//         })
//         .ToList();
// }
// ===== END CANONICAL PDF M03 3.12 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 8 =====
// Paso: Paso 3: Añadir la demostración en el caso de uso:
// Sección: Paso 3: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarResumenesPorEstadoOrdenados()
// {
//     Console.WriteLine("\n--- Resúmenes por estado ordenados ---");
// 
//     var resumenes = _unidad.Ordenes.ObtenerResumenesPorEstadoOrdenados("Pendiente");
//     Console.WriteLine($"Resúmenes pendientes: {resumenes.Count}");
//     foreach (var resumen in resumenes)
//     {
//         Console.WriteLine($"  {resumen.NumeroOrden} | {resumen.Cliente} | {resumen.FechaCreacion:dd/MM/yyyy}");
//     }
// }
// ===== END CANONICAL PDF M03 3.12 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.12 BLOCK 9 =====
// Paso: Paso 4: Llamar al método desde Ejecutar:
// Sección: Paso 4: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarResumenesPorEstadoOrdenados();
// Paso 5: Ejecutar dotnet run y verificar que los resúmenes se muestran ordenados por fecha.
// ===== END CANONICAL PDF M03 3.12 BLOCK 9 =====
