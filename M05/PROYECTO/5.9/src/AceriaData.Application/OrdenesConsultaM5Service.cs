using AceriaData.Application.Interfaces;
using AceriaData.Domain.Entities;

namespace AceriaData.Application.UseCases;

public sealed class OrdenesConsultaM5Service
{
    private readonly IOrdenRepositorio _repositorio;

    public OrdenesConsultaM5Service(IOrdenRepositorio repositorio) => _repositorio = repositorio;

    public List<OrdenFabricacion> ObtenerPendientes() =>
        _repositorio.ObtenerTodas().Where(o => o.Estado == "Pendiente").ToList();

    public void Registrar(OrdenFabricacion orden) => _repositorio.Agregar(orden);

    /* // RETO M05 5.9 - OPERACION ESPECIFICA SERVICIO
    public List<OrdenFabricacion> ObtenerPendientesRecientesPorCliente(string cliente, DateTime desde) =>
        _repositorio.ObtenerPendientesRecientesPorCliente(cliente, desde);
    */
}
