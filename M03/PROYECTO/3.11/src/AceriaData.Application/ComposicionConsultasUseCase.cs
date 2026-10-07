using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ComposicionConsultasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ComposicionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== COMPOSICIÓN DE CONSULTAS ===");
        var resultado = _unidad.Ordenes.BuscarOrdenes("Constructora del Norte", "Pendiente", new DateTime(2024,1,1), "fecha", true, 1, 10);
        if (resultado.Elementos.Count != 2) throw new InvalidOperationException("Consulta compuesta inesperada.");
        Console.WriteLine($"Resultados: {resultado.Elementos.Count}");
        Console.WriteLine(resultado.Sql);

        /*
        // ERROR CONTROLADO M03 3.11 - SKIP TAKE SIN ORDEN DETERMINISTA
        // Demuestra el Paso 9: la paginación puede traducirse, pero sin un OrderBy de negocio no es estable.
        var sinOrden = _unidad.Ordenes.BuscarOrdenesSinOrdenDiagnostico(1, 2);
        if (sinOrden.Elementos.Count != 2)
            throw new InvalidOperationException("Error controlado 3.11: la página sin orden no devolvió dos filas.");
        Console.WriteLine($"Error controlado 3.11 OK | Página sin orden: {sinOrden.Elementos.Count}");
        Console.WriteLine("SQL_SIN_ORDEN_INICIO");
        Console.WriteLine(sinOrden.Sql);
        Console.WriteLine("SQL_SIN_ORDEN_FIN");
        */

        /*
        // RETO M03 3.11 - FILTROS OPCIONALES Y SEGUNDA ORDENACION
        // Demuestra el Paso 10 sin materializar antes de Skip/Take.
        var retoFecha = _unidad.Ordenes.BuscarOrdenes(
            "Constructora del Norte", "Pendiente", null, "fecha", true, 1, 2);
        var retoCliente = _unidad.Ordenes.BuscarOrdenes(
            null, "Pendiente", null, "cliente", false, 1, 2);

        if (retoFecha.Elementos.Count != 2 || retoCliente.Elementos.Count != 2)
            throw new InvalidOperationException("Reto 3.11: cardinalidad inesperada.");

        Console.WriteLine("Reto 3.11 OK | Norte+Pendiente fecha desc: 2 | Pendiente cliente asc: 2");
        Console.WriteLine("RETO_FECHA_SQL_INICIO");
        Console.WriteLine(retoFecha.Sql);
        Console.WriteLine("RETO_FECHA_SQL_FIN");
        Console.WriteLine("RETO_CLIENTE_SQL_INICIO");
        Console.WriteLine(retoCliente.Sql);
        Console.WriteLine("RETO_CLIENTE_SQL_FIN");
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
// public sealed class ComposicionConsultasUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public ComposicionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== COMPOSICIÓN DE CONSULTAS ===");
//         var resultado = _unidad.Ordenes.BuscarOrdenes("Constructora del Norte", "Pendiente", new DateTime(2024,1,1), "fecha", true, 1, 10);
//         if (resultado.Elementos.Count != 2) throw new InvalidOperationException("Consulta compuesta inesperada.");
//         Console.WriteLine($"Resultados: {resultado.Elementos.Count}");
//         Console.WriteLine(resultado.Sql);
//     }
// }
// ============================================================================
