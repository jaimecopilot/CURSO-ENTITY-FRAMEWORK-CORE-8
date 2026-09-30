namespace AceriaData.Application.Dtos;

public sealed record ResultadoTransaccionM5Dto(
    string Escenario,
    bool ResultadoEsperado,
    bool PrimeraOrdenExiste,
    bool SegundaOrdenExiste,
    bool MarsHabilitado,
    IReadOnlyList<string> ComandosSql);
