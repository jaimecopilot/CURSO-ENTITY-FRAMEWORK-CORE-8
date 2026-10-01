using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure;

public sealed class BuenasPracticasAntiPatronesM5Diagnostico
{
    private readonly AceriaDbContext _context;

    public BuenasPracticasAntiPatronesM5Diagnostico(AceriaDbContext context) => _context = context;

    public BuenasPracticasAntiPatronesM5Resultado Ejecutar()
    {
        var nMasUno = MedirNMasUno();
        var overFetching = MedirOverFetching();
        var traduccion = MedirTraduccion();
        var matriz = CrearMatriz();

        return new BuenasPracticasAntiPatronesM5Resultado(
            nMasUno,
            overFetching,
            traduccion,
            matriz);
    }

    private NMasUnoRefactorM5Resultado MedirNMasUno()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var cabeceras = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, o.NumeroOrden })
            .ToList();

        var planchasNMasUno = 0;
        foreach (var orden in cabeceras)
        {
            planchasNMasUno += _context.PlanchasAcero
                .AsNoTracking()
                .Count(p => p.OrdenId == orden.Id);
        }

        var consultasNMasUno = checked((int)SqlCommandCounterInterceptor.Instance.Count);
        var trackingNMasUno = _context.ChangeTracker.Entries().Count();

        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consultaInclude = _context.OrdenesFabricacion
            .AsNoTracking()
            .Include(o => o.Planchas)
            .OrderBy(o => o.Id);

        var sqlInclude = consultaInclude.ToQueryString();
        var conInclude = consultaInclude.ToList();
        var consultasInclude = checked((int)SqlCommandCounterInterceptor.Instance.Count);
        var trackingInclude = _context.ChangeTracker.Entries().Count();
        var planchasInclude = conInclude.Sum(o => o.Planchas.Count);

        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consultaProyeccion = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new
            {
                o.Id,
                o.NumeroOrden,
                TotalPlanchas = o.Planchas.Count
            });

        var sqlProyeccion = consultaProyeccion.ToQueryString();
        var proyectadas = consultaProyeccion.ToList();
        var consultasProyeccion = checked((int)SqlCommandCounterInterceptor.Instance.Count);
        var trackingProyeccion = _context.ChangeTracker.Entries().Count();
        var planchasProyeccion = proyectadas.Sum(o => o.TotalPlanchas);

        var equivalentes =
            cabeceras.Count == conInclude.Count &&
            conInclude.Count == proyectadas.Count &&
            planchasNMasUno == planchasInclude &&
            planchasInclude == planchasProyeccion;

        return new NMasUnoRefactorM5Resultado(
            Ordenes: cabeceras.Count,
            Planchas: planchasNMasUno,
            ConsultasNMasUno: consultasNMasUno,
            ConsultasInclude: consultasInclude,
            ConsultasProyeccion: consultasProyeccion,
            TrackingNMasUno: trackingNMasUno,
            TrackingInclude: trackingInclude,
            TrackingProyeccion: trackingProyeccion,
            ResultadosEquivalentes: equivalentes,
            SqlInclude: sqlInclude,
            SqlProyeccion: sqlProyeccion);
    }

    private OverFetchingRefactorM5Resultado MedirOverFetching()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consultaCompleta = _context.OrdenesFabricacion
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.Id);

        var sqlCompleta = consultaCompleta.ToQueryString();
        var completas = consultaCompleta.ToList();
        var consultasCompletas = checked((int)SqlCommandCounterInterceptor.Instance.Count);
        var trackingCompleto = _context.ChangeTracker.Entries<OrdenFabricacion>().Count();
        var columnasEntidad = _context.Model
            .FindEntityType(typeof(OrdenFabricacion))!
            .GetProperties()
            .Count();

        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consultaProyectada = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.Id)
            .Select(o => new
            {
                o.NumeroOrden,
                o.Cliente,
                o.Estado,
                o.FechaCreacion
            });

        var sqlProyectada = consultaProyectada.ToQueryString();
        var proyectadas = consultaProyectada.ToList();
        var consultasProyectadas = checked((int)SqlCommandCounterInterceptor.Instance.Count);
        var trackingProyectado = _context.ChangeTracker.Entries().Count();

        var resumenCompleto = completas
            .Select(o => $"{o.NumeroOrden}|{o.Cliente}|{o.Estado}|{o.FechaCreacion:O}")
            .ToArray();
        var resumenProyectado = proyectadas
            .Select(o => $"{o.NumeroOrden}|{o.Cliente}|{o.Estado}|{o.FechaCreacion:O}")
            .ToArray();

        return new OverFetchingRefactorM5Resultado(
            Filas: completas.Count,
            ConsultasEntidadCompleta: consultasCompletas,
            ConsultasProyeccion: consultasProyectadas,
            ColumnasEntidadCompleta: columnasEntidad,
            ColumnasProyeccion: 4,
            TrackingEntidadCompleta: trackingCompleto,
            TrackingProyeccion: trackingProyectado,
            ResultadosEquivalentes: resumenCompleto.SequenceEqual(resumenProyectado),
            SqlEntidadCompletaIncluyeRowVersion: sqlCompleta.Contains("RowVersion", StringComparison.OrdinalIgnoreCase),
            SqlProyeccionExcluyeRowVersion: !sqlProyectada.Contains("RowVersion", StringComparison.OrdinalIgnoreCase),
            SqlEntidadCompleta: sqlCompleta,
            SqlProyeccion: sqlProyectada);
    }

    private TraduccionWhereM5Resultado MedirTraduccion()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var falloTraduccion = false;
        try
        {
            _ = _context.OrdenesFabricacion
                .AsNoTracking()
                .Where(o => EsPendiente(o.Estado))
                .ToList();
        }
        catch (InvalidOperationException)
        {
            falloTraduccion = true;
        }

        var comandosAntesCliente = SqlCommandCounterInterceptor.Instance.Count;

        SqlCommandCounterInterceptor.Instance.Reset();
        var pendientesCliente = _context.OrdenesFabricacion
            .AsNoTracking()
            .AsEnumerable()
            .Count(o => EsPendiente(o.Estado));
        var comandosCliente = checked((int)SqlCommandCounterInterceptor.Instance.Count);

        return new TraduccionWhereM5Resultado(
            MetodoNoTraducibleFalla: falloTraduccion,
            ComandosEmitidosAntesDelFallo: checked((int)comandosAntesCliente),
            EvaluacionClienteExplicitaFunciona: pendientesCliente > 0,
            ComandosEvaluacionCliente: comandosCliente,
            PendientesEvaluadosEnCliente: pendientesCliente);
    }

    private static bool EsPendiente(string estado) =>
        estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<PracticaAntiPatronM5> CrearMatriz() =>
    [
        new(
            "DbContext compartido de larga duración",
            "ChangeTracker crece y se comparte estado mutable",
            "Riesgo de concurrencia y estado obsoleto",
            "Scope por unidad de trabajo",
            "Un contexto corto simplifica aislamiento, pero crear scopes también tiene coste y debe alinearse con la unidad de trabajo."),
        new(
            "N+1",
            "Una consulta inicial seguida de una consulta por fila",
            "Más roundtrips",
            "Include o proyección según los datos realmente necesarios",
            "Include materializa entidades relacionadas; una proyección puede transferir menos datos."),
        new(
            "Over-fetching",
            "Se materializan entidades completas para una vista parcial",
            "Más columnas y tracking innecesario",
            "Proyección + AsNoTracking para lectura",
            "Si después se va a modificar la entidad, el tracking puede ser precisamente lo adecuado."),
        new(
            "AsSplitQuery automático",
            "Se divide siempre una consulta con varias colecciones",
            "Más roundtrips y posible diferencia temporal entre consultas",
            "Elegir single/split según forma y volumen del grafo",
            "Split puede reducir explosión cartesiana; single conserva un único roundtrip."),
        new(
            "Método .NET no traducible dentro de Where",
            "EF Core no puede traducir el predicado",
            "InvalidOperationException antes de ejecutar SQL",
            "Reescribir a una expresión traducible o cruzar explícitamente a evaluación cliente",
            "La evaluación cliente explícita puede transferir muchas más filas."),
        new(
            "Función sobre columna asumida siempre como anti-patrón",
            "Se aplica transformación a la columna en el predicado",
            "Puede perjudicar sargabilidad según proveedor, collation e índice",
            "Preferir comparaciones que permitan aprovechar el diseño del índice",
            "No toda función invalida todo índice; debe revisarse el plan real."),
        new(
            "Fluent API tratado como única opción válida",
            "Regla de estilo convertida en dogma",
            "Diseño innecesariamente rígido",
            "Elegir Fluent API/Data Annotations según separación y complejidad",
            "Fluent API centraliza y cubre más escenarios; Data Annotations pueden ser suficientes en modelos simples."),
        new(
            "Repository/UoW obligatorio",
            "Se añade abstracción aunque no exista necesidad arquitectónica",
            "Capas y delegaciones sin valor",
            "Usarlo cuando la frontera de aplicación lo justifique",
            "DbContext ya ofrece capacidades de Repository/UoW; AceriaData mantiene abstracciones por decisión de arquitectura."),
        new(
            "Migración aplicada reescrita",
            "El código histórico deja de representar bases ya desplegadas",
            "Divergencia de esquemas",
            "Nueva migración correctiva o rollback coordinado",
            "Un downgrade puede ser destructivo y no sustituye una estrategia de recuperación."),
        new(
            "Test con proveedor sustituto como prueba de SQL Server",
            "SQLite/InMemory se toma como equivalente",
            "Falsos positivos en SQL, tipos, collation o rowversion",
            "Pruebas de proveedor + integración LocalDB con migraciones reales",
            "Los proveedores ligeros siguen siendo útiles cuando el comportamiento probado no depende de SQL Server.")
    ];
}

