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
    IdentityResolutionMetricaDto MedirNoTrackingSinResolucionM4();
    IdentityResolutionMetricaDto MedirNoTrackingConResolucionM4();
    NMasUnoMetricaDto EjecutarNMasUnoM4();
    SolucionNMasUnoMetricaDto EjecutarIncludeContraNMasUnoM4();
    SolucionNMasUnoMetricaDto EjecutarProyeccionContraNMasUnoM4();
    SolucionNMasUnoMetricaDto EjecutarSplitQueryContraNMasUnoM4();
    List<OrdenFabricacion> ObtenerPendientesEntidadCompletaM4();
    List<OrdenResumenDto> ObtenerPendientesProyectadasM4();
    string ObtenerSqlPendientesEntidadCompletaM4();
    string ObtenerSqlPendientesProyectadasM4();
    bool FiltroPersonalizadoNoTraducibleFallaM4(string estado);
    int ContarConEvaluacionClienteExplicitaM4(string estado);
    string ObtenerSqlClienteConFuncionM4(string cliente);
    string ObtenerSqlClienteDirectoM4(string cliente);
    SplitQueryMetricaDto MedirSingleQueryM4();
    SplitQueryMetricaDto MedirSplitQueryM4();
    string ObtenerSqlSingleQueryM4();
    string ObtenerSqlSplitQueryM4();
    List<OrdenFabricacion> ObtenerPorEstadoNormalM4(string estado);
    List<OrdenFabricacion> ObtenerPorEstadoCompiladoM4(string estado);
    PaginaOrdenesDto ObtenerPaginaOffsetM4(int pagina, int tamano);
    PaginaOrdenesDto ObtenerPaginaKeysetM4(DateTime ultimaFecha, int ultimoId, int tamano);
    DiagnosticoRendimientoDto DiagnosticarPendientesM4();
    ChecklistRendimientoDto EjecutarChecklistFinalM4();
    void Agregar(OrdenFabricacion orden);
    void Eliminar(OrdenFabricacion orden);
}

public interface IUnidadDeTrabajo : IDisposable
{
    IOrdenRepositorio Ordenes { get; }
    int Guardar();
}
