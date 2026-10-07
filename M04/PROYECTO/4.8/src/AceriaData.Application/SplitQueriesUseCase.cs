using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SplitQueriesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");
        var single = _unidad.Ordenes.MedirSingleQueryM4();
        var split = _unidad.Ordenes.MedirSplitQueryM4();

        if (single.ConsultasSql != 1)
            throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");
        if (single.Ordenes != split.Ordenes ||
            single.Planchas != split.Planchas ||
            single.RelacionesAleacion != split.RelacionesAleacion)
            throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");

        Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");
        Console.WriteLine("--- ToQueryString SingleQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());
        Console.WriteLine("--- ToQueryString SplitQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());
    }

    /*
    // ERROR CONTROLADO M04 4.8 - INFERIR ROUNDTRIPS DESDE TOQUERYSTRING
    public void EjecutarDiagnosticoToQueryString()
    {
        var split = _unidad.Ordenes.MedirSplitQueryM4();
        var sqlMostrado = _unidad.Ordenes.ObtenerSqlSplitQueryM4();
        var selectsVisibles = System.Text.RegularExpressions.Regex.Matches(
            sqlMostrado,
            @"\bSELECT\b",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count;

        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.8 diagnóstico: el interceptor esperaba 3 comandos y observó {split.ConsultasSql}.");

        Console.WriteLine(
            $"Diagnostico 4.8 OK | comandos reales={split.ConsultasSql} | SELECT visibles en ToQueryString={selectsVisibles}");
        Console.WriteLine(
            "ToQueryString describe la forma SQL para diagnóstico; el interceptor mide los comandos realmente ejecutados.");
    }
    */

    /*
    // RETO M04 4.8 - SPLITQUERY GLOBAL Y OVERRIDE LOCAL
    public void EjecutarRetoSplitGlobal()
    {
        var global = _unidad.Ordenes.MedirComportamientoGlobalM4();
        var single = _unidad.Ordenes.MedirSingleQueryM4();

        if (global.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"Reto 4.8: con SplitQuery global se esperaban 3 comandos y se obtuvieron {global.ConsultasSql}.");
        if (single.ConsultasSql != 1)
            throw new InvalidOperationException(
                $"Reto 4.8: AsSingleQuery no sobrescribió el comportamiento global; comandos={single.ConsultasSql}.");
        if (global.Ordenes != single.Ordenes ||
            global.Planchas != single.Planchas ||
            global.RelacionesAleacion != single.RelacionesAleacion)
            throw new InvalidOperationException(
                "Reto 4.8: el comportamiento global y el override local no materializaron el mismo grafo.");

        Console.WriteLine(
            $"Reto 4.8 OK | Split global={global.ConsultasSql} | override AsSingleQuery={single.ConsultasSql} | grafo={global.Ordenes}/{global.Planchas}/{global.RelacionesAleacion}");
        Console.WriteLine(
            "Señal diagnóstica: vigilar el número real de comandos y no convertir SplitQuery global en una regla ciega.");
    }
    */

}


