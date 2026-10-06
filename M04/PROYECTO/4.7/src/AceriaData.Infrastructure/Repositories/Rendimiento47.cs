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
