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

    /*
    // ERROR CONTROLADO M04 4.3 - PLANCHA NO DEMUESTRA IDENTITY RESOLUTION
    public void EjecutarErrorEntidadNoRepetida()
    {
        var sin = _unidad.Ordenes.MedirPlanchasSinResolucionM4();
        var con = _unidad.Ordenes.MedirPlanchasConResolucionM4();

        if (sin.Filas == 0 || sin.Filas != sin.ClavesUnicas || sin.InstanciasUnicas != sin.ClavesUnicas)
            throw new InvalidOperationException("4.3 error controlado: el contraejemplo de planchas no es único por clave.");
        if (con.Filas != sin.Filas || con.ClavesUnicas != sin.ClavesUnicas || con.InstanciasUnicas != sin.InstanciasUnicas)
            throw new InvalidOperationException("4.3 error controlado: IdentityResolution cambió un conjunto sin claves repetidas.");

        Console.WriteLine(
            $"Error controlado 4.3 OK | Plancha no demuestra resolución | filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias sin={sin.InstanciasUnicas}, con={con.InstanciasUnicas}");
    }
    */

    /*
    // RETO M04 4.3 - COMPARAR REFERENCIAS DE ALEACION COMPARTIDA
    public void EjecutarRetoReferencias()
    {
        var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();
        var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();

        var duplicaReferencias = sin.InstanciasUnicas > sin.ClavesUnicas;
        var reutilizaReferencias = con.InstanciasUnicas == con.ClavesUnicas;

        if (sin.Filas != 4 || sin.ClavesUnicas != 2 || sin.InstanciasUnicas != 4)
            throw new InvalidOperationException("Reto 4.3: se esperaban 4 relaciones, 2 claves y 4 referencias sin resolución.");
        if (con.Filas != 4 || con.ClavesUnicas != 2 || con.InstanciasUnicas != 2)
            throw new InvalidOperationException("Reto 4.3: se esperaban 4 relaciones, 2 claves y 2 referencias con resolución.");
        if (!duplicaReferencias || !reutilizaReferencias)
            throw new InvalidOperationException("Reto 4.3: la comparación por referencia no refleja la resolución de identidad.");

        Console.WriteLine(
            $"Reto 4.3 OK | 4 relaciones / 2 aleaciones | sin={sin.InstanciasUnicas} referencias | con={con.InstanciasUnicas} referencias");
    }
    */

}


// FRAGMENTO PDF M04 4.3 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class IdentityResolutionUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public IdentityResolutionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===");
//         var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();
//         var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();
//
//         if (sin.Filas <= sin.ClavesUnicas)
//             throw new InvalidOperationException("4.3: el dataset no contiene una entidad relacionada repetida.");
//         if (sin.InstanciasUnicas != sin.Filas)
//             throw new InvalidOperationException("4.3: AsNoTracking resolvio identidades cuando no debia.");
//         if (con.InstanciasUnicas != con.ClavesUnicas)
//             throw new InvalidOperationException("4.3: AsNoTrackingWithIdentityResolution no deduplico por clave.");
//
//         Console.WriteLine($"AsNoTracking: filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias={sin.InstanciasUnicas}");
//         Console.WriteLine($"IdentityResolution: filas={con.Filas}, claves={con.ClavesUnicas}, instancias={con.InstanciasUnicas}");
//     }
// }
// ========================================================================
