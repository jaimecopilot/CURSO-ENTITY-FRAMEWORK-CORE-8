using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class IdentityResolutionUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public IdentityResolutionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===");
        var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();
        var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();

        if (sin.Filas <= sin.ClavesUnicas)
            throw new InvalidOperationException("4.3: el dataset no contiene una entidad relacionada repetida.");
        if (sin.InstanciasUnicas != sin.Filas)
            throw new InvalidOperationException("4.3: AsNoTracking resolvio identidades cuando no debia.");
        if (con.InstanciasUnicas != con.ClavesUnicas)
            throw new InvalidOperationException("4.3: AsNoTrackingWithIdentityResolution no deduplico por clave.");

        Console.WriteLine($"AsNoTracking: filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias={sin.InstanciasUnicas}");
        Console.WriteLine($"IdentityResolution: filas={con.Filas}, claves={con.ClavesUnicas}, instancias={con.InstanciasUnicas}");
    }
}
