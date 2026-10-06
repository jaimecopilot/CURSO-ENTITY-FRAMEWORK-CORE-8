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
    List<OrdenConPlanchasDto> ObtenerOrdenesConPlanchas();
    List<OrdenConDetalleDto> ObtenerOrdenesConDetalle();
    List<OrdenCompletaDto> ObtenerOrdenesCompletas();
    string ObtenerSqlProyeccionNavegacion();
    int ContarOrdenes();
    int ContarOrdenesPorEstado(string estado);
    bool ExisteAlgunaOrden();
    bool TodasLasOrdenesTienenEstado();
    decimal ObtenerPesoTotalDePlanchas();
    decimal ObtenerPesoPromedioDePlanchas();
    decimal ObtenerPesoMinimoDePlanchas();
    decimal ObtenerPesoMaximoDePlanchas();
    List<ResumenPorClienteDto> ObtenerResumenPorCliente();
    List<ResumenPorEstadoDto> ObtenerResumenPorEstado();
    List<ResumenMensualDto> ObtenerResumenMensual();
    List<ResumenPorClienteConOrdenesDto> ObtenerResumenPorClienteConOrdenes();
    List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstado();
    List<ResumenPorClienteYEstadoDto> ObtenerResumenPorClienteYEstadoConFiltro();
    List<ResumenMensualConOrdenesDto> ObtenerResumenMensualConOrdenes();
    string ObtenerSqlAgrupacionClienteEstado();

    /*
    // RETO M03 3.6 - PUERTO SQL HAVING
    string ObtenerSqlAgrupacionClienteEstadoConHavingReto();
    */
    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
}

public interface IUnidadDeTrabajo : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    int Guardar();
}
