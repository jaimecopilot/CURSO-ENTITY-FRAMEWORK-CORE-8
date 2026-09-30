using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TrackingUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public TrackingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.2 TRACKING Y NO TRACKING ===");
        var con = _unidad.Ordenes.MedirConsultaConTrackingM4();
        var sin = _unidad.Ordenes.MedirConsultaSinTrackingM4();

        if (con.Filas == 0 || con.EntidadesRastreadas != con.Filas)
            throw new InvalidOperationException("4.2: tracking no produjo el numero esperado de entradas.");
        if (sin.Filas != con.Filas || sin.EntidadesRastreadas != 0)
            throw new InvalidOperationException("4.2: AsNoTracking dejo entidades rastreadas.");

        Console.WriteLine($"Con tracking: filas={con.Filas}, rastreadas={con.EntidadesRastreadas}");
        Console.WriteLine($"Sin tracking: filas={sin.Filas}, rastreadas={sin.EntidadesRastreadas}");
        Console.WriteLine("El SQL puede ser equivalente; la diferencia relevante esta en la materializacion y el ChangeTracker.");
    }
}
