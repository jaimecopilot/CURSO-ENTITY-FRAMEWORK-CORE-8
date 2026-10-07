using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface ITransaccionesM5Repositorio
{
    ResultadoTransaccionM5Dto DemostrarAtomicidadSaveChanges();
    ResultadoTransaccionM5Dto DemostrarCommitExplicito();
    ResultadoTransaccionM5Dto DemostrarRollbackExplicito();
    ResultadoTransaccionM5Dto DemostrarRollbackASavepoint();

    /*
    // RETO M05 5.4 - PUERTO TRES SAVECHANGES
    SavepointTresGuardadosM5Dto DemostrarTresSaveChangesConRollbackParcial();
    */

}
