// ========================================================================
// M05 5.9 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 01
// SECTION: Paso 2: Crear la interfaz genérica IRepositorio
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/Interfaces/IRepositorio.cs:
// ------------------------------------------------------------------------
// namespace AceriaData.Application.Interfaces;
//
// public interface IRepositorio<T> where T : class
// {
//     T? ObtenerPorId(int id);
//     List<T> ObtenerTodas();
//     void Agregar(T entidad);
//     void Eliminar(T entidad);
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 02
// SECTION: Paso 3: Crear la implementación genérica Repositorio
// SOURCE TARGET: Crear el archivo src/AceriaData.Infrastructure/Repositories/Repositorio.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public class Repositorio<T> : IRepositorio<T> where T : class
// {
//     protected readonly AceriaDbContext _context;
//     protected readonly DbSet<T> _dbSet;
//
//     public Repositorio(AceriaDbContext context)
//     {
//         _context = context;
//         _dbSet = context.Set<T>();
//     }
//
//     public virtual T? ObtenerPorId(int id)
//     {
//         return _dbSet.Find(id);
//     }
//
//     public virtual List<T> ObtenerTodas()
//     {
//         return _dbSet.ToList();
//     }
//
//     public virtual void Agregar(T entidad)
//     {
//         _dbSet.Add(entidad);
//     }
//
//     public virtual void Eliminar(T entidad)
//     {
//         _dbSet.Remove(entidad);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 03
// SECTION: Paso 4: Refactorizar IOrdenRepositorio para heredar de IRepositorio
// SOURCE TARGET: Modificar src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
//
// namespace AceriaData.Application.Interfaces;
//
// public interface IOrdenRepositorio : IRepositorio<OrdenFabricacion>
// {
//     OrdenFabricacion? ObtenerPorNumeroOrden(string numeroOrden);
//     List<OrdenFabricacion> ObtenerPendientes();
//     List<OrdenFabricacion> ObtenerPorCliente(string cliente);
//     List<OrdenFabricacion> ObtenerPorEstadoOrdenadasPorFecha(string estado);
//     List<OrdenResumenDto> ObtenerResumenes();
//     int ContarOrdenes();
//     bool ExisteAlgunaOrden();
//     List<OrdenFabricacion> ObtenerOrdenesConPlanchasInclude();
//     List<OrdenFabricacion> ObtenerConAleacionesThenInclude();
//     List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery();
//     OrdenFabricacion? ObtenerPorIdParaActualizar(int id);
//     bool ActualizarClienteConClienteGana(OrdenFabricacion orden, string nuevoCliente);
//     bool InsertarDosOrdenesConTransaccion(string numero1, string numero2);
//     void AplicarMigracionHasta(string nombreMigracion);
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 04
// SECTION: Paso 5: Refactorizar OrdenRepositorio para heredar de Repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public class OrdenRepositorio : Repositorio<OrdenFabricacion>, IOrdenRepositorio
// {
//     private readonly ILogger<OrdenRepositorio> _logger;
//
//     public OrdenRepositorio(AceriaDbContext context, ILogger<OrdenRepositorio> logger) : base(context)
//     {
//         _logger = logger;
//     }
//
//     public OrdenFabricacion? ObtenerPorNumeroOrden(string numeroOrden)
//     {
//         return _context.OrdenesFabricacion
//             .FirstOrDefault(o => o.NumeroOrden == numeroOrden);
//     }
//
//     public List<OrdenFabricacion> ObtenerPendientes()
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Estado == "Pendiente")
//             .ToList();
//     }
//
//     public List<OrdenFabricacion> ObtenerPorCliente(string cliente)
//     {
//         return _context.OrdenesFabricacion
//             .Where(o => o.Cliente == cliente)
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
//     public List<OrdenResumenDto> ObtenerResumenes()
//     {
//         return _context.OrdenesFabricacion
//             .AsNoTracking()
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
//     public int ContarOrdenes()
//     {
//         return _context.OrdenesFabricacion.Count();
//     }
//
//     public bool ExisteAlgunaOrden()
//     {
//         return _context.OrdenesFabricacion.Any();
//     }
//
//     public List<OrdenFabricacion> ObtenerOrdenesConPlanchasInclude()
//     {
//         return _context.OrdenesFabricacion
//             .AsNoTracking()
//             .Include(o => o.Planchas)
//             .OrderBy(o => o.NumeroOrden)
//             .ToList();
//     }
//
//     public List<OrdenFabricacion> ObtenerConAleacionesThenInclude()
//     {
//         return _context.OrdenesFabricacion
//             .AsNoTrackingWithIdentityResolution()
//             .Include(o => o.OrdenesAleaciones)
//             .ThenInclude(oa => oa.Aleacion)
//             .OrderBy(o => o.NumeroOrden)
//             .ToList();
//     }
//
//     public List<OrdenFabricacion> ObtenerConPlanchasYDetalleSplitQuery()
//     {
//         return _context.OrdenesFabricacion
//             .AsNoTrackingWithIdentityResolution()
//             .Include(o => o.Planchas)
//             .Include(o => o.Detalle)
//             .AsSplitQuery()
//             .OrderBy(o => o.NumeroOrden)
//             .ToList();
//     }
//
//     public OrdenFabricacion? ObtenerPorIdParaActualizar(int id)
//     {
//         return _context.OrdenesFabricacion
//             .FirstOrDefault(o => o.Id == id);
//     }
//
//     public bool ActualizarClienteConClienteGana(OrdenFabricacion orden, string nuevoCliente)
//     {
//         orden.Cliente = nuevoCliente;
//
//         try
//         {
//             _context.SaveChanges();
//             return true;
//         }
//         catch (DbUpdateConcurrencyException ex)
//         {
//             var entry = ex.Entries.First();
//             var valoresBd = entry.GetDatabaseValues();
//
//             if (valoresBd is null) return false;
//
//             entry.OriginalValues.SetValues(valoresBd);
//             _context.SaveChanges();
//             return true;
//         }
//     }
//
//     public bool InsertarDosOrdenesConTransaccion(string numero1, string numero2)
//     {
//         using var transaction = _context.Database.BeginTransaction();
//
//         try
//         {
//             _context.OrdenesFabricacion.Add(new OrdenFabricacion { NumeroOrden = numero1, Cliente = "Norte", Estado = "Pendiente", FechaCreacion = DateTime.Now });
//             _context.SaveChanges();
//
//             _context.OrdenesFabricacion.Add(new OrdenFabricacion { NumeroOrden = numero2, Cliente = "Sur", Estado = "Pendiente", FechaCreacion = DateTime.Now });
//             _context.SaveChanges();
//
//             transaction.Commit();
//             return true;
//         }
//         catch (DbUpdateException ex)
//         {
//             transaction.Rollback();
//             _logger.LogError(ex, "Error al insertar las órdenes");
//             return false;
//         }
//     }
//
//     public void AplicarMigracionHasta(string nombreMigracion)
//     {
//         var migrator = _context.Database.GetService<Microsoft.EntityFrameworkCore.Migrations.IMigrator>();
//         migrator.Migrate(nombreMigracion);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 05
// SECTION: Paso 6: Refactorizar UnidadDeTrabajo con IDbContextFactory
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/UnidadDeTrabajo.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Infrastructure.Persistence;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public class UnidadDeTrabajo : IUnidadDeTrabajo
// {
//     private readonly AceriaDbContext _context;
//     private IOrdenRepositorio? _ordenes;
//     private IPlanchaRepositorio? _planchas;
//     private IAleacionRepositorio? _aleaciones;
//     private IDetalleOrdenRepositorio? _detalles;
//
//     public UnidadDeTrabajo(AceriaDbContext context)
//     {
//         _context = context;
//     }
//
//     public IOrdenRepositorio Ordenes => _ordenes ??= new OrdenRepositorio(_context, NullLogger<OrdenRepositorio>.Instance);
//     public IPlanchaRepositorio Planchas => _planchas ??= new PlanchaRepositorio(_context);
//     public IAleacionRepositorio Aleaciones => _aleaciones ??= new AleacionRepositorio(_context);
//     public IDetalleOrdenRepositorio Detalles => _detalles ??= new DetalleOrdenRepositorio(_context);
//
//     public int Guardar()
//     {
//         return _context.SaveChanges();
//     }
//
//     public void Dispose()
//     {
//         _context.Dispose();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 06
// SECTION: Paso 7: Crear un servicio de larga duración con IDbContextFactory
// SOURCE TARGET: Crear el archivo src/AceriaData.Infrastructure/Services/ProcesadorOrdenes.cs:
// ------------------------------------------------------------------------
// using AceriaData.Infrastructure.Persistence;
// using AceriaData.Infrastructure.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Hosting;
//
// namespace AceriaData.Infrastructure.Services;
//
// public class ProcesadorOrdenes : BackgroundService
// {
//     private readonly IDbContextFactory<AceriaDbContext> _factory;
//
//     public ProcesadorOrdenes(IDbContextFactory<AceriaDbContext> factory)
//     {
//         _factory = factory;
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 using var context = await _factory.CreateDbContextAsync(stoppingToken);
//                 using var unidad = new UnidadDeTrabajo(context);
//
//                 var pendientes = unidad.Ordenes.ObtenerPendientes();
//                 Console.WriteLine($"Órdenes pendientes: {pendientes.Count}");
//
//                 await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error en el procesador: {ex.Message}");
//                 await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
//             }
//         }
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 07
// SECTION: Paso 8: Registrar los servicios en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// ------------------------------------------------------------------------
// services.AddDbContextFactory<AceriaDbContext>(options =>
//     options
//         .UseSqlServer(connectionString, sqlOptions =>
//         {
//             sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//             sqlOptions.CommandTimeout(60);
//             sqlOptions.MigrationsAssembly("AceriaData.Infrastructure");
//             // Se conserva la tabla predeterminada __EFMigrationsHistory y el historial heredado.
//         }));
//
// services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
// services.AddScoped<IPlanchaRepositorio, PlanchaRepositorio>();
// services.AddScoped<IAleacionRepositorio, AleacionRepositorio>();
// services.AddScoped<IDetalleOrdenRepositorio, DetalleOrdenRepositorio>();
// services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
// services.AddHostedService<ProcesadorOrdenes>();
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 08
// SECTION: Corrección del benchmark original
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/RepositorioUnidadTrabajoUseCase.cs:
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
//
// namespace AceriaData.Application.UseCases;
//
// public class RepositorioUnidadTrabajoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public RepositorioUnidadTrabajoUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PATRÓN REPOSITORIO Y UNIDAD DE TRABAJO ===");
//
//         DemostrarRepositorioGenerico();
//         DemostrarRepositorioEspecifico();
//         DemostrarUnidadDeTrabajo();
//         CompararRendimientoConYSinRepositorio();
//         CompararMemoriaConYSinRepositorio();
//         MostrarAntiPatrones();
//     }
//
//     private void DemostrarRepositorioGenerico()
//     {
//         Console.WriteLine("\n--- Repositorio genérico ---");
//
//         var todas = _unidad.Ordenes.ObtenerTodas();
//         var porId = _unidad.Ordenes.ObtenerPorId(1);
//
//         Console.WriteLine($"Todas las órdenes: {todas.Count}");
//         Console.WriteLine($"Orden con Id 1: {porId?.NumeroOrden}");
//     }
//
//     private void DemostrarRepositorioEspecifico()
//     {
//         Console.WriteLine("\n--- Repositorio específico ---");
//
//         var pendientes = _unidad.Ordenes.ObtenerPendientes();
//         var resumenes = _unidad.Ordenes.ObtenerResumenes();
//         var total = _unidad.Ordenes.ContarOrdenes();
//
//         Console.WriteLine($"Órdenes pendientes: {pendientes.Count}");
//         Console.WriteLine($"Resúmenes: {resumenes.Count}");
//         Console.WriteLine($"Total de órdenes: {total}");
//     }
//
//     private void DemostrarUnidadDeTrabajo()
//     {
//         Console.WriteLine("\n--- Unidad de trabajo ---");
//
//         var orden = new OrdenFabricacion
//         {
//             NumeroOrden = "OF-UOW-001",
//             Cliente = "Constructora Unidad",
//             Estado = "Pendiente",
//             FechaCreacion = DateTime.Now
//         };
//
//         _unidad.Ordenes.Agregar(orden);
//
//         var plancha = new PlanchaAcero
//         {
//             Orden = orden,
//             Espesor = 10.5,
//             Ancho = 1500,
//             Largo = 3000,
//             Peso = 370.5m,
//             Activa = true
//         };
//
//         _unidad.Planchas.Agregar(plancha);
//
//         var filas = _unidad.Guardar();
//         Console.WriteLine($"Filas afectadas: {filas}");
//         Console.WriteLine($"Orden creada: {orden.NumeroOrden} con Id {orden.Id}");
//     }
//
//     private void CompararRendimientoConYSinRepositorio()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento con y sin repositorio ---");
//
//         var cronometroSin = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             var ordenes = _unidad.Ordenes.ObtenerPendientes();
//         }
//         cronometroSin.Stop();
//
//         var cronometroCon = Stopwatch.StartNew();
//         for (int i = 0; i < 100; i++)
//         {
//             var ordenes = _unidad.Ordenes.ObtenerPendientes();
//         }
//         cronometroCon.Stop();
//
//         Console.WriteLine($"100 consultas con repositorio: {cronometroSin.ElapsedMilliseconds} ms");
//         Console.WriteLine($"100 consultas con repositorio (mismo método): {cronometroCon.ElapsedMilliseconds} ms");
//     }
//
//     private void CompararMemoriaConYSinRepositorio()
//     {
//         Console.WriteLine("\n--- Comparación de memoria con y sin repositorio ---");
//
//         GC.Collect();
//         GC.WaitForPendingFinalizers();
//         GC.Collect();
//
//         var memoriaAntes = GC.GetTotalMemory(true);
//
//         for (int i = 0; i < 1000; i++)
//         {
//             var repositorio = _unidad.Ordenes;
//             var total = repositorio.ContarOrdenes();
//         }
//
//         GC.Collect();
//         GC.WaitForPendingFinalizers();
//         GC.Collect();
//
//         var memoriaDespues = GC.GetTotalMemory(true);
//         var diferencia = (memoriaDespues - memoriaAntes) / 1024;
//
//         Console.WriteLine($"Memoria antes: {memoriaAntes / 1024} KB");
//         Console.WriteLine($"Memoria después: {memoriaDespues / 1024} KB");
//         Console.WriteLine($"Diferencia: {diferencia} KB");
//     }
//
//     private void MostrarAntiPatrones()
//     {
//         Console.WriteLine("\n--- Anti-patrones del patrón Repositorio ---");
//         Console.WriteLine("1. Exponer IQueryable a través de una frontera donde filtra detalles del proveedor o permite composición no controlada.");
//         Console.WriteLine("2. Exponer detalles concretos de EF Core en una interfaz de Application cuando rompe la frontera arquitectónica.");
//         Console.WriteLine("3. Repositorio genérico que fuerza operaciones que el dominio no necesita o no aporta valor arquitectónico.");
//         Console.WriteLine("4. En AceriaData, repositorio que confirma cambios por su cuenta y evita la coordinación de la unidad de trabajo.");
//         Console.WriteLine("5. Devolver entidades desconectadas sin documentar identidad, tracking y estrategia de actualización.");
//         Console.WriteLine();
//         Console.WriteLine("Buenas prácticas:");
//         Console.WriteLine("1. Devolver contratos o formas de datos acordes al caso de uso y a la frontera de Application.");
//         Console.WriteLine("2. Encapsular las consultas específicas del dominio.");
//         Console.WriteLine("3. Añadir solo los métodos que la capa de negocio necesita.");
//         Console.WriteLine("4. En la arquitectura de AceriaData, la unidad de trabajo coordina la confirmación de varios repositorios.");
//         Console.WriteLine("5. Documentar las decisiones de acceso a datos.");
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 09
// SECTION: Paso 10: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// ------------------------------------------------------------------------
// services.AddScoped<RepositorioUnidadTrabajoUseCase>();
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 10
// SECTION: Paso 11: Insertar datos de prueba iniciales
// SOURCE TARGET: Asegurarse de que hay datos en la base de datos. Modificar el método Main para insertar órdenes:
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.Migrate();
//
//     var ordenes = new List<OrdenFabricacion>
//     {
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "EnProceso", FechaCreacion = new DateTime(2024, 2, 20) },
//         new OrdenFabricacion { NumeroOrden = "OF-2024-0003", Cliente = "Constructora del Este", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 3, 10) }
//     };
//
//     context.OrdenesFabricacion.AddRange(ordenes);
//     context.SaveChanges();
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 11
// SECTION: Paso 14: Diagnosticar un error común
// SOURCE TARGET: Modificar el repositorio para exponer IQueryable:
// ------------------------------------------------------------------------
// public interface IOrdenRepositorio : IRepositorio<OrdenFabricacion>
// {
//     IQueryable<OrdenFabricacion> ObtenerQueryable();
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 12
// SECTION: Paso 14: Diagnosticar un error común
// SOURCE TARGET: Paso 14: Diagnosticar un error común
// ------------------------------------------------------------------------
// public interface IOrdenRepositorio : IRepositorio<OrdenFabricacion>
// {
//     List<OrdenFabricacion> ObtenerPendientes();
//     List<OrdenFabricacion> ObtenerPorCliente(string cliente);
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 13
// SECTION: Paso 15: Crear un test con Moq
// SOURCE TARGET: Crear el archivo tests/AceriaData.Tests/Repositories/OrdenRepositorioMockTests.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using Moq;
//
// namespace AceriaData.Tests.Repositories;
//
// public class OrdenRepositorioMockTests
// {
//     [Fact]
//     public void ObtenerPendientes_ConVariasOrdenes_DevuelveSoloLasPendientes()
//     {
//         var mockRepositorio = new Mock<IOrdenRepositorio>();
//         mockRepositorio.Setup(r => r.ObtenerTodas()).Returns(new List<OrdenFabricacion>
//         {
//             new OrdenFabricacion { NumeroOrden = "OF-001", Estado = "Pendiente" },
//             new OrdenFabricacion { NumeroOrden = "OF-002", Estado = "EnProceso" },
//             new OrdenFabricacion { NumeroOrden = "OF-003", Estado = "Pendiente" }
//         });
//
//         var ordenes = mockRepositorio.Object.ObtenerTodas().Where(o => o.Estado == "Pendiente").ToList();
//
//         Assert.Equal(2, ordenes.Count);
//     }
//
//     [Fact]
//     public void Agregar_ConOrdenValida_LlamaAlMetodoDelRepositorio()
//     {
//         var mockRepositorio = new Mock<IOrdenRepositorio>();
//         var orden = new OrdenFabricacion { NumeroOrden = "OF-001", Estado = "Pendiente" };
//
//         mockRepositorio.Object.Agregar(orden);
//
//         mockRepositorio.Verify(r => r.Agregar(orden), Times.Once);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 14
// SECTION: Paso 1: Crear la interfaz IDetalleOrdenRepositorio:
// SOURCE TARGET: Paso 1: Crear la interfaz IDetalleOrdenRepositorio:
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
//
// namespace AceriaData.Application.Interfaces;
//
// public interface IDetalleOrdenRepositorio : IRepositorio<DetalleOrden>
// {
//     DetalleOrden? ObtenerPorOrden(int ordenId);
//     List<DetalleOrden> ObtenerConTemperaturaMayorA(double temperatura);
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 15
// SECTION: Paso 2: Crear la implementación DetalleOrdenRepositorio:
// SOURCE TARGET: Paso 2: Crear la implementación DetalleOrdenRepositorio:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public class DetalleOrdenRepositorio : Repositorio<DetalleOrden>, IDetalleOrdenRepositorio
// {
//     public DetalleOrdenRepositorio(AceriaDbContext context) : base(context) { }
//
//     public DetalleOrden? ObtenerPorOrden(int ordenId)
//     {
//         return _context.DetallesOrden
//             .FirstOrDefault(d => d.OrdenId == ordenId);
//     }
//
//     public List<DetalleOrden> ObtenerConTemperaturaMayorA(double temperatura)
//     {
//         return _context.DetallesOrden
//             .Where(d => d.TemperaturaColada > temperatura)
//             .ToList();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 16
// SECTION: Paso 3: Añadir el repositorio a la unidad de trabajo:
// SOURCE TARGET: Paso 3: Añadir el repositorio a la unidad de trabajo:
// ------------------------------------------------------------------------
// public interface IUnidadDeTrabajo : IDisposable
// {
//     IOrdenRepositorio Ordenes { get; }
//     IPlanchaRepositorio Planchas { get; }
//     IAleacionRepositorio Aleaciones { get; }
//     IDetalleOrdenRepositorio Detalles { get; }
//     int Guardar();
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 17
// SECTION: Paso 4: Implementar el repositorio en la unidad de trabajo:
// SOURCE TARGET: Paso 4: Implementar el repositorio en la unidad de trabajo:
// ------------------------------------------------------------------------
// public class UnidadDeTrabajo : IUnidadDeTrabajo
// {
//     private readonly AceriaDbContext _context;
//     private IOrdenRepositorio? _ordenes;
//     private IPlanchaRepositorio? _planchas;
//     private IAleacionRepositorio? _aleaciones;
//     private IDetalleOrdenRepositorio? _detalles;
//
//     public UnidadDeTrabajo(AceriaDbContext context)
//     {
//         _context = context;
//     }
//
//     public IOrdenRepositorio Ordenes => _ordenes ??= new OrdenRepositorio(_context, NullLogger<OrdenRepositorio>.Instance);
//     public IPlanchaRepositorio Planchas => _planchas ??= new PlanchaRepositorio(_context);
//     public IAleacionRepositorio Aleaciones => _aleaciones ??= new AleacionRepositorio(_context);
//     public IDetalleOrdenRepositorio Detalles => _detalles ??= new DetalleOrdenRepositorio(_context);
//
//     public int Guardar() => _context.SaveChanges();
//
//     public void Dispose() => _context.Dispose();
// }
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 18
// SECTION: Paso 5: Registrar el repositorio en el contenedor:
// SOURCE TARGET: Paso 5: Registrar el repositorio en el contenedor:
// ------------------------------------------------------------------------
// services.AddScoped<IDetalleOrdenRepositorio, DetalleOrdenRepositorio>();
// ========================================================================

