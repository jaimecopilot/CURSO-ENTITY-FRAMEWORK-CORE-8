using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConsultasBasicasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ConsultasBasicasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== WHERE, ORDERBY Y THENBY ===");
        var norte = _unidad.Ordenes.ObtenerPendientesPorCliente("Constructora del Norte");
        var pendientes = _unidad.Ordenes.ObtenerPorEstadoOrdenadasPorFecha("Pendiente");
        var rango = _unidad.Ordenes.ObtenerPorRangoDeFechas(new DateTime(2024,1,1), new DateTime(2024,12,31));
        var ordenadas = _unidad.Ordenes.ObtenerPorClienteOrdenadas("Constructora del Norte");
        var combinada = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas("Constructora del Norte", new DateTime(2024,1,1), new DateTime(2024,12,31));
        if (norte.Count != 2 || pendientes.Count != 3 || rango.Count != 5 || ordenadas.Count != 3 || combinada.Count != 3) throw new InvalidOperationException("Resultados de filtros/ordenaciones inesperados.");
        if (!ordenadas.Select(o => o.NumeroOrden).SequenceEqual(new[] { "OF-2024-0004", "OF-2024-0003", "OF-2024-0001" })) throw new InvalidOperationException("ThenByDescending no produjo el orden esperado.");
        Console.WriteLine($"Norte pendientes: {norte.Count} | Pendientes: {pendientes.Count} | Rango: {rango.Count} | Norte ordenadas: {ordenadas.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlConsultaBasica());
        
        /*
        // ERROR CONTROLADO M03 3.2 - SEGUNDO ORDERBY SUSTITUYE EL PRIMERO
        // Demuestra el Paso 9: el segundo OrderBy reemplaza el criterio anterior.
        var ordenCorrecto = _unidad.Ordenes.Consulta()
            .Where(o => o.Cliente == "Constructora del Norte")
            .OrderBy(o => o.Estado)
            .ThenByDescending(o => o.FechaCreacion)
            .Select(o => o.NumeroOrden)
            .ToList();

        var segundoOrderBy = _unidad.Ordenes.Consulta()
            .Where(o => o.Cliente == "Constructora del Norte")
            .OrderBy(o => o.Estado)
            .OrderByDescending(o => o.FechaCreacion)
            .Select(o => o.NumeroOrden)
            .ToList();

        if (ordenCorrecto.SequenceEqual(segundoOrderBy))
            throw new InvalidOperationException("Error controlado 3.2: el segundo OrderBy no mostró la sustitución esperada.");

        Console.WriteLine($"Error controlado 3.2 OK | Correcto: {string.Join(",", ordenCorrecto)} | Segundo OrderBy: {string.Join(",", segundoOrderBy)}");
        */

        /*
        // RETO M03 3.2 - CLIENTE RANGO ORDEN Y SQL PARAMETRIZADO
        // Demuestra el Paso 10 y el laboratorio adicional con Constructora del Norte durante 2024.
        var desdeReto = new DateTime(2024, 1, 1);
        var hastaReto = new DateTime(2024, 12, 31);
        var resultadoReto = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas(
            "Constructora del Norte", desdeReto, hastaReto);

        var numerosReto = resultadoReto.Select(o => o.NumeroOrden).ToArray();
        var ordenEsperado = new[] { "OF-2024-0003", "OF-2024-0004", "OF-2024-0001" };

        if (!numerosReto.SequenceEqual(ordenEsperado))
            throw new InvalidOperationException("Reto 3.2: el orden por estado y fecha descendente no coincide con el dataset.");

        var sqlReto = _unidad.Ordenes.ObtenerSqlRetoConsultaBasica(
            "Constructora del Norte", desdeReto, hastaReto);

        Console.WriteLine($"Reto 3.2 OK | Orden: {string.Join(",", numerosReto)}");
        Console.WriteLine(sqlReto);
        */
    }
}

// ============================================================================
// EJEMPLO DEL PASO 4
// COPIA COMENTADA DEL BLOQUE DE LA PRÁCTICA PARA QUE PUEDAS PROBARLO.
// Para utilizarla, comenta temporalmente la implementación activa equivalente y descomenta esta copia en una rama o copia de trabajo.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class ConsultasBasicasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public ConsultasBasicasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== WHERE, ORDERBY Y THENBY ===");
//         var norte = _unidad.Ordenes.ObtenerPendientesPorCliente("Constructora del Norte");
//         var pendientes = _unidad.Ordenes.ObtenerPorEstadoOrdenadasPorFecha("Pendiente");
//         var rango = _unidad.Ordenes.ObtenerPorRangoDeFechas(new DateTime(2024,1,1), new DateTime(2024,12,31));
//         var ordenadas = _unidad.Ordenes.ObtenerPorClienteOrdenadas("Constructora del Norte");
//         var combinada = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas("Constructora del Norte", new DateTime(2024,1,1), new DateTime(2024,12,31));
//         if (norte.Count != 2 || pendientes.Count != 3 || rango.Count != 5 || ordenadas.Count != 3 || combinada.Count != 3) throw new InvalidOperationException("Resultados de filtros/ordenaciones inesperados.");
//         if (!ordenadas.Select(o => o.NumeroOrden).SequenceEqual(new[] { "OF-2024-0004", "OF-2024-0003", "OF-2024-0001" })) throw new InvalidOperationException("ThenByDescending no produjo el orden esperado.");
//         Console.WriteLine($"Norte pendientes: {norte.Count} | Pendientes: {pendientes.Count} | Rango: {rango.Count} | Norte ordenadas: {ordenadas.Count}");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlConsultaBasica());
//     }
// }
// ============================================================================
