using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TraduccionConsultasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public TraduccionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===");

        if (!_unidad.Ordenes.FiltroPersonalizadoNoTraducibleFallaM4("Pendiente"))
            throw new InvalidOperationException("4.7: EF Core no rechazo el filtro personalizado no traducible.");

        var cliente = _unidad.Ordenes.ContarConEvaluacionClienteExplicitaM4("Pendiente");
        if (cliente <= 0)
            throw new InvalidOperationException("4.7: la evaluacion cliente explicita no devolvio datos.");

        var sqlFuncion = _unidad.Ordenes.ObtenerSqlClienteConFuncionM4("Constructora del Norte");
        var sqlDirecto = _unidad.Ordenes.ObtenerSqlClienteDirectoM4("Constructora del Norte");

        if (!sqlFuncion.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.7: no se observa LOWER en el SQL con funcion.");
        if (sqlDirecto.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.7: la comparacion directa introdujo LOWER inesperadamente.");

        Console.WriteLine("Filtro no traducible: InvalidOperationException observada.");
        Console.WriteLine($"Evaluacion cliente explicita: {cliente} filas coincidentes.");
        Console.WriteLine("--- SQL con funcion sobre columna ---");
        Console.WriteLine(sqlFuncion);
        Console.WriteLine("--- SQL con comparacion directa ---");
        Console.WriteLine(sqlDirecto);
    }

    /*
    // ERROR CONTROLADO M04 4.7 - ASENUMERABLE ANTES DEL FILTRO
    public void EjecutarErrorFronteraTemprana()
    {
        var resultado = _unidad.Ordenes.ContarConFronteraClienteTempranaM4("Pendiente");

        if (resultado.Coincidencias <= 0)
            throw new InvalidOperationException("4.7 error controlado: la evaluación cliente no devolvió coincidencias.");
        if (resultado.SqlAntesDeFrontera.Contains("Pendiente", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.7 error controlado: el predicado Pendiente llegó a SQL antes de AsEnumerable.");

        Console.WriteLine(
            $"Error controlado 4.7 OK | coincidencias={resultado.Coincidencias} | SQL previo conserva filtros globales pero no filtra Pendiente");
    }
    */

    /*
    // RETO M04 4.7 - FORMATO TRADUCIBLE EN SERVIDOR
    public void EjecutarRetoFormatoTraducible()
    {
        var filas = _unidad.Ordenes.ContarNumeroOrdenConFormatoTraducibleM4();
        var sql = _unidad.Ordenes.ObtenerSqlNumeroOrdenConFormatoTraducibleM4();

        if (filas != 5)
            throw new InvalidOperationException($"Reto 4.7: se esperaban 5 números de orden con formato válido y se obtuvieron {filas}.");
        if (!sql.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Reto 4.7: el filtro de formato no quedó en SQL.");
        if (!sql.Contains("LIKE", StringComparison.OrdinalIgnoreCase) &&
            !sql.Contains("LEFT", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Reto 4.7: StartsWith no aparece traducido en SQL.");
        if (!sql.Contains("LEN", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Reto 4.7: Length no aparece traducido en SQL.");

        Console.WriteLine($"Reto 4.7 OK | formato traducible en SQL | filas={filas}");
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
// public sealed class TraduccionConsultasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public TraduccionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===");
//
//         if (!_unidad.Ordenes.FiltroPersonalizadoNoTraducibleFallaM4("Pendiente"))
//             throw new InvalidOperationException("4.7: EF Core no rechazo el filtro personalizado no traducible.");
//
//         var cliente = _unidad.Ordenes.ContarConEvaluacionClienteExplicitaM4("Pendiente");
//         if (cliente <= 0)
//             throw new InvalidOperationException("4.7: la evaluacion cliente explicita no devolvio datos.");
//
//         var sqlFuncion = _unidad.Ordenes.ObtenerSqlClienteConFuncionM4("Constructora del Norte");
//         var sqlDirecto = _unidad.Ordenes.ObtenerSqlClienteDirectoM4("Constructora del Norte");
//
//         if (!sqlFuncion.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.7: no se observa LOWER en el SQL con funcion.");
//         if (sqlDirecto.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
//             throw new InvalidOperationException("4.7: la comparacion directa introdujo LOWER inesperadamente.");
//
//         Console.WriteLine("Filtro no traducible: InvalidOperationException observada.");
//         Console.WriteLine($"Evaluacion cliente explicita: {cliente} filas coincidentes.");
//         Console.WriteLine("--- SQL con funcion sobre columna ---");
//         Console.WriteLine(sqlFuncion);
//         Console.WriteLine("--- SQL con comparacion directa ---");
//         Console.WriteLine(sqlDirecto);
//     }
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 03
// SECTION: Paso 4: Crear el caso de uso de consultas ineficientes
// UBICACION EN EL EJERCICIO: Crear el archivo src/AceriaData.Application/UseCases/ConsultasIneficientesUseCase.cs:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ConsultasIneficientesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ConsultasIneficientesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CONSULTAS INEFICIENTES ===");
//
//         DemostrarConFuncion();
//         DemostrarSinFuncion();
//         DemostrarMetodoPersonalizado();
//         DemostrarEstadoDirecto();
//         CompararSql();
//     }
//
//     private void DemostrarConFuncion()
//     {
//         Console.WriteLine("\n--- Consulta con función en Where ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorClienteConFuncion("Constructora del Norte");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarSinFuncion()
//     {
//         Console.WriteLine("\n--- Consulta sin función en Where ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorClienteSinFuncion("Constructora del Norte");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarMetodoPersonalizado()
//     {
//         Console.WriteLine("\n--- Consulta con método personalizado ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorMetodoPersonalizado("Pendiente");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void DemostrarEstadoDirecto()
//     {
//         Console.WriteLine("\n--- Consulta con estado directo ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerPorEstadoDirecto("Pendiente");
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//     }
//
//     private void CompararSql()
//     {
//         Console.WriteLine("\n--- SQL con función ---");
//         var sqlConFuncion = _unidad.Ordenes.ObtenerSqlConFuncion("Constructora del Norte");
//         Console.WriteLine(sqlConFuncion);
//
//         Console.WriteLine("\n--- SQL sin función ---");
//         var sqlSinFuncion = _unidad.Ordenes.ObtenerSqlSinFuncion("Constructora del Norte");
//         Console.WriteLine(sqlSinFuncion);
//     }
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// UBICACION EN EL EJERCICIO: Paso 3: Añadir la demostración en el caso de uso:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// private void DemostrarFormatoNumeroOrden()
// {
//     Console.WriteLine("\n--- Formato de número de orden ---");
//
//     var ordenes = _unidad.Ordenes.ObtenerPorFormatoNumeroOrden();
//     Console.WriteLine($"Órdenes con formato válido: {ordenes.Count}");
//     foreach (var orden in ordenes)
//     {
//         Console.WriteLine($"  {orden.NumeroOrden}");
//     }
// }
// ========================================================================

// EJEMPLO COMPLEMENTARIO - BLOQUE 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// UBICACION EN EL EJERCICIO: Paso 4: Llamar al método desde Ejecutar:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
// ------------------------------------------------------------------------
// DemostrarFormatoNumeroOrden();
// ========================================================================
