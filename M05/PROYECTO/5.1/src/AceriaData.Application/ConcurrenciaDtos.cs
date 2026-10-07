namespace AceriaData.Application.Dtos;

public sealed record ConcurrenciaMismaPropiedadDto(
    string ClienteInicial,
    string ClienteUsuarioA,
    string ClienteUsuarioB,
    string ClienteFinal,
    bool CambioUsuarioAPerdido,
    IReadOnlyList<string> ComandosSql);

public sealed record ConcurrenciaPropiedadesDistintasDto(
    string ClienteInicial,
    string EstadoInicial,
    string ClienteFinal,
    string EstadoFinal,
    bool AmbosCambiosConservados,
    IReadOnlyList<string> ComandosSql);

/*
// RETO M05 5.1 - DTO TERCERA ESCRITURA
public sealed record ConcurrenciaTresEscriturasDto(
    string ClienteFinal,
    bool UltimaEscrituraPrevalece,
    int NumeroComandos,
    IReadOnlyList<string> ComandosSql);
*/
