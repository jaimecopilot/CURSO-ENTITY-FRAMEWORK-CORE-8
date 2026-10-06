using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class AnalisisSqlUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public AnalisisSqlUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.1 ANALISIS DEL SQL GENERADO ===");
        var pendientes = _unidad.Ordenes.ObtenerSqlPendientesOrdenadasM4();
        var include = _unidad.Ordenes.ObtenerSqlConIncludeM4();
        var proyeccion = _unidad.Ordenes.ObtenerSqlConProyeccionM4();

        if (!pendientes.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.1: el SQL de pendientes no contiene filtro.");
        if (!include.Contains("JOIN", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.1: el SQL con Include no contiene JOIN.");
        if (proyeccion.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.1: la proyeccion recupera columnas no solicitadas.");

        Console.WriteLine("--- SQL pendientes ---");
        Console.WriteLine(pendientes);
        Console.WriteLine("--- SQL Include ---");
        Console.WriteLine(include);
        Console.WriteLine("--- SQL proyeccion ---");
        Console.WriteLine(proyeccion);
    }

    /*
    // RETO M04 4.1 - ANTICIPAR DOS COLECCIONES SIN CONCLUIR RENDIMIENTO
    public void EjecutarRetoDosColecciones()
    {
        var sql = _unidad.Ordenes.ObtenerSqlDosColeccionesM4();
        var joins = System.Text.RegularExpressions.Regex.Matches(
            sql,
            @"\bJOIN\b",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count;

        if (joins < 2)
            throw new InvalidOperationException("Reto 4.1: se esperaban al menos dos JOIN al incluir dos colecciones.");

        Console.WriteLine($"Reto 4.1 OK | JOINs: {joins}");
        Console.WriteLine(sql);
    }
    */

}


// FRAGMENTO PDF M04 4.1 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class AnalisisSqlUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public AnalisisSqlUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.1 ANALISIS DEL SQL GENERADO ===");
//         var pendientes = _unidad.Ordenes.ObtenerSqlPendientesOrdenadasM4();
//         var include = _unidad.Ordenes.ObtenerSqlConIncludeM4();
//         var proyeccion = _unidad.Ordenes.ObtenerSqlConProyeccionM4();
//
//         if (!pendientes.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.1: el SQL de pendientes no contiene filtro.");
//         if (!include.Contains("JOIN", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.1: el SQL con Include no contiene JOIN.");
//         if (proyeccion.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.1: la proyeccion recupera columnas no solicitadas.");
//
//         Console.WriteLine("--- SQL pendientes ---");
//         Console.WriteLine(pendientes);
//         Console.WriteLine("--- SQL Include ---");
//         Console.WriteLine(include);
//         Console.WriteLine("--- SQL proyeccion ---");
//         Console.WriteLine(proyeccion);
//     }
// }
// ========================================================================
