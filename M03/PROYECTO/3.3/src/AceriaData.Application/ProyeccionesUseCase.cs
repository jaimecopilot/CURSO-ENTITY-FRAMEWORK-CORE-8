using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ProyeccionesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ProyeccionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== PROYECCIONES CON SELECT ===");
        var clientes = _unidad.Ordenes.ObtenerClientesUnicos();
        var resumenes = _unidad.Ordenes.ObtenerResumenes();
        var pendientes = _unidad.Ordenes.ObtenerResumenesPorEstado("Pendiente");
        var totales = _unidad.Ordenes.ObtenerOrdenesConTotales();
        if (clientes.Count != 3 || resumenes.Count != 5 || pendientes.Count != 3 || totales.Count != 5) throw new InvalidOperationException("Proyecciones inesperadas.");
        Console.WriteLine($"Clientes: {string.Join(", ", clientes)} | Resúmenes: {resumenes.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccion());

        /*
        // RETO M03 3.3 - PROYECCION MINIMA Y CLIENTES UNICOS
        // Demuestra el Paso 10 y el laboratorio adicional: Distinct estable,
        // DTO con nombre y comparación del SELECT mínimo frente a entidad completa.
        var clientesReto = _unidad.Ordenes.ObtenerClientesUnicos();
        var clientesEsperados = new[]
        {
            "Constructora del Este",
            "Constructora del Norte",
            "Constructora del Sur"
        };

        if (!clientesReto.SequenceEqual(clientesEsperados))
            throw new InvalidOperationException("Reto 3.3: los clientes únicos no son estables o contienen duplicados.");

        var resumenesReto = _unidad.Ordenes.ObtenerResumenes();
        if (resumenesReto.Count != 5 ||
            resumenesReto.Any(r => string.IsNullOrWhiteSpace(r.NumeroOrden) ||
                                   string.IsNullOrWhiteSpace(r.Cliente) ||
                                   string.IsNullOrWhiteSpace(r.Estado)))
            throw new InvalidOperationException("Reto 3.3: el DTO de resumen no contiene el shape esperado.");

        var sqlMinimo = _unidad.Ordenes.ObtenerSqlProyeccion();
        var sqlEntidad = _unidad.Ordenes.ObtenerSqlEntidadCompletaRetoProyeccion();

        Console.WriteLine($"Reto 3.3 OK | Clientes: {string.Join(", ", clientesReto)} | DTOs: {resumenesReto.Count}");
        Console.WriteLine("SQL_MINIMO_INICIO");
        Console.WriteLine(sqlMinimo);
        Console.WriteLine("SQL_MINIMO_FIN");
        Console.WriteLine("SQL_ENTIDAD_INICIO");
        Console.WriteLine(sqlEntidad);
        Console.WriteLine("SQL_ENTIDAD_FIN");
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
// public sealed class ProyeccionesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public ProyeccionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== PROYECCIONES CON SELECT ===");
//         var clientes = _unidad.Ordenes.ObtenerClientesUnicos();
//         var resumenes = _unidad.Ordenes.ObtenerResumenes();
//         var pendientes = _unidad.Ordenes.ObtenerResumenesPorEstado("Pendiente");
//         var totales = _unidad.Ordenes.ObtenerOrdenesConTotales();
//         if (clientes.Count != 3 || resumenes.Count != 5 || pendientes.Count != 3 || totales.Count != 5) throw new InvalidOperationException("Proyecciones inesperadas.");
//         Console.WriteLine($"Clientes: {string.Join(", ", clientes)} | Resúmenes: {resumenes.Count}");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccion());
//     }
// }
// ============================================================================