public sealed record BuenasPracticasAntiPatronesM5Resultado(
    NMasUnoRefactorM5Resultado NMasUno,
    OverFetchingRefactorM5Resultado OverFetching,
    TraduccionWhereM5Resultado Traduccion,
    IReadOnlyList<PracticaAntiPatronM5> Matriz);

public sealed record NMasUnoRefactorM5Resultado(
    int Ordenes,
    int Planchas,
    int ConsultasNMasUno,
    int ConsultasInclude,
    int ConsultasProyeccion,
    int TrackingNMasUno,
    int TrackingInclude,
    int TrackingProyeccion,
    bool ResultadosEquivalentes,
    string SqlInclude,
    string SqlProyeccion);

public sealed record OverFetchingRefactorM5Resultado(
    int Filas,
    int ConsultasEntidadCompleta,
    int ConsultasProyeccion,
    int ColumnasEntidadCompleta,
    int ColumnasProyeccion,
    int TrackingEntidadCompleta,
    int TrackingProyeccion,
    bool ResultadosEquivalentes,
    bool SqlEntidadCompletaIncluyeRowVersion,
    bool SqlProyeccionExcluyeRowVersion,
    string SqlEntidadCompleta,
    string SqlProyeccion);

public sealed record TraduccionWhereM5Resultado(
    bool MetodoNoTraducibleFalla,
    int ComandosEmitidosAntesDelFallo,
    bool EvaluacionClienteExplicitaFunciona,
    int ComandosEvaluacionCliente,
    int PendientesEvaluadosEnCliente);

public sealed record PracticaAntiPatronM5(
    string Caso,
    string EvidenciaOSintoma,
    string Consecuencia,
    string Refactor,
    string TradeOff);
