using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private static bool EstadoCoincideM4(string actual, string buscado) =>
        string.Equals(actual, buscado, StringComparison.OrdinalIgnoreCase);

    public bool FiltroPersonalizadoNoTraducibleFallaM4(string estado)
    {
        try
        {
            _ = _context.OrdenesFabricacion
                .AsNoTracking()
                .Where(o => EstadoCoincideM4(o.Estado, estado))
                .ToList();
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    public int ContarConEvaluacionClienteExplicitaM4(string estado) =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .AsEnumerable()
            .Count(o => EstadoCoincideM4(o.Estado, estado));

    public string ObtenerSqlClienteConFuncionM4(string cliente)
    {
        var normalizado = cliente.ToLowerInvariant();
        return _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Cliente.ToLower() == normalizado)
            .ToQueryString();
    }

    public string ObtenerSqlClienteDirectoM4(string cliente) =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Cliente == cliente)
            .ToQueryString();

    /*
    // ERROR CONTROLADO M04 4.7 - FRONTERA CLIENTE DEMASIADO PRONTO
    public (int Coincidencias, string SqlAntesDeFrontera) ContarConFronteraClienteTempranaM4(string estado)
    {
        var consultaServidor = _context.OrdenesFabricacion
            .AsNoTracking();

        var sqlAntesDeFrontera = consultaServidor.ToQueryString();

        var coincidencias = consultaServidor
            .AsEnumerable()
            .Count(o => EstadoCoincideM4(o.Estado, estado));

        return (coincidencias, sqlAntesDeFrontera);
    }
    */

    /*
    // RETO M04 4.7 - VALIDACION DE FORMATO TRADUCIBLE
    public int ContarNumeroOrdenConFormatoTraducibleM4() =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .Count(o => o.NumeroOrden.StartsWith("OF-2024-") && o.NumeroOrden.Length == 12);

    public string ObtenerSqlNumeroOrdenConFormatoTraducibleM4() =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.NumeroOrden.StartsWith("OF-2024-") && o.NumeroOrden.Length == 12)
            .ToQueryString();
    */

}


// FRAGMENTO PDF M04 4.7 - PASO 4
// ------------------------------------------------------------------------
// using Microsoft.EntityFrameworkCore;
//
// namespace AceriaData.Infrastructure.Repositories;
//
// public sealed partial class OrdenRepositorio
// {
//     private static bool EstadoCoincideM4(string actual, string buscado) =>
//         string.Equals(actual, buscado, StringComparison.OrdinalIgnoreCase);
//
//     public bool FiltroPersonalizadoNoTraducibleFallaM4(string estado)
//     {
//         try
//         {
//             _ = _context.OrdenesFabricacion
//                 .AsNoTracking()
//                 .Where(o => EstadoCoincideM4(o.Estado, estado))
//                 .ToList();
//             return false;
//         }
//         catch (InvalidOperationException)
//         {
//             return true;
//         }
//     }
//
//     public int ContarConEvaluacionClienteExplicitaM4(string estado) =>
//         _context.OrdenesFabricacion
//             .AsNoTracking()
//             .AsEnumerable()
//             .Count(o => EstadoCoincideM4(o.Estado, estado));
//
//     public string ObtenerSqlClienteConFuncionM4(string cliente)
//     {
//         var normalizado = cliente.ToLowerInvariant();
//         return _context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Cliente.ToLower() == normalizado)
//             .ToQueryString();
//     }
//
//     public string ObtenerSqlClienteDirectoM4(string cliente) =>
//         _context.OrdenesFabricacion
//             .AsNoTracking()
//             .Where(o => o.Cliente == cliente)
//             .ToQueryString();
// }
// ========================================================================

// CANONICAL INLINE M04 4.7 - BLOCK 02
// SECTION: Paso 3: Implementar los métodos de consultas ineficientes en el repositorio
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorClienteConFuncion(string cliente)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente.ToLower() == cliente.ToLower())
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorClienteSinFuncion(string cliente)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente == cliente)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado)
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     return ordenes.Where(o => EsEstadoValido(o.Estado, estado)).ToList();
// }
//
// public List<OrdenFabricacion> ObtenerPorEstadoDirecto(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Estado == estado)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
//
// public string ObtenerSqlConFuncion(string cliente)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente.ToLower() == cliente.ToLower());
//
//     return consulta.ToQueryString();
// }
//
// public string ObtenerSqlSinFuncion(string cliente)
// {
//     var consulta = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.Cliente == cliente);
//
//     return consulta.ToQueryString();
// }
//
// private static bool EsEstadoValido(string estadoActual, string estadoBuscado)
// {
//     return string.Equals(estadoActual, estadoBuscado, StringComparison.OrdinalIgnoreCase);
// }
// ========================================================================

// CANONICAL INLINE M04 4.7 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ObtenerPorMetodoPersonalizado para usar el método personalizado directamente en Where:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado)
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => EsEstadoValido(o.Estado, estado))
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================

// CANONICAL INLINE M04 4.7 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorMetodoPersonalizado(string estado)
// {
//     var ordenes = _context.OrdenesFabricacion
//         .AsNoTracking()
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
//
//     return ordenes.Where(o => EsEstadoValido(o.Estado, estado)).ToList();
// }
// Resultado esperado con la solución: el método funciona y filtra en memoria. La desventaja es que carga todas las órdenes en memoria.
//
// ========================================================================

// CANONICAL INLINE M04 4.7 - BLOCK 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 2: Implementar el método en OrdenRepositorio:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public List<OrdenFabricacion> ObtenerPorFormatoNumeroOrden()
// {
//     return _context.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.NumeroOrden.StartsWith("OF-") && o.NumeroOrden.Length == 12)
//         .OrderBy(o => o.NumeroOrden)
//         .ToList();
// }
// ========================================================================
