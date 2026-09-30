namespace AceriaData.Application.Dtos;

public sealed record ValorConflictoM5Dto(
    string Propiedad,
    string ValorOriginal,
    string ValorActual,
    string ValorBaseDeDatos);

public sealed record ResolucionConflictoM5Dto(
    string Estrategia,
    bool ConflictoDetectado,
    string ClienteFinal,
    string EstadoFinal,
    int Intentos,
    IReadOnlyList<ValorConflictoM5Dto> Valores,
    IReadOnlyList<string> ComandosSql);

public sealed record EliminacionConcurrenteM5Dto(
    bool ConflictoDetectado,
    bool EntidadYaNoExiste,
    bool EntidadDesacoplada,
    IReadOnlyList<string> ComandosSql);
