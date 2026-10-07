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


// EJEMPLO DEL PASO 5
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

// VARIANTE COMENTADA - BLOQUE 03
// PASO: Paso 4: Crear el caso de uso de análisis de SQL
// UBICACIÓN INDICADA: Crear el archivo src/AceriaData.Application/UseCases/AnalisisSqlUseCase.cs:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class AnalisisSqlUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public AnalisisSqlUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== ANÁLISIS DEL SQL GENERADO ===");
//
//         DemostrarSqlConsultasPendientes();
//         DemostrarSqlConsultasConInclude();
//         DemostrarSqlConsultasConProyeccion();
//         DemostrarSqlConsultasConFiltroGlobal();
//     }
//
//     private void DemostrarSqlConsultasPendientes()
//     {
//         Console.WriteLine("\n--- SQL de consultas pendientes ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasPendientes();
//         Console.WriteLine(sql);
//     }
//
//     private void DemostrarSqlConsultasConInclude()
//     {
//         Console.WriteLine("\n--- SQL de consultas con Include ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasConInclude();
//         Console.WriteLine(sql);
//     }
//
//     private void DemostrarSqlConsultasConProyeccion()
//     {
//         Console.WriteLine("\n--- SQL de consultas con proyección ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasConProyeccion();
//         Console.WriteLine(sql);
//     }
//
//     private void DemostrarSqlConsultasConFiltroGlobal()
//     {
//         Console.WriteLine("\n--- SQL de consultas con filtro global de Soft Delete ---");
//         var sql = _unidad.Ordenes.ObtenerSqlConsultasConFiltroGlobal();
//         Console.WriteLine(sql);
//     }
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 11
// PASO: Paso 3: Añadir la demostración en el caso de uso:
// UBICACIÓN INDICADA: Paso 3: Añadir la demostración en el caso de uso:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// private void DemostrarSqlConsultasConMultiplesInclude()
// {
//     Console.WriteLine("\n--- SQL de consultas con múltiples Include ---");
//     var sql = _unidad.Ordenes.ObtenerSqlConsultasConMultiplesInclude();
//     Console.WriteLine(sql);
// }
// ========================================================================

// VARIANTE COMENTADA - BLOQUE 12
// PASO: Paso 4: Llamar al método desde Ejecutar:
// UBICACIÓN INDICADA: Paso 4: Llamar al método desde Ejecutar:
// PARA PROBARLO: activa este fragmento en una copia de trabajo siguiendo las indicaciones del paso.
// ------------------------------------------------------------------------
// DemostrarSqlConsultasConMultiplesInclude();
// ========================================================================