// CANONICAL PDF M05 5.9 - BLOCK 19
// SECTION: Paso 6: Crear el test con Moq:
// SOURCE TARGET: Paso 6: Crear el test con Moq:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using Moq;
//
// namespace AceriaData.Tests.Repositories;
//
// public class DetalleOrdenRepositorioMockTests
// {
//     [Fact]
//     public void ObtenerPorOrden_ConDetalleExistente_DevuelveElDetalle()
//     {
//         var mockRepositorio = new Mock<IDetalleOrdenRepositorio>();
//         mockRepositorio.Setup(r => r.ObtenerPorOrden(1)).Returns(new DetalleOrden
//         {
//             Id = 1,
//             OrdenId = 1,
//             ComposicionQuimica = "C: 0.45%, Mn: 0.75%",
//             TemperaturaColada = 1550.5
//         });
//
//         var detalle = mockRepositorio.Object.ObtenerPorOrden(1);
//
//         Assert.NotNull(detalle);
//         Assert.Equal(1550.5, detalle.TemperaturaColada);
//     }
//
//     [Fact]
//     public void ObtenerPorOrden_ConOrdenInexistente_DevuelveNull()
//     {
//         var mockRepositorio = new Mock<IDetalleOrdenRepositorio>();
//         mockRepositorio.Setup(r => r.ObtenerPorOrden(99999)).Returns((DetalleOrden?)null);
//
//         var detalle = mockRepositorio.Object.ObtenerPorOrden(99999);
//
//         Assert.Null(detalle);
//     }
// }
// ========================================================================

