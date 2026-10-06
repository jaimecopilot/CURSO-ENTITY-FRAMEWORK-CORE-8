using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SplitQueriesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");
        var single = _unidad.Ordenes.MedirSingleQueryM4();
        var split = _unidad.Ordenes.MedirSplitQueryM4();

        if (single.ConsultasSql != 1)
            throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");
        if (single.Ordenes != split.Ordenes ||
            single.Planchas != split.Planchas ||
            single.RelacionesAleacion != split.RelacionesAleacion)
            throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");

        Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");
        Console.WriteLine("--- ToQueryString SingleQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());
        Console.WriteLine("--- ToQueryString SplitQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());
    }

    /*
    // ERROR CONTROLADO M04 4.8 - INFERIR ROUNDTRIPS DESDE TOQUERYSTRING
    public void EjecutarDiagnosticoToQueryString()
    {
        var split = _unidad.Ordenes.MedirSplitQueryM4();
        var sqlMostrado = _unidad.Ordenes.ObtenerSqlSplitQueryM4();
        var selectsVisibles = System.Text.RegularExpressions.Regex.Matches(
            sqlMostrado,
            @"\bSELECT\b",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count;

        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.8 diagnóstico: el interceptor esperaba 3 comandos y observó {split.ConsultasSql}.");

        Console.WriteLine(
            $"Diagnostico 4.8 OK | comandos reales={split.ConsultasSql} | SELECT visibles en ToQueryString={selectsVisibles}");
        Console.WriteLine(
            "ToQueryString describe la forma SQL para diagnóstico; el interceptor mide los comandos realmente ejecutados.");
    }
    */

    /*
    // RETO M04 4.8 - SPLITQUERY GLOBAL Y OVERRIDE LOCAL
    public void EjecutarRetoSplitGlobal()
    {
        var global = _unidad.Ordenes.MedirComportamientoGlobalM4();
        var single = _unidad.Ordenes.MedirSingleQueryM4();

        if (global.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"Reto 4.8: con SplitQuery global se esperaban 3 comandos y se obtuvieron {global.ConsultasSql}.");
        if (single.ConsultasSql != 1)
            throw new InvalidOperationException(
                $"Reto 4.8: AsSingleQuery no sobrescribió el comportamiento global; comandos={single.ConsultasSql}.");
        if (global.Ordenes != single.Ordenes ||
            global.Planchas != single.Planchas ||
            global.RelacionesAleacion != single.RelacionesAleacion)
            throw new InvalidOperationException(
                "Reto 4.8: el comportamiento global y el override local no materializaron el mismo grafo.");

        Console.WriteLine(
            $"Reto 4.8 OK | Split global={global.ConsultasSql} | override AsSingleQuery={single.ConsultasSql} | grafo={global.Ordenes}/{global.Planchas}/{global.RelacionesAleacion}");
        Console.WriteLine(
            "Señal diagnóstica: vigilar el número real de comandos y no convertir SplitQuery global en una regla ciega.");
    }
    */

}


// FRAGMENTO PDF M04 4.8 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class SplitQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");
//         var single = _unidad.Ordenes.MedirSingleQueryM4();
//         var split = _unidad.Ordenes.MedirSplitQueryM4();
//
//         if (single.ConsultasSql != 1)
//             throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");
//         if (split.ConsultasSql != 3)
//             throw new InvalidOperationException(
//                 $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");
//         if (single.Ordenes != split.Ordenes ||
//             single.Planchas != split.Planchas ||
//             single.RelacionesAleacion != split.RelacionesAleacion)
//             throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");
//
//         Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");
//         Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");
//         Console.WriteLine("--- ToQueryString SingleQuery ---");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());
//         Console.WriteLine("--- ToQueryString SplitQuery ---");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());
//     }
// }
// ========================================================================
