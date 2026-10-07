namespace AceriaData.Application.Dtos;

public sealed record ResultadoTransaccionM5Dto(
    string Escenario,
    bool ResultadoEsperado,
    bool PrimeraOrdenExiste,
    bool SegundaOrdenExiste,
    bool MarsHabilitado,
    IReadOnlyList<string> ComandosSql);

/*
// RETO M05 5.4 - DTO TRES SAVECHANGES
public sealed record SavepointTresGuardadosM5Dto(
    bool PrimeraOrdenExiste,
    bool SegundaOrdenExiste,
    bool TerceraOrdenExiste,
    bool MarsHabilitado,
    IReadOnlyList<string> ComandosSql);
*/
