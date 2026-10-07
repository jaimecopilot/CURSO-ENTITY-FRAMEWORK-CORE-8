using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface ITransaccionesAmbientalesM5Repositorio
{
    Task<TransaccionAmbientalM5Dto> DemostrarDosContextosAsync();
    Task<TransaccionAmbientalM5Dto> DemostrarRollbackSinCompleteAsync();
    OpcionesTransactionScopeM5Dto DemostrarOpcionesDeScope();
    EfectoExternoM5Dto DemostrarRecursoExternoNoTransaccional();
    AislamientoM5Dto DemostrarReadCommitted();
    AislamientoM5Dto DemostrarSnapshot();

    /*
    // RETO M05 5.5 - PUERTO SUPPRESS
    SuppressFueraAmbienteM5Dto DemostrarSuppressFueraDeRollback();
    */

}
