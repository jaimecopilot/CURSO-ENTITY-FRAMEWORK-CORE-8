// ========================================================================
// M05 5.11 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 01
// SECTION: Paso 4: Crear la clase base de pruebas
// SOURCE TARGET: Crear el archivo tests/AceriaData.Tests/Base/TestBase.cs:
// ------------------------------------------------------------------------
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Tests.Base;
//
// public abstract class TestBase : IDisposable
// {
//     protected readonly AceriaDbContext Context;
//     private readonly SqliteConnection _connection;
//
//     protected TestBase()
//     {
//         _connection = new SqliteConnection("Data Source=:memory:");
//         _connection.Open();
//
//         var options = new DbContextOptionsBuilder<AceriaDbContext>()
//             .UseSqlite(_connection)
//             .Options;
//
//         Context = new AceriaDbContext(options);
//         Context.Database.EnsureCreated();
//     }
//
//     public void Dispose()
//     {
//         Context.Dispose();
//         _connection.Dispose();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 02
// SECTION: Paso 5: Crear tests del repositorio de órdenes
// SOURCE TARGET: Crear el archivo tests/AceriaData.Tests/Repositories/OrdenRepositorioTests.cs:
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Repositories;
// using AceriaData.Tests.Base;
// using Microsoft.Extensions.Logging.Abstractions;
//
// namespace AceriaData.Tests.Repositories;
//
// public class OrdenRepositorioTests : TestBase
// {
//     private readonly OrdenRepositorio _repositorio;
//
//     public OrdenRepositorioTests()
//     {
//         _repositorio = new OrdenRepositorio(Context, NullLogger<OrdenRepositorio>.Instance);
//     }
//
//     [Fact]
//     public void ObtenerPorId_ConIdExistente_DevuelveLaOrden()
//     {
//         var orden = new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = DateTime.Now };
//         Context.OrdenesFabricacion.Add(orden);
//         Context.SaveChanges();
//
//         var recuperada = _repositorio.ObtenerPorId(orden.Id);
//
//         Assert.NotNull(recuperada);
//         Assert.Equal("OF-001", recuperada.NumeroOrden);
//     }
//
//     [Fact]
//     public void ObtenerPorId_ConIdInexistente_DevuelveNull()
//     {
//         var recuperada = _repositorio.ObtenerPorId(99999);
//
//         Assert.Null(recuperada);
//     }
//
//     [Fact]
//     public void ObtenerPendientes_ConVariasOrdenes_DevuelveSoloLasPendientes()
//     {
//         Context.OrdenesFabricacion.AddRange(
//             new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "A", Estado = "Pendiente", FechaCreacion = DateTime.Now },
//             new OrdenFabricacion { NumeroOrden = "OF-002", Cliente = "B", Estado = "EnProceso", FechaCreacion = DateTime.Now },
//             new OrdenFabricacion { NumeroOrden = "OF-003", Cliente = "C", Estado = "Pendiente", FechaCreacion = DateTime.Now });
//         Context.SaveChanges();
//
//         var pendientes = _repositorio.ObtenerPendientes();
// Assert.Equal(2, pendientes.Count);
//     }
//
//     [Fact]
//     public void ContarOrdenes_ConVariasOrdenes_DevuelveElTotal()
//     {
//         Context.OrdenesFabricacion.AddRange(
//             new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "A", Estado = "Pendiente", FechaCreacion = DateTime.Now },
//             new OrdenFabricacion { NumeroOrden = "OF-002", Cliente = "B", Estado = "EnProceso", FechaCreacion = DateTime.Now });
//         Context.SaveChanges();
//
//         var total = _repositorio.ContarOrdenes();
//
//         Assert.Equal(2, total);
//     }
//
//     [Fact]
//     public void ExisteAlgunaOrden_SinOrdenes_DevuelveFalse()
//     {
//         var existe = _repositorio.ExisteAlgunaOrden();
//
//         Assert.False(existe);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 03
// SECTION: Paso 6: Crear tests de la unidad de trabajo
// SOURCE TARGET: Crear el archivo tests/AceriaData.Tests/Repositories/UnidadDeTrabajoTests.cs:
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Repositories;
// using AceriaData.Tests.Base;
//
// namespace AceriaData.Tests.Repositories;
//
// public class UnidadDeTrabajoTests : TestBase
// {
//     [Fact]
//     public void Guardar_ConOrdenYPlancha_InsertaAmbasEnUnaTransaccion()
//     {
//         using var unidad = new UnidadDeTrabajo(Context);
//
//         var orden = new OrdenFabricacion
//         {
//             NumeroOrden = "OF-UOW-001",
//             Cliente = "Constructora del Norte",
//             Estado = "Pendiente",
//             FechaCreacion = DateTime.Now
//         };
//         unidad.Ordenes.Agregar(orden);
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
//         unidad.Planchas.Agregar(plancha);
//
//         var filas = unidad.Guardar();
//
//         Assert.Equal(2, filas);
//         Assert.NotEqual(0, orden.Id);
//         Assert.NotEqual(0, plancha.Id);
//     }
//
//     [Fact]
//     public void Guardar_SinCambios_DevuelveCero()
//     {
//         using var unidad = new UnidadDeTrabajo(Context);
//
//         var filas = unidad.Guardar();
//
//         Assert.Equal(0, filas);
//     }
//
//     [Fact]
//     public void Guardar_ConDosRepositorios_CompartenElMismoContexto()
//     {
//         using var unidad = new UnidadDeTrabajo(Context);
//
//         var orden = new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "A", Estado = "Pendiente", FechaCreacion = DateTime.Now };
//         unidad.Ordenes.Agregar(orden);
//         unidad.Guardar();
//
//         var plancha = new PlanchaAcero { OrdenId = orden.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//         unidad.Planchas.Agregar(plancha);
//         var filas = unidad.Guardar();
//
//         Assert.Equal(1, filas);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 04
// SECTION: Paso 7: Crear tests con InMemory
// SOURCE TARGET: Crear el archivo tests/AceriaData.Tests/InMemory/OrdenRepositorioInMemoryTests.cs:
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using AceriaData.Infrastructure.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging.Abstractions;
//
// namespace AceriaData.Tests.InMemory;
//
// public class OrdenRepositorioInMemoryTests : IDisposable
// {
//     private readonly AceriaDbContext _context;
//     private readonly OrdenRepositorio _repositorio;
//
//     public OrdenRepositorioInMemoryTests()
//     {
//         var options = new DbContextOptionsBuilder<AceriaDbContext>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options;
//
//         _context = new AceriaDbContext(options);
//         _repositorio = new OrdenRepositorio(_context, NullLogger<OrdenRepositorio>.Instance);
//     }
//
//     [Fact]
//     public void InsertarOrden_ConDatosValidos_SeGuardaEnMemoria()
//     {
//         var orden = new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = DateTime.Now };
//         _repositorio.Agregar(orden);
//         _context.SaveChanges();
//
//         var recuperada = _repositorio.ObtenerPorNumeroOrden("OF-001");
//
//         Assert.NotNull(recuperada);
//         Assert.Equal("Constructora del Norte", recuperada.Cliente);
//     }
//
//     [Fact]
//     public void InsertarPlanchaConOrdenInvalida_NoLanzaExcepcion()
//     {
//         var plancha = new PlanchaAcero { OrdenId = 99999, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//         _context.PlanchasAcero.Add(plancha);
//
//         var excepcion = Record.Exception(() => _context.SaveChanges());
//
//         Assert.Null(excepcion);
//     }
//
//     public void Dispose()
//     {
//         _context.Dispose();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 05
// SECTION: Paso 9: Crear tests con SQLite en memoria
// SOURCE TARGET: Crear el archivo tests/AceriaData.Tests/Sqlite/OrdenRepositorioSqliteTests.cs:
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Repositories;
// using AceriaData.Tests.Base;
//
// namespace AceriaData.Tests.Sqlite;
//
// public class OrdenRepositorioSqliteTests : TestBase
// {
//     private readonly OrdenRepositorio _repositorio;
//
//     public OrdenRepositorioSqliteTests()
//     {
//         _repositorio = new OrdenRepositorio(Context, Microsoft.Extensions.Logging.Abstractions.NullLogger<OrdenRepositorio>.Instance);
//     }
//
//     [Fact]
//     public void InsertarOrden_ConDatosValidos_SeGuardaEnSqlite()
//     {
//         var orden = new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = DateTime.Now };
//         _repositorio.Agregar(orden);
//         Context.SaveChanges();
//
//         var recuperada = _repositorio.ObtenerPorNumeroOrden("OF-001");
//
//         Assert.NotNull(recuperada);
//         Assert.Equal("Constructora del Norte", recuperada.Cliente);
//     }
//
//     [Fact]
//     public void InsertarPlanchaConOrdenInvalida_LanzaExcepcion()
//     {
//         var plancha = new PlanchaAcero { OrdenId = 99999, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//         Context.PlanchasAcero.Add(plancha);
//
//         var excepcion = Record.Exception(() => Context.SaveChanges());
//
//         Assert.NotNull(excepcion);
//     }
//
//     [Fact]
//     public void InsertarOrdenConNumeroDuplicado_LanzaExcepcion()
//     {
//         Context.OrdenesFabricacion.Add(new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "A", Estado = "Pendiente", FechaCreacion = DateTime.Now });
//         Context.SaveChanges();
//
//         Context.OrdenesFabricacion.Add(new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "B", Estado = "Pendiente", FechaCreacion = DateTime.Now });
//
//         var excepcion = Record.Exception(() => Context.SaveChanges());
//
//         Assert.NotNull(excepcion);
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 06
// SECTION: Paso 12: Diagnosticar un error común
// SOURCE TARGET: Modificar el test InsertarPlanchaConOrdenInvalida_NoLanzaExcepcion para ejecutarlo con SQLite en memoria:
// ------------------------------------------------------------------------
// [Fact]
// public void InsertarPlanchaConOrdenInvalida_ConSqlite_LanzaExcepcion()
// {
//     var plancha = new PlanchaAcero { OrdenId = 99999, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     Context.PlanchasAcero.Add(plancha);
//
//     var excepcion = Record.Exception(() => Context.SaveChanges());
//
//     Assert.NotNull(excepcion);
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 07
// SECTION: Paso 1: Crear el test en tests/AceriaData.Tests/Sqlite/ConcurrenciaSqliteTests.cs:
// SOURCE TARGET: ### Paso 1: Crear el test en tests/AceriaData.Tests/Sqlite/ConcurrenciaSqliteTests.cs:
// ------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Tests.Sqlite;
//
// public class ConcurrenciaSqliteTests : IDisposable
// {
//     private readonly SqliteConnection _connection;
//
//     public ConcurrenciaSqliteTests()
//     {
//         _connection = new SqliteConnection("Data Source=:memory:");
//         _connection.Open();
//
//         using var context = CrearContexto();
//         context.Database.EnsureCreated();
//
//         context.OrdenesFabricacion.Add(new OrdenFabricacion { NumeroOrden = "OF-001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = DateTime.Now });
//         context.SaveChanges();
//     }
//
//     private AceriaDbContext CrearContexto()
//     {
//         var options = new DbContextOptionsBuilder<AceriaDbContext>()
//             .UseSqlite(_connection)
//             .Options;
//         return new AceriaDbContext(options);
//     }
//
//     [Fact]
//     public void ActualizarOrden_ConDosContextos_LanzaExcepcionDeConcurrencia()
//     {
//         using var contextA = CrearContexto();
//         using var contextB = CrearContexto();
//
//         var ordenA = contextA.OrdenesFabricacion.First(o => o.NumeroOrden == "OF-001");
//         var ordenB = contextB.OrdenesFabricacion.First(o => o.NumeroOrden == "OF-001");
//
//         ordenA.Cliente = "Constructora del Norte Actualizada";
//         contextA.SaveChanges();
//
//         ordenB.Estado = "EnProceso";
//
//         var excepcion = Record.Exception(() => contextB.SaveChanges());
//
//         Assert.NotNull(excepcion);
//         Assert.IsType<DbUpdateConcurrencyException>(excepcion);
//     }
//
//     public void Dispose()
//     {
//         _connection.Dispose();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 08
// SECTION: Paso 13: Añadir unit tests de Application con xUnit + Moq, sin EF Core
// SOURCE TARGET: Paso 13: Añadir unit tests de Application con xUnit + Moq, sin EF Core
// ------------------------------------------------------------------------
// var repo = new Mock<IOrdenRepositorio>();
// repo.Setup(r => r.ObtenerPorNumero("OF-UNIT-001"))
//     .Returns(new OrdenFabricacion
//     {
//         NumeroOrden = "OF-UNIT-001",
//         Cliente = "Cliente unitario",
//         Estado = "Pendiente"
//     });
//
// var sut = new ConsultarOrdenUseCase(repo.Object);
// var resultado = sut.Ejecutar("OF-UNIT-001");
//
// Assert.Equal("Cliente unitario", resultado.Cliente);
// repo.Verify(r => r.ObtenerPorNumero("OF-UNIT-001"), Times.Once);
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 09
// SECTION: Paso 15: Crear una `DatabaseFixture` contra SQL Server LocalDB real
// SOURCE TARGET: Paso 15: Crear una `DatabaseFixture` contra SQL Server LocalDB real
// ------------------------------------------------------------------------
// public sealed class SqlServerDatabaseFixture : IAsyncLifetime
// {
//     private readonly string _dbName = $"AceriaData_Test_{Guid.NewGuid():N}";
//     public string ConnectionString =>
//         $"Server=(localdb)\\MSSQLLocalDB;Database={_dbName};Trusted_Connection=True;TrustServerCertificate=True";
//
//     public async Task InitializeAsync()
//     {
//         var options = new DbContextOptionsBuilder<AceriaDbContext>()
//             .UseSqlServer(ConnectionString)
//             .Options;
//
//         await using var context = new AceriaDbContext(options);
//         await context.Database.MigrateAsync();
//     }
//
//     public async Task DisposeAsync()
//     {
//         var options = new DbContextOptionsBuilder<AceriaDbContext>()
//             .UseSqlServer(ConnectionString)
//             .Options;
//         await using var context = new AceriaDbContext(options);
//         await context.Database.EnsureDeletedAsync();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 10
// SECTION: Paso 16: Usar Respawn para restaurar los datos entre tests
// SOURCE TARGET: Paso 16: Usar Respawn para restaurar los datos entre tests
// ------------------------------------------------------------------------
// await using var connection = new SqlConnection(fixture.ConnectionString);
// await connection.OpenAsync();
//
// var checkpoint = await Respawner.CreateAsync(connection, new RespawnerOptions
// {
//     DbAdapter = DbAdapter.SqlServer,
//     TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
// });
//
// await checkpoint.ResetAsync(connection);
// ========================================================================

// CANONICAL PDF M05 5.11 - BLOCK 11
// SECTION: Paso 17: Añadir un host `AceriaData.Api` mínimo y probarlo con `WebApplicationFactory<Program>`
// SOURCE TARGET: Paso 17: Añadir un host `AceriaData.Api` mínimo y probarlo con `WebApplicationFactory<Program>`
// ------------------------------------------------------------------------
// public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
// {
//     private readonly HttpClient _client;
//
//     public ApiIntegrationTests(WebApplicationFactory<Program> factory)
//     {
//         _client = factory.CreateClient();
//     }
//
//     [Fact]
//     public async Task GetOrdenes_DevuelveSuccess()
//     {
//         var response = await _client.GetAsync("/api/ordenes");
//         response.EnsureSuccessStatusCode();
//     }
// }
// ========================================================================

