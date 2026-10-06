using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface IResolucionConflictosM5Repositorio
{
    ResolucionConflictoM5Dto ClienteGana();
    ResolucionConflictoM5Dto BaseDeDatosGana();
    ResolucionConflictoM5Dto ResolucionPersonalizada();
    ResolucionConflictoM5Dto NotificarSinSobrescribir();
    ResolucionConflictoM5Dto ReintentoAcotado(int maxIntentos);
    EliminacionConcurrenteM5Dto DetectarFilaEliminada();

    /*
    // RETO M05 5.3 - PUERTO MERGE POR PROPIEDAD
    MergePropiedadesM5Dto ResolverMergePorPropiedad();
    */

}
