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

    /*
    // ERROR CONTROLADO M04 4.2 - ESTADO PREVIO DEL CHANGETRACKER
    public void EjecutarErrorEstadoPrevio()
    {
        var con = _unidad.Ordenes.MedirConsultaConTrackingM4();
        var sinAislada = _unidad.Ordenes.MedirConsultaSinTrackingM4();

        if (!string.Equals(con.Sql, sinAislada.Sql, StringComparison.Ordinal))
            throw new InvalidOperationException("4.2 error controlado: tracking cambio inesperadamente el SQL.");

        var conOtraVez = _unidad.Ordenes.MedirConsultaConTrackingM4();
        var contaminada = _unidad.Ordenes.MedirConsultaSinTrackingSinLimpiarM4();

        if (conOtraVez.EntidadesRastreadas == 0 || contaminada.EntidadesRastreadas == 0)
            throw new InvalidOperationException("4.2 error controlado: no se observó el estado previo del ChangeTracker.");

        Console.WriteLine(
            $"Error controlado 4.2 OK | SQL equivalente: True | rastreadas heredadas={contaminada.EntidadesRastreadas}");
    }
    */

    /*
    // RETO M04 4.2 - GRAFO DE ENTIDADES RELACIONADAS
    public void EjecutarRetoGrafo()
    {
        var con = _unidad.Ordenes.MedirGrafoConTrackingM4();
        var sin = _unidad.Ordenes.MedirGrafoSinTrackingM4();

        if (con.Filas == 0 || con.EntidadesRastreadas <= con.Filas)
            throw new InvalidOperationException("Reto 4.2: el grafo con tracking no dejó visibles las entidades relacionadas.");
        if (sin.Filas != con.Filas || sin.EntidadesRastreadas != 0)
            throw new InvalidOperationException("Reto 4.2: el grafo AsNoTracking dejó entidades rastreadas.");

        Console.WriteLine(
            $"Reto 4.2 OK | filas={con.Filas} | grafo rastreado={con.EntidadesRastreadas} | sin tracking={sin.EntidadesRastreadas}");
    }
    */

}


// FRAGMENTO PDF M04 4.2 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class TrackingUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public TrackingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.2 TRACKING Y NO TRACKING ===");
//         var con = _unidad.Ordenes.MedirConsultaConTrackingM4();
//         var sin = _unidad.Ordenes.MedirConsultaSinTrackingM4();
//
//         if (con.Filas == 0 || con.EntidadesRastreadas != con.Filas)
//             throw new InvalidOperationException("4.2: tracking no produjo el numero esperado de entradas.");
//         if (sin.Filas != con.Filas || sin.EntidadesRastreadas != 0)
//             throw new InvalidOperationException("4.2: AsNoTracking dejo entidades rastreadas.");
//
//         Console.WriteLine($"Con tracking: filas={con.Filas}, rastreadas={con.EntidadesRastreadas}");
//         Console.WriteLine($"Sin tracking: filas={sin.Filas}, rastreadas={sin.EntidadesRastreadas}");
//         Console.WriteLine("El SQL puede ser equivalente; la diferencia relevante esta en la materializacion y el ChangeTracker.");
//     }
// }
// ========================================================================
