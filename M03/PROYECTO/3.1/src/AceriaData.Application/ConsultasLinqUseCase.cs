using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConsultasLinqUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");
        var enMemoria = _unidad.Ordenes.ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList();
        var consulta = _unidad.Ordenes.Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion);
        Console.WriteLine("Consulta IQueryable construida: aún no se ha materializado.");
        var enSql = consulta.ToList();
        if (enMemoria.Count != 3 || enSql.Count != 3) throw new InvalidOperationException("Comparación IEnumerable/IQueryable inesperada.");
        Console.WriteLine($"Enumerable: {enMemoria.Count} | IQueryable: {enSql.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlFundamentos());

        /*
        // RETO M03 3.1 - FILTRO OPCIONAL SIN MATERIALIZAR
        // Activa este bloque junto con los bloques RETO 3.1 del puerto y del repositorio.
        // Demuestra el Paso 10 y el laboratorio adicional: segundo filtro opcional,
        // materialización al final, orden descendente y proyección mínima.
        string? estadoOpcional = "Pendiente";

        var consultaReto = _unidad.Ordenes.Consulta()
            .Where(o => o.Cliente == "Constructora del Norte");

        if (!string.IsNullOrWhiteSpace(estadoOpcional))
        {
            consultaReto = consultaReto.Where(o => o.Estado == estadoOpcional);
        }

        var resultadoReto = consultaReto
            .OrderByDescending(o => o.FechaCreacion)
            .Select(o => new { o.NumeroOrden, o.Cliente })
            .ToList();

        if (resultadoReto.Count != 2)
            throw new InvalidOperationException("Reto 3.1: se esperaban dos órdenes pendientes del Norte.");

        var sqlReto = _unidad.Ordenes.ObtenerSqlRetoFundamentos("Constructora del Norte", estadoOpcional);
        Console.WriteLine($"Reto 3.1 OK | Filas: {resultadoReto.Count}");
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
// public sealed class ConsultasLinqUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");
//         var enMemoria = _unidad.Ordenes.ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList();
//         var consulta = _unidad.Ordenes.Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion);
//         Console.WriteLine("Consulta IQueryable construida: aún no se ha materializado.");
//         var enSql = consulta.ToList();
//         if (enMemoria.Count != 3 || enSql.Count != 3) throw new InvalidOperationException("Comparación IEnumerable/IQueryable inesperada.");
//         Console.WriteLine($"Enumerable: {enMemoria.Count} | IQueryable: {enSql.Count}");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlFundamentos());
//     }
// }
// 
// ============================================================================
