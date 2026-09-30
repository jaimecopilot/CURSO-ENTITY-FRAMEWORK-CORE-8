using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace AceriaData.Infrastructure.Repositories;

public sealed class ResolucionConflictosM5Repositorio : IResolucionConflictosM5Repositorio
{
    private const string NumeroOrden = "OF-2024-0001";
    private readonly IServiceScopeFactory _scopeFactory;

    public ResolucionConflictosM5Repositorio(IServiceScopeFactory scopeFactory) =>
        _scopeFactory = scopeFactory;

    public ResolucionConflictoM5Dto ClienteGana()
    {
        RestaurarOrden();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();
        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var a = Cargar(contextA);
        var b = Cargar(contextB);

        SqlCommandCounterInterceptor.Instance.Reset();

        a.Cliente = "Cliente A - cliente gana";
        contextA.SaveChanges();

        b.Cliente = "Cliente B - cliente gana";
        var conflicto = false;
        var intentos = 1;
        var valores = new List<ValorConflictoM5Dto>();

        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            conflicto = true;
            var entry = ex.Entries.Single();
            var db = entry.GetDatabaseValues()
                ?? throw new InvalidOperationException("La orden desapareció durante ClienteGana.");
            valores = CrearValores(entry, db);
            entry.OriginalValues.SetValues(db);
            intentos++;
            contextB.SaveChanges();
        }

