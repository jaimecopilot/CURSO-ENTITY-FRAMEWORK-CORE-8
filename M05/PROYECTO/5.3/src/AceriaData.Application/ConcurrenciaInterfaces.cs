using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface IConcurrenciaOptimistaM5Repositorio
{
    ConcurrenciaMismaPropiedadDto DemostrarActualizacionPerdidaMismaPropiedad();
    ConcurrenciaPropiedadesDistintasDto DemostrarCambiosEnPropiedadesDistintas();
}
