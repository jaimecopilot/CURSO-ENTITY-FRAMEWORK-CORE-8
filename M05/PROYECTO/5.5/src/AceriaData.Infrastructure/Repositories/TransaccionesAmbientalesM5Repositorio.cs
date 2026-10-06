using System.Transactions;
using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace AceriaData.Infrastructure.Repositories;

public sealed class TransaccionesAmbientalesM5Repositorio : ITransaccionesAmbientalesM5Repositorio
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<string> _eventosExternos = new();

    public TransaccionesAmbientalesM5Repositorio(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<TransaccionAmbientalM5Dto> DemostrarDosContextosAsync()
    {
        const string numero = "OF-M5-55-AMBIENT";
        Limpiar(numero);

        var options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(30)
        };

        SqlCommandCounterInterceptor.Instance.Reset();

        bool ambienteActivo;
        bool flujoAsync;
        bool promocion;
        string aislamiento;
        int ordenId;

        using (var scope = new TransactionScope(
                   TransactionScopeOption.Required,
                   options,
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            await using var connection = new SqlConnection(ObtenerConnectionString());
            await connection.OpenAsync();

            ambienteActivo = Transaction.Current is not null;
            aislamiento = Transaction.Current?.IsolationLevel.ToString() ?? "<none>";

            await using (var contextOrden = CrearContexto(connection))
            {
                var orden = CrearOrden(numero);
                contextOrden.OrdenesFabricacion.Add(orden);
                await contextOrden.SaveChangesAsync();
                ordenId = orden.Id;
            }

            await Task.Yield();
            flujoAsync = Transaction.Current is not null;

            await using (var contextPlancha = CrearContexto(connection))
            {
                contextPlancha.PlanchasAcero.Add(new PlanchaAcero
                {
                    OrdenId = ordenId,
                    Espesor = 10,
                    Ancho = 1000,
                    Largo = 2000,
                    Peso = 150m,
                    Activa = true
                });

                await contextPlancha.SaveChangesAsync();
            }

            promocion =
                Transaction.Current?.TransactionInformation.DistributedIdentifier != Guid.Empty;

            scope.Complete();
        }

        var comandos = SqlCommandCounterInterceptor.Instance.SnapshotCommands();
        var persistido = ExisteOrdenYPlancha(numero);

        return new TransaccionAmbientalM5Dto(
            "Dos DbContext con una conexión compartida",
            persistido,
            ambienteActivo,
            flujoAsync,
            promocion,
            aislamiento,
            comandos);
    }

    public async Task<TransaccionAmbientalM5Dto> DemostrarRollbackSinCompleteAsync()
    {
        const string numero = "OF-M5-55-NOCOMPLETE";
        Limpiar(numero);

        var options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(30)
        };

        SqlCommandCounterInterceptor.Instance.Reset();

        bool ambienteActivo;
        bool flujoAsync;
        bool promocion;
        string aislamiento;

        using (var scope = new TransactionScope(
                   TransactionScopeOption.Required,
                   options,
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            await using var connection = new SqlConnection(ObtenerConnectionString());
            await connection.OpenAsync();

            ambienteActivo = Transaction.Current is not null;
            aislamiento = Transaction.Current?.IsolationLevel.ToString() ?? "<none>";

            await using var context = CrearContexto(connection);
            context.OrdenesFabricacion.Add(CrearOrden(numero));
            await context.SaveChangesAsync();

            await Task.Yield();
            flujoAsync = Transaction.Current is not null;
            promocion =
                Transaction.Current?.TransactionInformation.DistributedIdentifier != Guid.Empty;

            // Intencionadamente NO se llama a Complete().
        }

        var comandos = SqlCommandCounterInterceptor.Instance.SnapshotCommands();
        var persistido = Existe(numero);

        return new TransaccionAmbientalM5Dto(
            "Scope sin Complete",
            persistido,
            ambienteActivo,
            flujoAsync,
            promocion,
            aislamiento,
            comandos);
    }

    public OpcionesTransactionScopeM5Dto DemostrarOpcionesDeScope()
    {
        string outerId;
        bool requiredMisma;
        bool requiresNewOtra;
        bool suppressSinAmbiente;
        string aislamiento;

        using (var outer = new TransactionScope())
        {
            var outerTransaction = Transaction.Current
                ?? throw new InvalidOperationException("No se creó la transacción ambiental.");

            outerId = outerTransaction.TransactionInformation.LocalIdentifier;
            aislamiento = outerTransaction.IsolationLevel.ToString();

            using (var required = new TransactionScope(TransactionScopeOption.Required))
            {
                requiredMisma =
                    Transaction.Current?.TransactionInformation.LocalIdentifier == outerId;
                required.Complete();
            }

            using (var requiresNew = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                requiresNewOtra =
                    Transaction.Current?.TransactionInformation.LocalIdentifier != outerId;
                requiresNew.Complete();
            }

            using (var suppressed = new TransactionScope(TransactionScopeOption.Suppress))
            {
                suppressSinAmbiente = Transaction.Current is null;
                suppressed.Complete();
            }

            outer.Complete();
        }

        return new OpcionesTransactionScopeM5Dto(
            requiredMisma,
            requiresNewOtra,
            suppressSinAmbiente,
            aislamiento,
            TransactionManager.DefaultTimeout.TotalSeconds);
    }

    public EfectoExternoM5Dto DemostrarRecursoExternoNoTransaccional()
    {
        const string numero = "OF-M5-55-EXTERNO";
        Limpiar(numero);
        _eventosExternos.Remove(numero);

        var options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(30)
        };

        using (var scope = new TransactionScope(
                   TransactionScopeOption.Required,
                   options,
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            using var diScope = _scopeFactory.CreateScope();
            var context = diScope.ServiceProvider.GetRequiredService<AceriaDbContext>();

            context.OrdenesFabricacion.Add(CrearOrden(numero));
            context.SaveChanges();

            // Simula un efecto externo normal que NO participa en System.Transactions.
            _eventosExternos.Add(numero);

            // Se simula un fallo posterior omitiendo Complete().
        }

        return new EfectoExternoM5Dto(
            Existe(numero),
            _eventosExternos.Contains(numero));
    }

    public AislamientoM5Dto DemostrarReadCommitted() =>
        EjecutarConAislamiento(
            "ReadCommitted",
            IsolationLevel.ReadCommitted,
            "OF-M5-55-RC");

    public AislamientoM5Dto DemostrarSnapshot()
    {
        HabilitarSnapshot();

        return EjecutarConAislamiento(
            "Snapshot",
            IsolationLevel.Snapshot,
            "OF-M5-55-SNAPSHOT");
    }

    private AislamientoM5Dto EjecutarConAislamiento(
        string nombre,
        IsolationLevel isolationLevel,
        string numero)
    {
        Limpiar(numero);

        var options = new TransactionOptions
        {
            IsolationLevel = isolationLevel,
            Timeout = TimeSpan.FromSeconds(30)
        };

        string observado;

        using (var scope = new TransactionScope(
                   TransactionScopeOption.Required,
                   options,
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            observado = Transaction.Current?.IsolationLevel.ToString() ?? "<none>";

            using var diScope = _scopeFactory.CreateScope();
            var context = diScope.ServiceProvider.GetRequiredService<AceriaDbContext>();

            context.OrdenesFabricacion.Add(CrearOrden(numero));
            context.SaveChanges();

            scope.Complete();
        }

        return new AislamientoM5Dto(
            nombre,
            observado,
            Existe(numero));
    }

    private AceriaDbContext CrearContexto(SqlConnection connection)
    {
        var options = new DbContextOptionsBuilder<AceriaDbContext>()
            .UseSqlServer(connection)
            .EnableDetailedErrors()
            .AddInterceptors(SqlCommandCounterInterceptor.Instance)
            .Options;

        return new AceriaDbContext(options);
    }

    private string ObtenerConnectionString()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

        return context.Database.GetConnectionString()
            ?? throw new InvalidOperationException("No hay cadena de conexión.");
    }

    private void HabilitarSnapshot()
    {
        var builder = new SqlConnectionStringBuilder(ObtenerConnectionString());
        var database = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            $"ALTER DATABASE [{database.Replace("]", "]]")}] SET ALLOW_SNAPSHOT_ISOLATION ON;";
        command.ExecuteNonQuery();
    }

    private static OrdenFabricacion CrearOrden(string numero) =>
        new()
        {
            NumeroOrden = numero,
            Cliente = "Cliente TransactionScope 5.5",
            Estado = "Pendiente",
            FechaCreacion = new DateTime(2026, 9, 30)
        };

    private bool Existe(string numero)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

        return context.OrdenesFabricacion
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Any(o => o.NumeroOrden == numero);
    }

    private bool ExisteOrdenYPlancha(string numero)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

        return context.OrdenesFabricacion
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Any(o => o.NumeroOrden == numero && o.Planchas.Any());
    }

    private void Limpiar(params string[] numeros)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var filas = context.OrdenesFabricacion
            .IgnoreQueryFilters()
            .Where(o => numeros.Contains(o.NumeroOrden))
            .ToList();

        if (filas.Count == 0)
            return;

        context.RemoveRange(filas);
        context.SaveChanges();
    }

    /*
    // RETO M05 5.5 - SUPPRESS FUERA DEL ROLLBACK AMBIENTAL
    public SuppressFueraAmbienteM5Dto DemostrarSuppressFueraDeRollback()
    {
        const string numeroAmbiental = "OF-M5-55-SUPPRESS-IN";
        const string numeroSuprimido = "OF-M5-55-SUPPRESS-OUT";
        Limpiar(numeroAmbiental, numeroSuprimido);

        bool suppressSinAmbiente;

        using (var outer = new TransactionScope(
                   TransactionScopeOption.Required,
                   new TransactionOptions
                   {
                       IsolationLevel = IsolationLevel.ReadCommitted,
                       Timeout = TimeSpan.FromSeconds(30)
                   },
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            using (var diScope = _scopeFactory.CreateScope())
            {
                var context = diScope.ServiceProvider.GetRequiredService<AceriaDbContext>();
                context.OrdenesFabricacion.Add(CrearOrden(numeroAmbiental));
                context.SaveChanges();
            }

            using (var suppressed = new TransactionScope(TransactionScopeOption.Suppress))
            {
                suppressSinAmbiente = Transaction.Current is null;

                using var diScope = _scopeFactory.CreateScope();
                var context = diScope.ServiceProvider.GetRequiredService<AceriaDbContext>();
                context.OrdenesFabricacion.Add(CrearOrden(numeroSuprimido));
                context.SaveChanges();

                suppressed.Complete();
            }

            // Intencionadamente no se llama a outer.Complete().
        }

        return new SuppressFueraAmbienteM5Dto(
            Existe(numeroAmbiental),
            Existe(numeroSuprimido),
            suppressSinAmbiente);
    }
    */

}
