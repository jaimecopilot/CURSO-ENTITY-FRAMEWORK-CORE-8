// ============================================================================
// M03 3.9 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.9 BLOCK 1 =====
// Paso: Paso 3: Marcar las propiedades de navegación como virtual
// Sección: Paso 3: Marcar las propiedades de navegación como virtual
// Ruta indicada por la práctica: (sin ruta explícita en el bloque)
// namespace AceriaData.Domain.Entities;
// 
// public class OrdenFabricacion
// {
//     public int Id { get; set; }
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public DateTime FechaCreacion { get; set; }
//     public DateTime? FechaEntrega { get; set; }
//     public string Estado { get; set; } = string.Empty;
//     public string? Observaciones { get; set; }
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
// 
//     public virtual List<PlanchaAcero> Planchas { get; set; } = new();
//     public virtual DetalleOrden? Detalle { get; set; }
//     public virtual CertificadoCalidad? Certificado { get; set; }
//     public virtual List<OrdenAleacion> OrdenesAleaciones { get; set; } = new();
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 2 =====
// Paso: Paso 3: Marcar las propiedades de navegación como virtual
// Sección: Paso 3: Marcar las propiedades de navegación como virtual
// Ruta indicada por la práctica: (sin ruta explícita en el bloque)
// namespace AceriaData.Domain.Entities;
// 
// public class PlanchaAcero
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public double Espesor { get; set; }
//     public double Ancho { get; set; }
//     public double Largo { get; set; }
//     public decimal Peso { get; set; }
//     public bool Activa { get; set; }
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
// 
//     public virtual OrdenFabricacion Orden { get; set; } = null!;
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 3 =====
// Paso: Paso 3: Marcar las propiedades de navegación como virtual
// Sección: Paso 3: Marcar las propiedades de navegación como virtual
// Ruta indicada por la práctica: (sin ruta explícita en el bloque)
// namespace AceriaData.Domain.Entities;
// 
// public class Aleacion
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public string Codigo { get; set; } = string.Empty;
//     public decimal PorcentajeCarbono { get; set; }
//     public decimal PorcentajeManganeso { get; set; }
//     public string? Descripcion { get; set; }
//     public bool Activo { get; set; }
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
// 
//     public virtual List<OrdenAleacion> OrdenesAleaciones { get; set; } = new();
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 4 =====
// Paso: Paso 3: Marcar las propiedades de navegación como virtual
// Sección: Paso 3: Marcar las propiedades de navegación como virtual
// Ruta indicada por la práctica: (sin ruta explícita en el bloque)
// namespace AceriaData.Domain.Entities;
// 
// public class DetalleOrden
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public string ComposicionQuimica { get; set; } = string.Empty;
//     public double TemperaturaColada { get; set; }
//     public string? Notas { get; set; }
// 
//     public virtual OrdenFabricacion Orden { get; set; } = null!;
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 5 =====
// Paso: Paso 3: Marcar las propiedades de navegación como virtual
// Sección: Paso 3: Marcar las propiedades de navegación como virtual
// Ruta indicada por la práctica: (sin ruta explícita en el bloque)
// namespace AceriaData.Domain.Entities;
// 
// public class CertificadoCalidad
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public string NumeroCertificado { get; set; } = string.Empty;
//     public DateTime FechaEmision { get; set; }
//     public string OrganismoCertificador { get; set; } = string.Empty;
// 
//     public virtual OrdenFabricacion Orden { get; set; } = null!;
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 6 =====
// Paso: Paso 3: Marcar las propiedades de navegación como virtual
// Sección: Paso 3: Marcar las propiedades de navegación como virtual
// Ruta indicada por la práctica: (sin ruta explícita en el bloque)
// namespace AceriaData.Domain.Entities;
// 
// public class OrdenAleacion
// {
//     public int OrdenFabricacionId { get; set; }
//     public int AleacionId { get; set; }
//     public DateTime FechaAsignacion { get; set; }
//     public decimal CantidadUtilizada { get; set; }
//     public string EstadoRelacion { get; set; } = string.Empty;
// 
//     public virtual OrdenFabricacion Orden { get; set; } = null!;
//     public virtual Aleacion Aleacion { get; set; } = null!;
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 7 =====
// Paso: Paso 4: Configurar la carga Lazy en el DbContext
// Sección: Paso 4: Configurar la carga Lazy en el DbContext
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Persistence/AceriaDbContext.cs
// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// {
//     if (!optionsBuilder.IsConfigured)
//     {
//         optionsBuilder
//             .UseSqlServer(
//                 "Server=(localdb)\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;",
//                 sqlOptions =>
//                 {
//                     sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//                     sqlOptions.CommandTimeout(60);
//                 })
//             .UseLazyLoadingProxies()
//             .LogTo(
//                 Console.WriteLine,
//                 new[] { "Microsoft.EntityFrameworkCore.Database.Command" },
//                 LogLevel.Information)
//             .EnableSensitiveDataLogging()
//             .EnableDetailedErrors();
//     }
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 8 =====
// Paso: Paso 5: Crear el caso de uso de carga Lazy
// Sección: Paso 5: Crear el caso de uso de carga Lazy
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/CargaLazyUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class CargaLazyUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public CargaLazyUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CARGA LAZY CON PROXIES ===");
// 
//         DemostrarCargaLazySimple();
//         DemostrarProblemaN1();
//         DemostrarCargaEagerComoAlternativa();
//     }
// 
//     private void DemostrarCargaLazySimple()
//     {
//         Console.WriteLine("\n--- Carga Lazy simple ---");
// 
//         var orden = _unidad.Ordenes.ObtenerPorId(1);
//         if (orden is not null)
//         {
//             Console.WriteLine($"  Orden: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
//             Console.WriteLine($"  Tipo de la entidad: {orden.GetType().Name}");
//             Console.WriteLine($"  Planchas (carga Lazy): {orden.Planchas.Count}");
//         }
//     }
// 
//     private void DemostrarProblemaN1()
//     {
//         Console.WriteLine("\n--- Problema N+1 con carga Lazy ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerTodas();
//         Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
// 
//         var totalPlanchas = 0;
//         foreach (var orden in ordenes)
//         {
//             totalPlanchas += orden.Planchas.Count;
//         }
// 
//         Console.WriteLine($"Total de planchas: {totalPlanchas}");
//     }
// 
//     private void DemostrarCargaEagerComoAlternativa()
//     {
//         Console.WriteLine("\n--- Carga Eager como alternativa ---");
// 
//         var ordenes = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();
//         var totalPlanchas = ordenes.Sum(o => o.Planchas.Count);
// 
//         Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//         Console.WriteLine($"Total de planchas: {totalPlanchas}");
//     }
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 9 =====
// Paso: Paso 6: Registrar el caso de uso en el contenedor
// Sección: Paso 6: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<CargaLazyUseCase>();
// ===== END CANONICAL PDF M03 3.9 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 10 =====
// Paso: Paso 7: Llamar al caso de uso desde la consola
// Sección: Paso 7: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<CargaLazyUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 10 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 11 =====
// Paso: Paso 8: Insertar datos de prueba con varias órdenes y planchas
// Sección: Paso 8: Insertar datos de prueba con varias órdenes y planchas
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
// 
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     var orden3 = new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 3, 10) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2, orden3);
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
// ===== END CANONICAL PDF M03 3.9 BLOCK 11 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 12 =====
// Paso: Paso 10: Analizar la salida
// Sección: Paso 10: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// var ordenes = _unidad.Ordenes.ObtenerQueryable()
//     .Include(o => o.Planchas)
//     .ThenInclude(p => p.Orden)
//     .ThenInclude(o => o.OrdenesAleaciones)
//     .ToList();
// Resultado esperado con la solución: una sola consulta carga todas las entidades relacionadas.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// Carga Lazy no funciona	Las propiedades de navegación no son virtual	Marcar como virtual
// Carga Lazy no funciona	No se configuró UseLazyLoadingProxies	Configurar en OnConfiguring
// N+1 en bucle	Se accede a propiedades de navegación en un bucle	Usar Include
// ObjectDisposedException	Se accede a propiedades de navegación fuera del ámbito	Usar Include o DTOs
// Referencia circular en serialización	Las entidades se referencian mutuamente	Usar DTOs o configurar el serializador
// NullReferenceException	La propiedad de navegación es null	Comprobar antes de acceder
// Proxies no generados	Las clases son sealed	Quitar sealed
// Reto resuelto: Comparar carga Lazy y carga Eager con medición de tiempo
// Reto: Crear un método en el caso de uso que mida el tiempo de ejecución de una consulta con carga Lazy y otra con carga Eager. Comparar los resultados y analizar la diferencia.
// 
// Solución paso a paso:
// 
// Paso 1: Añadir el método CompararRendimiento al caso de uso:
// 
// csharp
// private void CompararRendimiento()
// {
//     Console.WriteLine("\n--- Comparación de rendimiento ---");
// 
//     var cronometroLazy = System.Diagnostics.Stopwatch.StartNew();
//     var ordenesLazy = _unidad.Ordenes.ObtenerTodas();
//     var totalLazy = ordenesLazy.Sum(o => o.Planchas.Count);
//     cronometroLazy.Stop();
// 
//     Console.WriteLine($"Carga Lazy: {cronometroLazy.ElapsedMilliseconds} ms | Total planchas: {totalLazy}");
// 
//     var cronometroEager = System.Diagnostics.Stopwatch.StartNew();
//     var ordenesEager = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();
//     var totalEager = ordenesEager.Sum(o => o.Planchas.Count);
//     cronometroEager.Stop();
// 
//     Console.WriteLine($"Carga Eager: {cronometroEager.ElapsedMilliseconds} ms | Total planchas: {totalEager}");
// }
// ===== END CANONICAL PDF M03 3.9 BLOCK 12 =====
// ===== CANONICAL PDF M03 3.9 BLOCK 13 =====
// Paso: Paso 2: Llamar al método desde Ejecutar:
// Sección: Paso 2: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// CompararRendimiento();
// Paso 3: Ejecutar dotnet run y comparar los tiempos.
// ===== END CANONICAL PDF M03 3.9 BLOCK 13 =====
