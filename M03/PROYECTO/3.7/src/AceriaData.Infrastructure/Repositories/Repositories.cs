using AceriaData.Application.Interfaces;
using AceriaData.Application.Dtos;
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

    public List<string> ObtenerClientesUnicos() => _context.OrdenesFabricacion
        .Select(o => o.Cliente).Distinct().OrderBy(c => c).ToList();

    public List<OrdenResumenDto> ObtenerResumenes() => _context.OrdenesFabricacion
        .AsNoTracking().OrderBy(o => o.FechaCreacion)
        .Select(o => new OrdenResumenDto { NumeroOrden = o.NumeroOrden, Cliente = o.Cliente, Estado = o.Estado, FechaCreacion = o.FechaCreacion })
        .ToList();

    public List<OrdenResumenDto> ObtenerResumenesPorEstado(string estado) => _context.OrdenesFabricacion
        .AsNoTracking().Where(o => o.Estado == estado).OrderBy(o => o.FechaCreacion)
        .Select(o => new OrdenResumenDto { NumeroOrden = o.NumeroOrden, Cliente = o.Cliente, Estado = o.Estado, FechaCreacion = o.FechaCreacion })
        .ToList();

    public List<OrdenConTotalesDto> ObtenerOrdenesConTotales() => _context.OrdenesFabricacion
        .AsNoTracking().OrderBy(o => o.NumeroOrden)
        .Select(o => new OrdenConTotalesDto
        {
            NumeroOrden = o.NumeroOrden,
            Cliente = o.Cliente,
            TotalPlanchas = o.Planchas.Count(),
            PesoTotal = o.Planchas.Select(p => (decimal?)p.Peso).Sum() ?? 0m
        }).ToList();

    public string ObtenerSqlProyeccion() => _context.OrdenesFabricacion
        .Where(o => o.Estado == "Pendiente").OrderBy(o => o.FechaCreacion)
        .Select(o => new { o.NumeroOrden, o.Cliente }).ToQueryString();

    public List<OrdenConPlanchasDto> ObtenerOrdenesConPlanchas() => _context.OrdenesFabricacion
        .AsNoTracking().OrderBy(o => o.NumeroOrden)
        .Select(o => new OrdenConPlanchasDto
        {
            NumeroOrden = o.NumeroOrden,
            Cliente = o.Cliente,
            Planchas = o.Planchas.OrderBy(p => p.Id).Select(p => new PlanchaDto { Espesor = p.Espesor, Peso = p.Peso }).ToList()
        }).ToList();

    public List<OrdenConDetalleDto> ObtenerOrdenesConDetalle() => _context.OrdenesFabricacion
        .AsNoTracking().OrderBy(o => o.NumeroOrden)
        .Select(o => new OrdenConDetalleDto
        {
            NumeroOrden = o.NumeroOrden,
            Cliente = o.Cliente,
            Detalle = o.Detalle == null ? null : new DetalleDto { ComposicionQuimica = o.Detalle.ComposicionQuimica, TemperaturaColada = o.Detalle.TemperaturaColada }
        }).ToList();

    public List<OrdenCompletaDto> ObtenerOrdenesCompletas() => _context.OrdenesFabricacion
        .AsNoTracking().OrderBy(o => o.NumeroOrden)
        .Select(o => new OrdenCompletaDto
        {
            NumeroOrden = o.NumeroOrden,
            Cliente = o.Cliente,
            Planchas = o.Planchas.Select(p => new PlanchaDto { Espesor = p.Espesor, Peso = p.Peso }).ToList(),
            Detalle = o.Detalle == null ? null : new DetalleDto { ComposicionQuimica = o.Detalle.ComposicionQuimica, TemperaturaColada = o.Detalle.TemperaturaColada }
        }).ToList();

    public string ObtenerSqlProyeccionNavegacion() => _context.OrdenesFabricacion
        .Select(o => new { o.NumeroOrden, Planchas = o.Planchas.Select(p => new { p.Espesor, p.Peso }).ToList() })
        .ToQueryString();

    public int ContarOrdenes() => _context.OrdenesFabricacion.Count();
    public int ContarOrdenesPorEstado(string estado) => _context.OrdenesFabricacion.Count(o => o.Estado == estado);
    public bool ExisteAlgunaOrden() => _context.OrdenesFabricacion.Any();
    public bool TodasLasOrdenesTienenEstado() => _context.OrdenesFabricacion.All(o => o.Estado != "");
    public decimal ObtenerPesoTotalDePlanchas() => _context.PlanchasAcero.Select(p => (decimal?)p.Peso).Sum() ?? 0m;
    public decimal ObtenerPesoPromedioDePlanchas() => _context.PlanchasAcero.Select(p => (decimal?)p.Peso).Average() ?? 0m;
    public decimal ObtenerPesoMinimoDePlanchas() => _context.PlanchasAcero.Select(p => (decimal?)p.Peso).Min() ?? 0m;
    public decimal ObtenerPesoMaximoDePlanchas() => _context.PlanchasAcero.Select(p => (decimal?)p.Peso).Max() ?? 0m;

    public List<ResumenPorClienteDto> ObtenerResumenPorCliente() => _context.OrdenesFabricacion
        .AsNoTracking().GroupBy(o => o.Cliente)
        .Select(g => new ResumenPorClienteDto { Cliente = g.Key, TotalOrdenes = g.Count(), FechaMasReciente = g.Max(o => o.FechaCreacion) })
        .OrderBy(x => x.Cliente).ToList();

    public List<ResumenPorEstadoDto> ObtenerResumenPorEstado() => _context.OrdenesFabricacion
        .AsNoTracking().GroupBy(o => o.Estado)
        .Select(g => new ResumenPorEstadoDto { Estado = g.Key, TotalOrdenes = g.Count() })
        .OrderBy(x => x.Estado).ToList();

    public List<ResumenMensualDto> ObtenerResumenMensual() => _context.OrdenesFabricacion
        .AsNoTracking().GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })
        .Select(g => new ResumenMensualDto { Anio = g.Key.Year, Mes = g.Key.Month, TotalOrdenes = g.Count() })
        .OrderBy(x => x.Anio).ThenBy(x => x.Mes).ToList();

    public List<ResumenPorClienteConOrdenesDto> ObtenerResumenPorClienteConOrdenes()
    {
        var cabeceras = _context.OrdenesFabricacion.AsNoTracking()
            .GroupBy(o => o.Cliente)
            .Select(g => new { Cliente = g.Key, TotalOrdenes = g.Count() })
            .OrderBy(x => x.Cliente).ToList();
        var ordenes = _context.OrdenesFabricacion.AsNoTracking()
            .Select(o => new OrdenResumenDto { NumeroOrden = o.NumeroOrden, Cliente = o.Cliente, Estado = o.Estado, FechaCreacion = o.FechaCreacion })
            .OrderBy(o => o.FechaCreacion).ToList();
        return cabeceras.Select(c => new ResumenPorClienteConOrdenesDto
        {
            Cliente = c.Cliente,
            TotalOrdenes = c.TotalOrdenes,
            Ordenes = ordenes.Where(o => o.Cliente == c.Cliente).ToList()
        }).ToList();
    }

    private Dictionary<(string Cliente, string Estado), decimal> ObtenerPesosPorClienteEstado() =>
        (from o in _context.OrdenesFabricacion.AsNoTracking()
         from p in o.Planchas
         group p by new { o.Cliente, o.Estado } into g
         select new { g.Key.Cliente, g.Key.Estado, PesoTotal = g.Sum(p => p.Peso) })
        .ToList()
        .ToDictionary(x => (x.Cliente, x.Estado), x => x.PesoTotal);

    public List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstado()
    {
        var pesos = ObtenerPesosPorClienteEstado();
        var grupos = _context.OrdenesFabricacion.AsNoTracking()
            .GroupBy(o => new { o.Cliente, o.Estado })
            .Select(g => new ResumenPorClienteYEstadoDto { Cliente = g.Key.Cliente, Estado = g.Key.Estado, TotalOrdenes = g.Count() })
            .OrderBy(x => x.Cliente).ThenBy(x => x.Estado).ToList();
        foreach (var g in grupos)
            g.PesoTotal = pesos.GetValueOrDefault((g.Cliente, g.Estado), 0m);
        return grupos;
    }

    public List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstadoConFiltro()
    {
        var pesos = ObtenerPesosPorClienteEstado();
        var grupos = _context.OrdenesFabricacion.AsNoTracking()
            .GroupBy(o => new { o.Cliente, o.Estado })
            .Where(g => g.Count() > 1)
            .Select(g => new ResumenPorClienteYEstadoDto { Cliente = g.Key.Cliente, Estado = g.Key.Estado, TotalOrdenes = g.Count() })
            .OrderBy(x => x.Cliente).ThenBy(x => x.Estado).ToList();
        foreach (var g in grupos)
            g.PesoTotal = pesos.GetValueOrDefault((g.Cliente, g.Estado), 0m);
        return grupos;
    }

    public List<ResumenMensualConOrdenesDto> ObtenerResumenMensualConOrdenes()
    {
        var ordenes = _context.OrdenesFabricacion.AsNoTracking()
            .Select(o => new { o.NumeroOrden, o.FechaCreacion })
            .OrderBy(o => o.FechaCreacion).ToList();
        return ordenes.GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })
            .Select(g => new ResumenMensualConOrdenesDto
            {
                Anio = g.Key.Year, Mes = g.Key.Month, TotalOrdenes = g.Count(),
                NumerosOrden = g.Select(o => o.NumeroOrden).ToList()
            }).OrderBy(x => x.Anio).ThenBy(x => x.Mes).ToList();
    }

    public string ObtenerSqlAgrupacionClienteEstado() => _context.OrdenesFabricacion
        .GroupBy(o => new { o.Cliente, o.Estado })
        .Select(g => new { g.Key.Cliente, g.Key.Estado, Total = g.Count() })
        .ToQueryString();

    public List<OrdenJoinDto> ObtenerJoinOrdenesPlanchas() => _context.OrdenesFabricacion
        .Join(_context.PlanchasAcero, o => o.Id, p => p.OrdenId,
            (o, p) => new OrdenJoinDto { NumeroOrden = o.NumeroOrden, Cliente = o.Cliente, Espesor = p.Espesor, Peso = p.Peso })
        .OrderBy(x => x.NumeroOrden).ToList();

    public List<OrdenJoinDto> ObtenerLeftJoinOrdenesPlanchas() =>
        (from o in _context.OrdenesFabricacion
         join p in _context.PlanchasAcero on o.Id equals p.OrdenId into planchas
         from p in planchas.DefaultIfEmpty()
         orderby o.NumeroOrden
         select new OrdenJoinDto
         {
             NumeroOrden = o.NumeroOrden, Cliente = o.Cliente,
             Espesor = p == null ? null : p.Espesor,
             Peso = p == null ? null : p.Peso
         }).ToList();

    public List<OrdenJoinDto> ObtenerOrdenesConDetalleJoin() => _context.OrdenesFabricacion
        .Select(o => new OrdenJoinDto
        {
            NumeroOrden = o.NumeroOrden, Cliente = o.Cliente,
            ComposicionQuimica = o.Detalle == null ? null : o.Detalle.ComposicionQuimica
        }).OrderBy(x => x.NumeroOrden).ToList();

    public List<OrdenConAleacionesDto> ObtenerOrdenesConAleaciones() => _context.OrdenesFabricacion
        .AsNoTracking().OrderBy(o => o.NumeroOrden)
        .Select(o => new OrdenConAleacionesDto
        {
            NumeroOrden = o.NumeroOrden,
            Aleaciones = o.OrdenesAleaciones.OrderBy(oa => oa.Aleacion.Codigo).Select(oa => oa.Aleacion.Codigo).ToList()
        }).ToList();

    public string ObtenerSqlJoinExplicito() => _context.OrdenesFabricacion
        .Join(_context.PlanchasAcero, o => o.Id, p => p.OrdenId,
            (o, p) => new { o.NumeroOrden, o.Cliente, p.Espesor, p.Peso })
        .ToQueryString();

    /*
    // RETO M03 3.7 - SQL REAL DEL LEFT JOIN
    // Usa el patrón GroupJoin + DefaultIfEmpty y permite inspeccionar la condición OrdenId.
    public string ObtenerSqlLeftJoinReto() =>
        (from o in _context.OrdenesFabricacion
         join p in _context.PlanchasAcero on o.Id equals p.OrdenId into planchas
         from p in planchas.DefaultIfEmpty()
         orderby o.NumeroOrden
         select new
         {
             o.NumeroOrden,
             o.Cliente,
             Espesor = (double?)p.Espesor,
             Peso = (decimal?)p.Peso
         }).ToQueryString();
    */

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
