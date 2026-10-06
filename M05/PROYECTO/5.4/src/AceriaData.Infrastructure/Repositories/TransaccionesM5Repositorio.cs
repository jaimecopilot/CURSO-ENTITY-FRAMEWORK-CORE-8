using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AceriaData.Infrastructure.Repositories;

public sealed class TransaccionesM5Repositorio : ITransaccionesM5Repositorio
{
    private const string NumeroExistente = "OF-2024-0001";
    private readonly IServiceScopeFactory _scopeFactory;

    public TransaccionesM5Repositorio(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ResultadoTransaccionM5Dto DemostrarAtomicidadSaveChanges()
    {
        const string numeroValido = "OF-M5-54-ATOMIC";
        Limpiar(numeroValido);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var mars = MarsHabilitado(context);

        SqlCommandCounterInterceptor.Instance.Reset();

        context.OrdenesFabricacion.Add(CrearOrden(numeroValido));
        context.OrdenesFabricacion.Add(CrearOrden(NumeroExistente));

        var falloEsperado = false;
        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateException)
        {
            falloEsperado = true;
            context.ChangeTracker.Clear();
        }

        var existeValida = Existe(numeroValido);

        return new ResultadoTransaccionM5Dto(
            "Atomicidad de un único SaveChanges",
            falloEsperado && !existeValida,
            existeValida,
            Existe(NumeroExistente),
            mars,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    public ResultadoTransaccionM5Dto DemostrarCommitExplicito()
    {
        const string numero1 = "OF-M5-54-COMMIT-1";
        const string numero2 = "OF-M5-54-COMMIT-2";
        Limpiar(numero1, numero2);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var mars = MarsHabilitado(context);

        SqlCommandCounterInterceptor.Instance.Reset();

        using (var transaction = context.Database.BeginTransaction())
        {
            context.OrdenesFabricacion.Add(CrearOrden(numero1));
            context.SaveChanges();

            context.OrdenesFabricacion.Add(CrearOrden(numero2));
            context.SaveChanges();

            transaction.Commit();
        }

        var existe1 = Existe(numero1);
        var existe2 = Existe(numero2);

        return new ResultadoTransaccionM5Dto(
            "Transacción explícita con Commit",
            existe1 && existe2,
            existe1,
            existe2,
            mars,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    public ResultadoTransaccionM5Dto DemostrarRollbackExplicito()
    {
        const string numero1 = "OF-M5-54-ROLLBACK-1";
        Limpiar(numero1);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var mars = MarsHabilitado(context);

        SqlCommandCounterInterceptor.Instance.Reset();

        var falloEsperado = false;

        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                context.OrdenesFabricacion.Add(CrearOrden(numero1));
                context.SaveChanges();

                context.OrdenesFabricacion.Add(CrearOrden(NumeroExistente));
                context.SaveChanges();

                transaction.Commit();
            }
            catch (DbUpdateException)
            {
                falloEsperado = true;
                transaction.Rollback();
                context.ChangeTracker.Clear();
            }
        }

        var existe1 = Existe(numero1);

        return new ResultadoTransaccionM5Dto(
            "Transacción explícita con Rollback",
            falloEsperado && !existe1,
            existe1,
            Existe(NumeroExistente),
            mars,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    public ResultadoTransaccionM5Dto DemostrarRollbackASavepoint()
    {
        const string numero1 = "OF-M5-54-SP-1";
        const string numero2 = "OF-M5-54-SP-2";
        const string savepoint = "AntesSegundaOrden";
        Limpiar(numero1, numero2);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var mars = MarsHabilitado(context);

        if (mars)
        {
            throw new InvalidOperationException(
                "La conexión de 5.4 debe tener MultipleActiveResultSets=false para demostrar savepoints.");
        }

        SqlCommandCounterInterceptor.Instance.Reset();

        using (var transaction = context.Database.BeginTransaction())
        {
            context.OrdenesFabricacion.Add(CrearOrden(numero1));
            context.SaveChanges();

            transaction.CreateSavepoint(savepoint);

            context.OrdenesFabricacion.Add(CrearOrden(numero2));
            context.SaveChanges();

            transaction.RollbackToSavepoint(savepoint);

            context.ChangeTracker.Clear();
            transaction.Commit();
        }

        var existe1 = Existe(numero1);
        var existe2 = Existe(numero2);

        return new ResultadoTransaccionM5Dto(
            "Rollback a savepoint",
            existe1 && !existe2,
            existe1,
            existe2,
            mars,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    private static OrdenFabricacion CrearOrden(string numero) =>
        new()
        {
            NumeroOrden = numero,
            Cliente = "Cliente transacciones 5.4",
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

    private static bool MarsHabilitado(AceriaDbContext context)
    {
        var connectionString = context.Database.GetConnectionString()
            ?? throw new InvalidOperationException("No hay cadena de conexión.");

        return new SqlConnectionStringBuilder(connectionString).MultipleActiveResultSets;
    }

    /*
    // RETO M05 5.4 - TRES SAVECHANGES TRAS SAVEPOINT
    public SavepointTresGuardadosM5Dto DemostrarTresSaveChangesConRollbackParcial()
    {
        const string numero1 = "OF-M5-54-RETO-1";
        const string numero2 = "OF-M5-54-RETO-2";
        const string numero3 = "OF-M5-54-RETO-3";
        const string savepoint = "AntesCambiosPosteriores";
        Limpiar(numero1, numero2, numero3);

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var mars = MarsHabilitado(context);
        if (mars)
            throw new InvalidOperationException("Reto 5.4 requiere MultipleActiveResultSets=false.");

        SqlCommandCounterInterceptor.Instance.Reset();

        using (var transaction = context.Database.BeginTransaction())
        {
            context.OrdenesFabricacion.Add(CrearOrden(numero1));
            context.SaveChanges();

            transaction.CreateSavepoint(savepoint);

            context.OrdenesFabricacion.Add(CrearOrden(numero2));
            context.SaveChanges();

            context.OrdenesFabricacion.Add(CrearOrden(numero3));
            context.SaveChanges();

            transaction.RollbackToSavepoint(savepoint);
            context.ChangeTracker.Clear();
            transaction.Commit();
        }

        return new SavepointTresGuardadosM5Dto(
            Existe(numero1),
            Existe(numero2),
            Existe(numero3),
            mars,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }
    */

}