        var final = LeerFinal();
        return Resultado("Cliente gana", conflicto, final, intentos, valores);
    }

    public ResolucionConflictoM5Dto BaseDeDatosGana()
    {
        RestaurarOrden();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();
        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var a = Cargar(contextA);
        var b = Cargar(contextB);

        SqlCommandCounterInterceptor.Instance.Reset();

        a.Cliente = "Cliente A - base gana";
        contextA.SaveChanges();

        b.Cliente = "Cliente B - descartado";
        var conflicto = false;
        var valores = new List<ValorConflictoM5Dto>();

        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            conflicto = true;
            var entry = ex.Entries.Single();
            var db = entry.GetDatabaseValues()
                ?? throw new InvalidOperationException("La orden desapareció durante BaseDeDatosGana.");
            valores = CrearValores(entry, db);
            entry.Reload();
        }

        var final = LeerFinal();
        return Resultado("Base de datos gana", conflicto, final, 1, valores);
    }

    public ResolucionConflictoM5Dto ResolucionPersonalizada()
    {
        RestaurarOrden();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();
        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var a = Cargar(contextA);
        var b = Cargar(contextB);

        SqlCommandCounterInterceptor.Instance.Reset();

        a.Cliente = "Cliente A - merge";
        a.Estado = "EnProceso A";
        contextA.SaveChanges();

        b.Cliente = "Cliente B - merge";
        b.Estado = "Completada B";

        var conflicto = false;
        var intentos = 1;
        var valores = new List<ValorConflictoM5Dto>();

        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            conflicto = true;
            var entry = ex.Entries.Single();
            var db = entry.GetDatabaseValues()
                ?? throw new InvalidOperationException("La orden desapareció durante la fusión.");
            valores = CrearValores(entry, db);

            entry.OriginalValues.SetValues(db);
            entry.CurrentValues[nameof(OrdenFabricacion.Estado)] =
                db[nameof(OrdenFabricacion.Estado)];

            intentos++;
            contextB.SaveChanges();
        }

        var final = LeerFinal();
        return Resultado("Resolución personalizada", conflicto, final, intentos, valores);
    }

    public ResolucionConflictoM5Dto NotificarSinSobrescribir()
    {
        RestaurarOrden();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();
        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var a = Cargar(contextA);
        var b = Cargar(contextB);

        SqlCommandCounterInterceptor.Instance.Reset();

        a.Cliente = "Cliente A - notificación";
        contextA.SaveChanges();

        b.Cliente = "Cliente B - pendiente de decisión";
        var conflicto = false;
        var valores = new List<ValorConflictoM5Dto>();

        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            conflicto = true;
            var entry = ex.Entries.Single();
            var db = entry.GetDatabaseValues()
                ?? throw new InvalidOperationException("La orden desapareció durante la notificación.");
            valores = CrearValores(entry, db);
        }

        var final = LeerFinal();
        return Resultado("Notificación al usuario", conflicto, final, 1, valores);
    }

    public ResolucionConflictoM5Dto ReintentoAcotado(int maxIntentos)
    {
        if (maxIntentos < 1)
            throw new ArgumentOutOfRangeException(nameof(maxIntentos));

        RestaurarOrden();

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();
        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var a = Cargar(contextA);
        var b = Cargar(contextB);

        SqlCommandCounterInterceptor.Instance.Reset();

        a.Cliente = "Cliente A - provoca reintento";
        contextA.SaveChanges();

        b.Cliente = "Cliente B - reintento";

        var intentos = 0;
        var conflicto = false;
        var valores = new List<ValorConflictoM5Dto>();

        while (true)
        {
            intentos++;
            try
            {
                contextB.SaveChanges();
                break;
            }
            catch (DbUpdateConcurrencyException ex) when (intentos < maxIntentos)
            {
                conflicto = true;
                var entry = ex.Entries.Single();
                var db = entry.GetDatabaseValues()
                    ?? throw new InvalidOperationException("La orden fue eliminada durante el reintento.");
                valores = CrearValores(entry, db);
                entry.OriginalValues.SetValues(db);
            }
        }

        var final = LeerFinal();
        return Resultado("Reintento acotado", conflicto, final, intentos, valores);
    }

    public EliminacionConcurrenteM5Dto DetectarFilaEliminada()
    {
        const string numero = "OF-M5-DELETE";

        using (var preparacion = _scopeFactory.CreateScope())
        {
            var context = preparacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
            var existente = context.OrdenesFabricacion
                .IgnoreQueryFilters()
                .SingleOrDefault(o => o.NumeroOrden == numero);
            if (existente is not null)
            {
                context.Remove(existente);
                context.SaveChanges();
            }

            context.OrdenesFabricacion.Add(new OrdenFabricacion
            {
                NumeroOrden = numero,
                Cliente = "Cliente eliminación",
                Estado = "Pendiente",
                FechaCreacion = new DateTime(2026, 9, 30)
            });
            context.SaveChanges();
        }

        using var scopeA = _scopeFactory.CreateScope();
        using var scopeB = _scopeFactory.CreateScope();
        var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var a = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == numero);
        var b = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == numero);

        SqlCommandCounterInterceptor.Instance.Reset();

        contextA.Remove(a);
        contextA.SaveChanges();

        b.Cliente = "Cambio sobre fila ya borrada";

        var conflicto = false;
        var noExiste = false;
        var desacoplada = false;

        try
        {
            contextB.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            conflicto = true;
            var entry = ex.Entries.Single();
            noExiste = entry.GetDatabaseValues() is null;

            if (noExiste)
            {
                entry.State = EntityState.Detached;
                desacoplada = entry.State == EntityState.Detached;
            }
        }

        return new EliminacionConcurrenteM5Dto(
            conflicto,
            noExiste,
            desacoplada,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());
    }

    private OrdenFabricacion Cargar(AceriaDbContext context) =>
        context.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);

    private void RestaurarOrden()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var orden = context.OrdenesFabricacion
            .IgnoreQueryFilters()
            .Single(o => o.NumeroOrden == NumeroOrden);

        orden.Cliente = "Constructora del Norte";
        orden.Estado = "Pendiente";
        context.SaveChanges();
    }

    private (string Cliente, string Estado) LeerFinal()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
        var fila = context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.NumeroOrden == NumeroOrden)
            .Select(o => new { o.Cliente, o.Estado })
            .Single();

        return (fila.Cliente, fila.Estado);
    }

    private ResolucionConflictoM5Dto Resultado(
        string estrategia,
        bool conflicto,
        (string Cliente, string Estado) final,
        int intentos,
        IReadOnlyList<ValorConflictoM5Dto> valores) =>
        new(
            estrategia,
            conflicto,
            final.Cliente,
            final.Estado,
            intentos,
            valores,
            SqlCommandCounterInterceptor.Instance.SnapshotCommands());

    private static List<ValorConflictoM5Dto> CrearValores(
        EntityEntry entry,
        PropertyValues db)
    {
        return entry.Properties
            .Select(p => new ValorConflictoM5Dto(
                p.Metadata.Name,
                Formatear(p.OriginalValue),
                Formatear(p.CurrentValue),
                Formatear(db[p.Metadata.Name])))
            .ToList();
    }

    private static string Formatear(object? value) =>
        value switch
        {
            null => "<null>",
            byte[] bytes => Convert.ToHexString(bytes),
            _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "<null>"
        };
}
