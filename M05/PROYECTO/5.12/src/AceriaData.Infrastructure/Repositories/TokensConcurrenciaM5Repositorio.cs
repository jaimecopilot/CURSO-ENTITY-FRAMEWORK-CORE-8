using System.Data;
using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AceriaData.Infrastructure.Repositories;

public sealed class TokensConcurrenciaM5Repositorio : ITokensConcurrenciaM5Repositorio
{
    private const string NumeroOrden = "OF-2024-0001";
    private readonly IServiceScopeFactory _scopeFactory;

    public TokensConcurrenciaM5Repositorio(IServiceScopeFactory scopeFactory) =>
        _scopeFactory = scopeFactory;

    public RowVersionM5Dto DemostrarRowVersion()
    {
        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();

        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
        var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);

        var inicialA = Convert.ToHexString(ordenA.RowVersion);
        var inicialB = Convert.ToHexString(ordenB.RowVersion);

        SqlCommandCounterInterceptor.Instance.Reset();

        ordenA.Cliente = "Cliente 5.2 A";
        contextA.SaveChanges();
        var despuesA = Convert.ToHexString(ordenA.RowVersion);

        ordenB.Estado = "EnProceso";
        var conflicto = false;
        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            conflicto = true;
        }

        return new RowVersionM5Dto(
            inicialA, inicialB, despuesA, conflicto,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    public TokenPropiedadM5Dto DemostrarTokenDePropiedad()
    {
        using (var preparacion = _scopeFactory.CreateScope())
        {
            var context = preparacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
            var detalle = context.DetallesOrden.Single(d => d.Orden.NumeroOrden == NumeroOrden);
            detalle.EstadoDetalle = "Pendiente";
            detalle.Notas = null;
            context.SaveChanges();
        }

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();

        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var detalleA = contextA.DetallesOrden.Single(d => d.Orden.NumeroOrden == NumeroOrden);
        var detalleB = contextB.DetallesOrden.Single(d => d.Orden.NumeroOrden == NumeroOrden);

        var inicialA = detalleA.EstadoDetalle;
        var inicialB = detalleB.EstadoDetalle;

        SqlCommandCounterInterceptor.Instance.Reset();

        detalleA.EstadoDetalle = "EnProceso";
        contextA.SaveChanges();

        detalleB.Notas = "Cambio concurrente de B";
        var conflicto = false;
        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            conflicto = true;
        }

        return new TokenPropiedadM5Dto(
            inicialA, inicialB, detalleA.EstadoDetalle, conflicto,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    public IndiceRowVersionM5Dto ComprobarIndiceRowVersion()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var connection = context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.indexes AS i
            INNER JOIN sys.index_columns AS ic
                ON i.object_id = ic.object_id
               AND i.index_id = ic.index_id
            INNER JOIN sys.columns AS c
                ON ic.object_id = c.object_id
               AND ic.column_id = c.column_id
            WHERE i.object_id = OBJECT_ID(N'dbo.OrdenesFabricacion')
              AND c.name = N'RowVersion';
            """;

        return new IndiceRowVersionM5Dto(Convert.ToInt32(command.ExecuteScalar()) > 0);
    }
}
