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

// ============================================================================
// EJEMPLO DEL PASO 6A
// Adaptadores OrdenRepositorio y UnidadDeTrabajo.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 6 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// using AceriaData.Domain.Entities;
// using AceriaData.Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
// 
// namespace AceriaData.Infrastructure.Repositories;
// 
// public sealed class OrdenRepositorio : IOrdenRepositorio
// {
//     private readonly AceriaDbContext _context;
//     public OrdenRepositorio(AceriaDbContext context) => _context = context;
//     public OrdenFabricacion? ObtenerPorId(int id) => _context.OrdenesFabricacion.Find(id);
//     public OrdenFabricacion? ObtenerPorNumero(string numeroOrden) => _context.OrdenesFabricacion.FirstOrDefault(x => x.NumeroOrden == numeroOrden);
//     public List<OrdenFabricacion> ObtenerTodas() => _context.OrdenesFabricacion.OrderBy(x => x.Id).ToList();
//     public void Agregar(OrdenFabricacion orden) => _context.OrdenesFabricacion.Add(orden);
//     public void Eliminar(OrdenFabricacion orden) => _context.OrdenesFabricacion.Remove(orden);
// }
// 
// public sealed class UnidadDeTrabajo : IUnidadDeTrabajo
// {
//     private readonly AceriaDbContext _context;
//     private IOrdenRepositorio? _ordenes;
//     public UnidadDeTrabajo(AceriaDbContext context) => _context = context;
//     public IOrdenRepositorio Ordenes => _ordenes ??= new OrdenRepositorio(_context);
//     public int Guardar() => _context.SaveChanges();
//     public void Dispose() => _context.Dispose();
// }
// ============================================================================
