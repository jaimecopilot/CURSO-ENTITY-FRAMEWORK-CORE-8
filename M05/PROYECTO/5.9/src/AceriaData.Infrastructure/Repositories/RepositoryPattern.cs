using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public class Repositorio<T> : IRepositorio<T> where T : class
{
    protected readonly AceriaDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repositorio(AceriaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual T? ObtenerPorId(int id) => _dbSet.Find(id);
    public virtual List<T> ObtenerTodas() => _dbSet.ToList();
    public virtual void Agregar(T entidad) => _dbSet.Add(entidad);
    public virtual void Eliminar(T entidad) => _dbSet.Remove(entidad);
}

public sealed class DetalleOrdenRepositorio : Repositorio<DetalleOrden>, IDetalleOrdenRepositorio
{
    public DetalleOrdenRepositorio(AceriaDbContext context) : base(context) { }

    public DetalleOrden? ObtenerPorOrden(int ordenId) =>
        _context.DetallesOrden.AsNoTracking().FirstOrDefault(d => d.OrdenId == ordenId);

    public List<DetalleOrden> ObtenerConTemperaturaMayorA(double temperatura) =>
        _context.DetallesOrden.AsNoTracking()
            .Where(d => d.TemperaturaColada > temperatura)
            .OrderBy(d => d.Id)
            .ToList();
}
