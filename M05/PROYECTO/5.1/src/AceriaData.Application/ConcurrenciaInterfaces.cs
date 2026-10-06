using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface IConcurrenciaOptimistaM5Repositorio
{
    ConcurrenciaMismaPropiedadDto DemostrarActualizacionPerdidaMismaPropiedad();
    ConcurrenciaPropiedadesDistintasDto DemostrarCambiosEnPropiedadesDistintas();

    /*
    // RETO M05 5.1 - PUERTO TERCERA ESCRITURA
    ConcurrenciaTresEscriturasDto DemostrarTerceraEscrituraMismaPropiedad();
    */

}
