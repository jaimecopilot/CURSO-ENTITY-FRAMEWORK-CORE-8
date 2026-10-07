using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class DiagnosticoRendimientoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public DiagnosticoRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.11 DIAGNOSTICO DE RENDIMIENTO ===");
        var d = _unidad.Ordenes.DiagnosticarPendientesM4();

        if (d.Filas == 0 || d.ConsultasSql != 1 || d.EntidadesRastreadas != 0)
            throw new InvalidOperationException(
                "4.11: las metricas observables no coinciden con la consulta optimizada.");
        if (!d.Sql.Contains("M4.11-DIAGNOSTICO", StringComparison.Ordinal))
            throw new InvalidOperationException("4.11: falta TagWith en el SQL de diagnostico.");

        Console.WriteLine(
            $"Filas={d.Filas} | SQL commands={d.ConsultasSql} | Tracking={d.EntidadesRastreadas} | Ticks={d.Ticks}");
        Console.WriteLine(d.Sql);
    }

    /*
    // ERROR CONTROLADO M04 4.11 - CONTADOR MANUAL NO REPRESENTA ROUNDTRIPS
    public void EjecutarErrorContadorManual()
    {
        var m = _unidad.Ordenes.DiagnosticarConContadorManualM4();

        if (m.ContadorManual != 1 || m.ComandosReales != 2)
            throw new InvalidOperationException(
                "4.11 error controlado: el dataset no demuestra la divergencia entre contador manual y comandos reales.");

        Console.WriteLine(
            $"Error controlado 4.11 OK | contador manual={m.ContadorManual} | comandos reales={m.ComandosReales}");
    }
    */

    /*
    // RETO M04 4.11 - DIAGNOSTICLISTENER CON UMBRAL CONFIGURABLE
    public void EjecutarRetoDiagnosticListener()
    {
        var d = _unidad.Ordenes.DiagnosticarConDiagnosticListenerM4(0);

        if (d.Filas == 0 ||
            d.ComandosReales != 1 ||
            d.ComandosObservados != 1 ||
            d.ConsultasLentas != 1)
            throw new InvalidOperationException(
                "Reto 4.11: DiagnosticListener no observó el comando ejecutado como se esperaba.");

        Console.WriteLine(
            $"Reto 4.11 OK | DiagnosticListener | comandos={d.ComandosObservados} | lentas={d.ConsultasLentas} | umbral={d.UmbralMs:0} ms");
    }
    */
}


// EJEMPLO DEL PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class DiagnosticoRendimientoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public DiagnosticoRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.11 DIAGNOSTICO DE RENDIMIENTO ===");
//         var d = _unidad.Ordenes.DiagnosticarPendientesM4();
//
//         if (d.Filas == 0 || d.ConsultasSql != 1 || d.EntidadesRastreadas != 0)
//             throw new InvalidOperationException(
//                 "4.11: las metricas observables no coinciden con la consulta optimizada.");
//         if (!d.Sql.Contains("M4.11-DIAGNOSTICO", StringComparison.Ordinal))
//             throw new InvalidOperationException("4.11: falta TagWith en el SQL de diagnostico.");
//
//         Console.WriteLine(
//             $"Filas={d.Filas} | SQL commands={d.ConsultasSql} | Tracking={d.EntidadesRastreadas} | Ticks={d.Ticks}");
//         Console.WriteLine(d.Sql);
//     }
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 11
// SECTION: Paso 2: Añadir la demostración en el caso de uso:
// UBICACION EN EL EJERCICIO: Paso 2: Añadir la demostración en el caso de uso:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// private void DemostrarDeteccionConObservador()
// {
//     Console.WriteLine("\n--- Detección de consultas lentas con observador ---");
//
//     var observer = new EfCoreDiagnosticObserver(message => Console.WriteLine(message));
//     DiagnosticListener.AllListeners.Subscribe(observer);
//
//     var ordenes = _unidad.Ordenes.ObtenerTodasConDiagnostico();
//     var pendientes = _unidad.Ordenes.ObtenerPendientesConDiagnostico();
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 12
// SECTION: Paso 3: Llamar al método desde Ejecutar:
// UBICACION EN EL EJERCICIO: Paso 3: Llamar al método desde Ejecutar:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// DemostrarDeteccionConObservador();
// ========================================================================
