using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SolucionesNMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");
        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
            throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");

        Console.WriteLine($"Include: {include.ConsultasSql} consulta.");
        Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");
    }

    /*
    // ERROR CONTROLADO M04 4.5 - EVITAR N+1 NO IMPLICA UNA CONSULTA
    public void EjecutarDiagnosticoNoTodoEsUnaConsulta()
    {
        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
            throw new InvalidOperationException("4.5 diagnóstico: Include/proyección no tienen la forma esperada.");
        if (split.ConsultasSql == 1)
            throw new InvalidOperationException("4.5 diagnóstico: SplitQuery se ha confundido con una única consulta.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.5 diagnóstico: se esperaban 3 comandos SplitQuery y se obtuvieron {split.ConsultasSql}.");

        Console.WriteLine(
            $"Error controlado 4.5 OK | evitar N+1 no implica 1 comando | Include={include.ConsultasSql} | Proyeccion={proyeccion.ConsultasSql} | Split={split.ConsultasSql}");
    }
    */

    /*
    // RETO M04 4.5 - GRAFO COMPLETO SPLIT QUERY
    public void EjecutarRetoGrafoCompleto()
    {
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (split.Ordenes != 5)
            throw new InvalidOperationException($"Reto 4.5: se esperaban 5 órdenes y se obtuvieron {split.Ordenes}.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException($"Reto 4.5: se esperaban 3 comandos y se obtuvieron {split.ConsultasSql}.");
        if (split.ElementosRelacionados != 9)
            throw new InvalidOperationException(
                $"Reto 4.5: se esperaban 9 elementos relacionados y se obtuvieron {split.ElementosRelacionados}.");

        Console.WriteLine(
            $"Reto 4.5 OK | Ordenes={split.Ordenes} | Relacionados={split.ElementosRelacionados} | Consultas SQL={split.ConsultasSql}");
        Console.WriteLine(
            "Justificacion: consulta raiz + colección Planchas + colección OrdenesAleaciones; ThenInclude(Aleacion) viaja con la consulta de esa colección.");
    }
    */

}


// FRAGMENTO PDF M04 4.5 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class SolucionesNMasUnoUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");
//         var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
//         var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
//         var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();
//
//         if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
//             throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");
//         if (split.ConsultasSql != 3)
//             throw new InvalidOperationException(
//                 $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");
//
//         Console.WriteLine($"Include: {include.ConsultasSql} consulta.");
//         Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");
//         Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");
//     }
// }
// ========================================================================
