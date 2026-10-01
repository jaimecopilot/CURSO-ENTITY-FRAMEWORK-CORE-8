using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed class RepositorioUnidadTrabajoM5Diagnostico
{
    private readonly AceriaDbContext _context;
    private readonly IOrdenRepositorio _repositorio;
    private readonly IDbContextFactory<AceriaDbContext> _factory;

    public RepositorioUnidadTrabajoM5Diagnostico(
        AceriaDbContext context,
        IOrdenRepositorio repositorio,
        IDbContextFactory<AceriaDbContext> factory)
    {
        _context = context;
        _repositorio = repositorio;
        _factory = factory;
    }

    public async Task EjecutarAsync()
    {
        Console.WriteLine("--- Evidencia observable Repository vs DbContext ---");

        var directos = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.Id)
            .Select(o => o.NumeroOrden)
            .ToList();

        var porRepositorio = _repositorio.ObtenerTodas()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.Id)
            .Select(o => o.NumeroOrden)
            .ToList();

        Console.WriteLine($"Resultados equivalentes: {directos.SequenceEqual(porRepositorio)}");
        Console.WriteLine($"Total pendientes observadas: {directos.Count}");

        await using var contextoA = await _factory.CreateDbContextAsync();
        await using var contextoB = await _factory.CreateDbContextAsync();
        Console.WriteLine($"IDbContextFactory crea contextos distintos: {!ReferenceEquals(contextoA, contextoB)}");

        var sqlDirecto = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.Id)
            .Select(o => o.NumeroOrden)
            .ToQueryString();

        Console.WriteLine("SQL directo observado:");
        Console.WriteLine(sqlDirecto);
        Console.WriteLine("No se atribuye un coste fijo de tiempo o memoria al patrón sin un benchmark controlado.");
    }
}
