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


// EJEMPLO DEL PASO 5
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

// VARIANTE COMENTADA - BLOQUE 03
// PASO: Paso 4: Crear el caso de uso de tracking
// UBICACIÓN INDICADA: Crear el archivo src/AceriaData.Application/UseCases/TrackingUseCase.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;public class TrackingUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public TrackingUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== TRACKING Y NO TRACKING ===");
//
//         DemostrarConTracking();
//         DemostrarSinTracking();
//         CompararRendimiento();
//     }
//
//     private void DemostrarConTracking()
//     {
//         Console.WriteLine("\n--- Con Tracking ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerConTracking();
//         var rastreadas = _unidad.Ordenes.ContarEntidadesRastreadas();
//
//         Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//         Console.WriteLine($"Entidades rastreadas: {rastreadas}");
//     }
//
//     private void DemostrarSinTracking()
//     {
//         Console.WriteLine("\n--- Sin Tracking ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerSinTracking();
//         var rastreadas = _unidad.Ordenes.ContarEntidadesRastreadas();
//
//         Console.WriteLine($"Órdenes cargadas: {ordenes.Count}");
//         Console.WriteLine($"Entidades rastreadas: {rastreadas}");
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroCon = Stopwatch.StartNew();
//         var ordenesCon = _unidad.Ordenes.ObtenerConTracking();
//         cronometroCon.Stop();
//
//         var cronometroSin = Stopwatch.StartNew();
//         var ordenesSin = _unidad.Ordenes.ObtenerSinTracking();
//         cronometroSin.Stop();
//
//         Console.WriteLine($"Con Tracking: {cronometroCon.ElapsedMilliseconds} ms | Órdenes: {ordenesCon.Count}");
//         Console.WriteLine($"Sin Tracking: {cronometroSin.ElapsedMilliseconds} ms | Órdenes: {ordenesSin.Count}");
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 11
// PASO: Paso 4: Llamar al método desde Ejecutar:
// UBICACIÓN INDICADA: Paso 4: Llamar al método desde Ejecutar:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// DemostrarTrackingConInclude();
// ========================================================================
