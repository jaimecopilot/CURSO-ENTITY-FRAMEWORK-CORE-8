using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AceriaData.Infrastructure.Repositories;

public sealed class ConcurrenciaOptimistaM5Repositorio : IConcurrenciaOptimistaM5Repositorio
{
    private const string NumeroOrden = "OF-2024-0001";
    private const string ClienteInicial = "Constructora del Norte";
    private const string EstadoInicial = "Pendiente";

    private readonly IServiceScopeFactory _scopeFactory;

    public ConcurrenciaOptimistaM5Repositorio(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ConcurrenciaMismaPropiedadDto DemostrarActualizacionPerdidaMismaPropiedad()
    {
        RestaurarEstadoInicial();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();

        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        SqlCommandCounterInterceptor.Instance.Reset();

        var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
        var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);

        var clienteInicial = ordenA.Cliente;

        ordenA.Cliente = "Cliente actualizado por A";
        contextA.SaveChanges();

        ordenB.Cliente = "Cliente actualizado por B";
        contextB.SaveChanges();

        using var scopeVerificacion = _scopeFactory.CreateScope();
        var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var clienteFinal = contextVerificacion.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.NumeroOrden == NumeroOrden)
            .Select(o => o.Cliente)
            .Single();

        return new ConcurrenciaMismaPropiedadDto(
            clienteInicial,
            ordenA.Cliente,
            ordenB.Cliente,
            clienteFinal,
            clienteFinal == ordenB.Cliente && clienteFinal != ordenA.Cliente,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    public ConcurrenciaPropiedadesDistintasDto DemostrarCambiosEnPropiedadesDistintas()
    {
        RestaurarEstadoInicial();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();

        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        SqlCommandCounterInterceptor.Instance.Reset();

        var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
        var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);

        var clienteInicial = ordenA.Cliente;
        var estadoInicial = ordenB.Estado;

        ordenA.Cliente = "Cliente actualizado por A";
        contextA.SaveChanges();

        ordenB.Estado = "EnProceso";
        contextB.SaveChanges();

        using var scopeVerificacion = _scopeFactory.CreateScope();
        var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var resultado = contextVerificacion.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.NumeroOrden == NumeroOrden)
            .Select(o => new { o.Cliente, o.Estado })
            .Single();

        return new ConcurrenciaPropiedadesDistintasDto(
            clienteInicial,
            estadoInicial,
            resultado.Cliente,
            resultado.Estado,
            resultado.Cliente == ordenA.Cliente && resultado.Estado == ordenB.Estado,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    private void RestaurarEstadoInicial()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var orden = context.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
        orden.Cliente = ClienteInicial;
        orden.Estado = EstadoInicial;
        context.SaveChanges();
    }
}
