namespace AceriaData.Application.Dtos;

public sealed record MigracionesProduccionM5Dto(
    int MigracionesAplicadas,
    int MigracionesPendientes,
    string UltimaMigracion,
    bool TablaHistorialPersonalizadaExiste,
    string TablaHistorial);
