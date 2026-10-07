using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;

namespace AceriaData.Application.Interfaces;

public interface IOrdenRepositorio
{
    OrdenFabricacion? ObtenerPorId(int id);
    OrdenFabricacion? ObtenerPorNumero(string numeroOrden);
    List<OrdenFabricacion> ObtenerTodas();
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
    List<OrdenJoinDto> ObtenerJoinOrdenesPlanchas();
    List<OrdenJoinDto> ObtenerLeftJoinOrdenesPlanchas();
    List<OrdenJoinDto> ObtenerOrdenesConDetalleJoin();
    List<OrdenConAleacionesDto> ObtenerOrdenesConAleaciones();
    string ObtenerSqlJoinExplicito();
    List<OrdenFabricacion> ObtenerOrdenesConPlanchasInclude();
    List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleInclude();
    List<OrdenFabricacion> ObtenerOrdenesConAleacionesInclude();
    List<OrdenFabricacion> ObtenerOrdenesConPlanchasPesadasInclude();
    List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleSplitQuery();
    string ObtenerSqlInclude();
    List<OrdenFabricacion> ObtenerTodasSinInclude();
    OrdenFabricacion? ObtenerConCargaExplicita(string numeroOrden);
    OrdenFabricacion? ObtenerConPlanchasPesadasExplicitas(string numeroOrden, decimal pesoMinimo);
    ConsultaCompuestaResultadoDto BuscarOrdenes(string? cliente, string? estado, DateTime? desde, string ordenarPor, bool descendente, int pagina, int tamanoPagina);
    List<OrdenResumenDto> ObtenerResumenesPendientesOptimizado();
    bool ExisteAlgunaOrdenPendiente();
    List<OrdenFabricacion> ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();
    OrdenFabricacion? ObtenerPorNumeroOptimizado(string numeroOrden);
    string ObtenerSqlPendientesOrdenadasM4();
    string ObtenerSqlConIncludeM4();
    string ObtenerSqlConProyeccionM4();
    TrackingMetricaDto MedirConsultaConTrackingM4();
    TrackingMetricaDto MedirConsultaSinTrackingM4();
    /*
    // ERROR CONTROLADO M04 4.2 - PUERTO SIN LIMPIAR TRACKER
    TrackingMetricaDto MedirConsultaSinTrackingSinLimpiarM4();
    */

    /*
    // RETO M04 4.2 - PUERTO GRAFO TRACKING
    TrackingMetricaDto MedirGrafoConTrackingM4();
    TrackingMetricaDto MedirGrafoSinTrackingM4();
    */

    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
}

public interface IUnidadDeTrabajo : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    int Guardar();
}

// CANONICAL INLINE M04 4.2 - BLOCK 01
// SECTION: Paso 2: Añadir los métodos de tracking a la interfaz del repositorio
// SOURCE TARGET: Modificar src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
//
// namespace AceriaData.Application.Interfaces;
//
// public interface IOrdenRepositorio
// {
//     // ... métodos existentes ...
//
//     List<OrdenFabricacion> ObtenerConTracking();
//     List<OrdenFabricacion> ObtenerSinTracking();
//     int ContarEntidadesRastreadas();
//
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ========================================================================

// CANONICAL INLINE M04 4.2 - BLOCK 08
// SECTION: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 1: Añadir los métodos a la interfaz IOrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// List<OrdenFabricacion> ObtenerConPlanchasConTracking();
// List<OrdenFabricacion> ObtenerConPlanchasSinTracking();
// ========================================================================
