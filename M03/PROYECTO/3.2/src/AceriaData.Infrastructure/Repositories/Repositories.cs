using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed class OrdenRepositorio : IOrdenRepositorio
{
    private readonly AceriaDbContext _context;
    public OrdenRepositorio(AceriaDbContext context) => _context = context;

    public OrdenFabricacion? ObtenerPorId(int id) => _context.OrdenesFabricacion.Find(id);
    public OrdenFabricacion? ObtenerPorNumero(string numeroOrden) => _context.OrdenesFabricacion.FirstOrDefault(x => x.NumeroOrden == numeroOrden);
    public List<OrdenFabricacion> ObtenerTodas() => _context.OrdenesFabricacion.OrderBy(x => x.Id).ToList();

    public IQueryable<OrdenFabricacion> Consulta() => _context.OrdenesFabricacion.AsQueryable();

    public string ObtenerSqlFundamentos() => _context.OrdenesFabricacion
        .Where(o => o.Cliente == "Constructora del Norte")
        .OrderBy(o => o.FechaCreacion)
        .ToQueryString();

    public List<OrdenFabricacion> ObtenerPendientesPorCliente(string cliente) => _context.OrdenesFabricacion
        .Where(o => o.Cliente == cliente && o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion).ToList();

    public List<OrdenFabricacion> ObtenerPorEstadoOrdenadasPorFecha(string estado) => _context.OrdenesFabricacion
        .Where(o => o.Estado == estado)
        .OrderByDescending(o => o.FechaCreacion).ToList();

    public List<OrdenFabricacion> ObtenerPorRangoDeFechas(DateTime desde, DateTime hasta) => _context.OrdenesFabricacion
        .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
        .OrderBy(o => o.FechaCreacion).ToList();

    public List<OrdenFabricacion> ObtenerPorClienteOrdenadas(string cliente) => _context.OrdenesFabricacion
        .Where(o => o.Cliente == cliente)
        .OrderBy(o => o.Cliente).ThenByDescending(o => o.FechaCreacion).ToList();

    public List<OrdenFabricacion> ObtenerPorClienteYRangoDeFechas(string cliente, DateTime desde, DateTime hasta) => _context.OrdenesFabricacion
        .Where(o => o.Cliente == cliente && o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
        .OrderBy(o => o.Estado).ThenByDescending(o => o.FechaCreacion).ToList();

    public string ObtenerSqlConsultaBasica() => _context.OrdenesFabricacion
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.Cliente).ThenByDescending(o => o.FechaCreacion)
        .ToQueryString();

    public void Agregar(OrdenFabricacion orden) => _context.OrdenesFabricacion.Add(orden);
    public void Eliminar(OrdenFabricacion orden) => _context.OrdenesFabricacion.Remove(orden);
}

public sealed class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly AceriaDbContext _context;
    private IOrdenRepositorio? _ordenes;
    public UnidadDeTrabajo(AceriaDbContext context) => _context = context;
    public IOrdenRepositorio Ordenes => _ordenes ??= new OrdenRepositorio(_context);
    public int Guardar() => _context.SaveChanges();
    public void Dispose() => _context.Dispose();
}
