using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface IMigracionesProduccionM5Repositorio
{
    Task<MigracionesProduccionM5Dto> AplicarConIMigratorAsync();
}
