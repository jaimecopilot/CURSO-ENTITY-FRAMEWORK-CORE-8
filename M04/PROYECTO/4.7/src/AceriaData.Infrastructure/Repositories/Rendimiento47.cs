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


// EJEMPLO DEL PASO 4
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

// EJEMPLO COMPLEMENTARIO - BLOQUE 02
// SECTION: Paso 3: Implementar los métodos de consultas ineficientes en el repositorio
// UBICACION EN EL EJERCICIO: Modificar src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
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

// EJEMPLO COMPLEMENTARIO - BLOQUE 07
// SECTION: Paso 10: Diagnosticar un error común
// UBICACION EN EL EJERCICIO: Modificar el método ObtenerPorMetodoPersonalizado para usar el método personalizado directamente en Where:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
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

// EJEMPLO COMPLEMENTARIO - BLOQUE 08
// SECTION: Paso 10: Diagnosticar un error común
// UBICACION EN EL EJERCICIO: Paso 10: Diagnosticar un error común
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
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

// EJEMPLO COMPLEMENTARIO - BLOQUE 10
// SECTION: Paso 2: Implementar el método en OrdenRepositorio:
// UBICACION EN EL EJERCICIO: Paso 2: Implementar el método en OrdenRepositorio:
// COMO PROBARLO: copia comentada del ejemplo; descoméntala solo cuando el paso lo indique.
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
