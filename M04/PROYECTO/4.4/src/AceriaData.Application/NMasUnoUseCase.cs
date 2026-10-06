using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class NMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");
        var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();

        if (metrica.ConsultasSql != metrica.Ordenes + 1)
            throw new InvalidOperationException(
                $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");

        Console.WriteLine(
            $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");
        Console.WriteLine(
            "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.");
    }

    /*
    // ERROR CONTROLADO M04 4.4 - MEDICION CONTAMINADA POR ESTADO PREVIO
    public void EjecutarErrorContadorSinReset()
    {
        var limpia = _unidad.Ordenes.EjecutarNMasUnoM4();
        var contaminada = _unidad.Ordenes.EjecutarNMasUnoSinResetM4();

        if (limpia.ConsultasSql != limpia.Ordenes + 1)
            throw new InvalidOperationException("4.4 error controlado: la medicion limpia no representa N+1.");
        if (contaminada.ConsultasSql <= contaminada.Ordenes + 1)
            throw new InvalidOperationException("4.4 error controlado: el contador sin Reset no quedó contaminado.");

        Console.WriteLine(
            $"Error controlado 4.4 OK | limpio={limpia.ConsultasSql} | sin reset={contaminada.ConsultasSql}");
    }
    */

    /*
    // RETO M04 4.4 - DETALLE POR ORDEN
    public void EjecutarRetoDetalle()
    {
        var metrica = _unidad.Ordenes.EjecutarNMasUnoDetalleM4();

        if (metrica.ConsultasSql != metrica.Ordenes + 1)
            throw new InvalidOperationException(
                $"Reto 4.4: se esperaban N+1 comandos; obtenidos {metrica.ConsultasSql} para N={metrica.Ordenes}.");

        Console.WriteLine(
            $"Reto 4.4 OK | Ordenes={metrica.Ordenes} | Detalles={metrica.Detalles} | Consultas SQL={metrica.ConsultasSql}");
        Console.WriteLine(
            "Comparacion conceptual: una carga anticipada o una proyeccion puede evitar la consulta adicional por orden; se implementa en 4.5.");
    }
    */

}


// FRAGMENTO PDF M04 4.4 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class NMasUnoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");
//         var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();
//
//         if (metrica.ConsultasSql != metrica.Ordenes + 1)
//             throw new InvalidOperationException(
//                 $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");
//
//         Console.WriteLine(
//             $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");
//         Console.WriteLine(
//             "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.");
//     }
// }
// ========================================================================