// FRAGMENTO PDF M04 4.8 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class SplitQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");
//         var single = _unidad.Ordenes.MedirSingleQueryM4();
//         var split = _unidad.Ordenes.MedirSplitQueryM4();
//
//         if (single.ConsultasSql != 1)
//             throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");
//         if (split.ConsultasSql != 3)
//             throw new InvalidOperationException(
//                 $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");
//         if (single.Ordenes != split.Ordenes ||
//             single.Planchas != split.Planchas ||
//             single.RelacionesAleacion != split.RelacionesAleacion)
//             throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");
//
//         Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");
//         Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");
//         Console.WriteLine("--- ToQueryString SingleQuery ---");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());
//         Console.WriteLine("--- ToQueryString SplitQuery ---");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.8 - BLOCK 02
// SECTION: Paso 4: Crear el caso de uso de Split Queries
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/SplitQueriesUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class SplitQueriesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public SplitQueriesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== SPLIT QUERIES ===");
//
//         DemostrarSingleQuery();
//         DemostrarSplitQuery();
//         CompararRendimiento();
//         MostrarSql();
//     }
//
//     private void DemostrarSingleQuery()
//     {
//         Console.WriteLine("\n--- Single Query (producto cartesiano) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConVariasColeccionesSingleQuery();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas | {orden.OrdenesAleaciones.Count} aleaciones");
//         }
//     }
//
//     private void DemostrarSplitQuery()
//     {
//         Console.WriteLine("\n--- Split Query (sin producto cartesiano) ---");
//
//         var cronometro = Stopwatch.StartNew();
//         var ordenes = _unidad.Ordenes.ObtenerConVariasColeccionesSplitQuery();
//         cronometro.Stop();
//
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Tiempo: {cronometro.ElapsedMilliseconds} ms");
//         foreach (var orden in ordenes)
//         {
//             Console.WriteLine($"  {orden.NumeroOrden}: {orden.Planchas.Count} planchas | {orden.OrdenesAleaciones.Count} aleaciones");
//         }
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroSingle = Stopwatch.StartNew();
//         var ordenesSingle = _unidad.Ordenes.ObtenerConVariasColeccionesSingleQuery();
//         cronometroSingle.Stop();
//
//         var cronometroSplit = Stopwatch.StartNew();
//         var ordenesSplit = _unidad.Ordenes.ObtenerConVariasColeccionesSplitQuery();
//         cronometroSplit.Stop();
//
//         Console.WriteLine($"Single Query: {cronometroSingle.ElapsedMilliseconds} ms | Órdenes: {ordenesSingle.Count}");
//         Console.WriteLine($"Split Query: {cronometroSplit.ElapsedMilliseconds} ms | Órdenes: {ordenesSplit.Count}");
//     }
//
//     private void MostrarSql()
//     {
//         Console.WriteLine("\n--- SQL de Single Query ---");
//         var sqlSingle = _unidad.Ordenes.ObtenerSqlSingleQuery();
//         Console.WriteLine(sqlSingle);
//
//         Console.WriteLine("\n--- SQL de Split Query ---");
//         var sqlSplit = _unidad.Ordenes.ObtenerSqlSplitQuery();
//         Console.WriteLine(sqlSplit);
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.8 - BLOCK 08
// SECTION: Paso 1: Modificar el método OnConfiguring del AceriaDbContext:
// SOURCE TARGET: Paso 1: Modificar el método OnConfiguring del AceriaDbContext:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// {
//     if (!optionsBuilder.IsConfigured)
//     {
//         optionsBuilder
//             .UseSqlServer(
//                 "Server=(localdb)\\\mssqllocaldb;Database=AceriaDB;Trusted_Connection=True;",
//                 sqlOptions =>
//                 {
//                     sqlOptions.EnableRetryOnFailure(maxRetryCount: 5);
//                     sqlOptions.CommandTimeout(60);
//                     sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
//                 })
//             .ConfigureWarnings(warnings =>
//                 warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.8 - BLOCK 09
// SECTION: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// SOURCE TARGET: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// var ordenes = context.OrdenesFabricacion
//     .Include(o => o.Planchas)
//     .Include(o => o.OrdenesAleaciones)
//     .ToList();
// ========================================================================

// CANONICAL INLINE M04 4.8 - BLOCK 10
// SECTION: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// SOURCE TARGET: Paso 2: Ejecutar una consulta con dos colecciones sin AsSplitQuery:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// var ordenes = context.OrdenesFabricacion
//     .Include(o => o.Planchas)
//     .Include(o => o.OrdenesAleaciones)
//     .AsSplitQuery()
//     .ToList();
// Resultado esperado con la solución: la consulta se ejecuta sin error porque AsSplitQuery está aplicado explícitamente.
//
// ========================================================================
