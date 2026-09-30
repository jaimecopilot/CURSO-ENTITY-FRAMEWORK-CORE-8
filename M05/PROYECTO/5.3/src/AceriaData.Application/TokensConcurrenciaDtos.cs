namespace AceriaData.Application.Dtos;

public sealed record RowVersionM5Dto(
    string RowVersionAInicial,
    string RowVersionBInicial,
    string RowVersionADespues,
    bool ConflictoDetectado,
    IReadOnlyList<string> ComandosSql);

public sealed record TokenPropiedadM5Dto(
    string EstadoAInicial,
    string EstadoBInicial,
    string EstadoADespues,
    bool ConflictoDetectado,
    IReadOnlyList<string> ComandosSql);

public sealed record IndiceRowVersionM5Dto(bool ExisteIndiceRowVersion);
