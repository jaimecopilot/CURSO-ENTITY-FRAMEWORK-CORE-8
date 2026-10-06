using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;

namespace AceriaData.Application.Interfaces;

public interface IOrdenRepositorio
{
    OrdenFabricacion? ObtenerPorId(int id);
    OrdenFabricacion? ObtenerPorNumero(string numeroOrden);
    List<OrdenFabricacion> ObtenerTodas();
    IQueryable<OrdenFabricacion> Consulta();
    string ObtenerSqlFundamentos();
    List<OrdenFabricacion> ObtenerPendientesPorCliente(string cliente);
    List<OrdenFabricacion> ObtenerPorEstadoOrdenadasPorFecha(string estado);
    List<OrdenFabricacion> ObtenerPorRangoDeFechas(DateTime desde, DateTime hasta);
    List<OrdenFabricacion> ObtenerPorClienteOrdenadas(string cliente);
    List<OrdenFabricacion> ObtenerPorClienteYRangoDeFechas(string cliente, DateTime desde, DateTime hasta);
    string ObtenerSqlConsultaBasica();
    List<string> ObtenerClientesUnicos();
    List<OrdenResumenDto> ObtenerResumenes();
    List<OrdenResumenDto> ObtenerResumenesPorEstado(string estado);
    List<OrdenConTotalesDto> ObtenerOrdenesConTotales();
    string ObtenerSqlProyeccion();

    /*
    // RETO M03 3.3 - PUERTO SQL ENTIDAD COMPLETA
    // Se activa coordinadamente con el repositorio y el caso de uso del reto.
    string ObtenerSqlEntidadCompletaRetoProyeccion();
    */

    /*
    // ERROR CONTROLADO M03 3.3 - TIPO ANONIMO COMO CONTRATO PUBLICO
    // 'var' no puede usarse como tipo de retorno público: el contrato debe usar un DTO con nombre.
    var ObtenerResumenAnonimo();
    */
    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
}

public interface IUnidadDeTrabajo : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    int Guardar();
}
