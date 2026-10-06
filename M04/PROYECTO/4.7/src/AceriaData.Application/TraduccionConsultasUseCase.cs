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
        if (resultado.SqlAntesDeFrontera.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.7 error controlado: el SQL previo a AsEnumerable ya contenía un filtro y no demuestra la frontera temprana.");

        Console.WriteLine(
            $"Error controlado 4.7 OK | coincidencias={resultado.Coincidencias} | SQL antes de AsEnumerable sin WHERE");
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


// FRAGMENTO PDF M04 4.7 - PASO 5
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
