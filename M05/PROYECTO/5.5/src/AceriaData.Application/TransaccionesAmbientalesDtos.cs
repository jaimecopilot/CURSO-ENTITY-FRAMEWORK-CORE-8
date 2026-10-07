namespace AceriaData.Application.Dtos;

public sealed record TransaccionAmbientalM5Dto(
    string Escenario,
    bool Persistido,
    bool TransaccionAmbientalActiva,
    bool FlujoAsyncConservado,
    bool PromocionDistribuida,
    string NivelAislamiento,
    IReadOnlyList<string> ComandosSql);

public sealed record OpcionesTransactionScopeM5Dto(
    bool RequiredReutilizaTransaccion,
    bool RequiresNewCreaOtra,
    bool SuppressEliminaAmbiente,
    string AislamientoPredeterminado,
    double TimeoutPredeterminadoSegundos);

public sealed record EfectoExternoM5Dto(
    bool FilaBaseDeDatosPersistida,
    bool EfectoExternoPermanece);

public sealed record AislamientoM5Dto(
    string Solicitado,
    string Observado,
    bool OperacionPersistida);

/*
// RETO M05 5.5 - DTO SUPPRESS
public sealed record SuppressFueraAmbienteM5Dto(
    bool EscrituraAmbientalPersistida,
    bool EscrituraSuprimidaPersistida,
    bool SuppressSinTransactionCurrent);
*/
