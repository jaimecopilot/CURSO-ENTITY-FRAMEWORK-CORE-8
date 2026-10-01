using AceriaData.Application.Dtos;

namespace AceriaData.Application.Interfaces;

public interface ITokensConcurrenciaM5Repositorio
{
    RowVersionM5Dto DemostrarRowVersion();
    TokenPropiedadM5Dto DemostrarTokenDePropiedad();
    IndiceRowVersionM5Dto ComprobarIndiceRowVersion();
}
