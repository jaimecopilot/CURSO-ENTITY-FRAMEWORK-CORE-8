from pathlib import Path
import html
import re

ROOT = Path(__file__).resolve().parents[1]
M4 = ROOT / "M04"
BT = chr(96)

POINTS = {
1: ("Análisis del SQL generado: ToQueryString y logging","AnalisisSqlUseCase.cs","Rendimiento41.cs",
["src/AceriaData.Infrastructure/DependencyInjection.cs"],
"ToQueryString inspecciona la representación SQL sin materializar; el logging muestra los comandos realmente ejecutados. Son herramientas complementarias y el manual definitivo usa el checkpoint validado.",
"Comparar el SQL de una entidad completa con el de una proyección y justificar qué columnas sobran.",
"ToQueryString es el plano previo; el logging es el registro de lo que realmente pasó por la línea."),
2: ("Tracking y No Tracking","TrackingUseCase.cs","Rendimiento42.cs",[],
"Tracking y NoTracking normalmente no cambian el SELECT: cambian sobre todo materialización y ChangeTracker. Un DTO puro sin entidades no se rastrea; una proyección que contenga entidades sí puede mantener tracking de esas entidades.",
"Explicar por qué dos consultas con SQL parecido pueden tener distinto coste de materialización.",
"Tracking es mantener una ficha viva de cada pieza; NoTracking es leerla sin abrir expediente."),
3: ("AsNoTracking y AsNoTrackingWithIdentityResolution","IdentityResolutionUseCase.cs","Rendimiento43.cs",[],
"La resolución de identidad solo se demuestra si la misma clave aparece repetida. AceriaData usa Aleacion porque una misma aleación está relacionada con varias órdenes; PlanchaAcero pertenece a una sola orden y no es una evidencia válida.",
"Predecir cuántas instancias habrá cuando cuatro relaciones apunten a dos aleaciones distintas.",
"La resolución de identidad evita crear dos fichas físicas para la misma clave dentro de una consulta."),
4: ("Problema N+1: identificación y causas","NMasUnoUseCase.cs","Rendimiento44.cs",
["src/AceriaData.Infrastructure/SqlCommandCounterInterceptor.cs"],
"La baseline 3.12 tiene Lazy Loading desactivado. El N+1 se provoca de forma explícita: una consulta para órdenes y una adicional por orden. Un interceptor cuenta DbCommand reales.",
"Calcular y después medir cuántos comandos se producen para N órdenes.",
"N+1 es pedir una lista y volver a la ventanilla una vez por cada elemento."),
5: ("Solución a N+1: Include, proyecciones y Split Queries","SolucionesNMasUnoUseCase.cs","Rendimiento45.cs",[],
"No existe una solución universal al N+1. Include sirve para grafos; una proyección cuando solo se necesitan campos concretos; SplitQuery puede reducir explosión cartesiana con varias colecciones a costa de más roundtrips.",
"Elegir entre Include, proyección o SplitQuery para tres escenarios y justificar el coste dominante.",
"Optimizar N+1 es decidir si conviene traer el expediente completo, un resumen o varios lotes coordinados."),
6: ("Over-fetching: causas y soluciones","OverFetchingUseCase.cs","Rendimiento46.cs",[],
"Over-fetching se diagnostica observando la forma real del SELECT. La práctica compara igual cardinalidad con entidad completa frente a proyección DTO.",
"Identificar en el SQL qué columnas desaparecen al proyectar y relacionarlo con transferencia y materialización.",
"Over-fetching es mover un palé entero cuando la siguiente estación solo necesita cuatro piezas."),
7: ("Consultas ineficientes: traducción y frontera cliente/servidor","TraduccionConsultasUseCase.cs","Rendimiento47.cs",[],
"En EF Core 8 un predicado no traducible dentro de Where no se evalúa silenciosamente en cliente: falla. La evaluación cliente exige una frontera explícita como AsEnumerable. Las funciones sobre columnas pueden perjudicar sargabilidad y deben medirse.",
"Comparar el SQL con LOWER(columna) frente a comparación directa y explicar qué debe medirse en SQL Server.",
"Una frontera cliente explícita es sacar las piezas de la máquina y continuar manualmente: se puede hacer, pero debe ser consciente."),
8: ("Split Queries: cuándo y cómo usarlas","SplitQueriesUseCase.cs","Rendimiento48.cs",[],
"SplitQuery ejecuta varios comandos y puede evitar explosión cartesiana. No implica una transacción independiente por subconsulta. Sin aislamiento adecuado puede no existir una instantánea consistente frente a cambios concurrentes.",
"Explicar por qué dos colecciones multiplican filas en SingleQuery y por qué SplitQuery intercambia volumen por roundtrips.",
"SingleQuery mezcla lotes en una hoja grande; SplitQuery los trae por separado y los ensambla por claves."),
9: ("Compiled Queries","CompiledQueriesUseCase.cs","Rendimiento49.cs",[],
"EF Core ya cachea consultas por forma. EF.CompileQuery evita parte del trabajo de búsqueda y preparación de EF; no almacena el plan de ejecución de SQL Server. Debe medirse en hot paths y no se exige ganar una microprueba aislada.",
"Justificar cuándo el coste evitado por CompileQuery puede importar frente a red y base de datos.",
"CompiledQuery guarda una ruta de preparación en EF; no reserva una vía dentro de SQL Server."),
10: ("Paginación eficiente: Skip/Take y keyset pagination","PaginacionUseCase.cs","Rendimiento410.cs",
["src/AceriaData.Console/DemoData.cs"],
"Toda paginación necesita orden totalmente determinista. AceriaData ordena por FechaCreacion e Id; keyset usa ambos valores como cursor. Offset es válido para saltos arbitrarios pero puede encarecerse con offsets altos.",
"Construir la condición seek para un orden compuesto FechaCreacion + Id.",
"Offset cuenta cajas desde el principio; keyset continúa desde la etiqueta exacta de la última caja vista."),
11: ("Diagnóstico con logs, métricas y herramientas","DiagnosticoRendimientoUseCase.cs","Rendimiento411.cs",[],
"Un tiempo aislado no prueba rendimiento. El diagnóstico reproducible combina SQL, comandos, filas, tracking y tiempo, y usa TagWith para correlación.",
"Definir qué métrica distinguiría roundtrips de materialización.",
"Diagnosticar es instrumentar la línea antes de cambiar la máquina."),
12: ("Estrategias de optimización y checklist de rendimiento","ChecklistRendimientoUseCase.cs","Rendimiento412.cs",[],
"El checklist no impone una clasificación universal. Primero se define la forma necesaria, después se observa SQL, roundtrips, materialización y tracking, y solo entonces se eligen o descartan técnicas.",
"Auditar una consulta y justificar tanto técnicas aplicadas como descartadas.",
"El checklist final no cambia todas las piezas de la máquina, solo las que la medición justifica.")
}

OFFICIAL = {
2:"https://learn.microsoft.com/ef/core/querying/tracking",
3:"https://learn.microsoft.com/ef/core/querying/tracking",
7:"https://learn.microsoft.com/ef/core/querying/client-eval",
8:"https://learn.microsoft.com/ef/core/querying/single-split-queries",
9:"https://learn.microsoft.com/ef/core/performance/advanced-performance-topics",
10:"https://learn.microsoft.com/ef/core/querying/pagination",
11:"https://learn.microsoft.com/ef/core/performance/efficient-querying"
}

SOURCE_COVERAGE = {
1: {
"focus": [
"ToQueryString antes de materializar y logging de comandos SQL.",
"Consultas con Where, OrderBy, Select e Include, incluyendo el filtro global de Soft Delete.",
"Análisis de múltiples Include como origen potencial de multiplicación de filas."
],
"adaptation": "El checkpoint valida ToQueryString, logging, filtro, Include y proyección. El reto de múltiples colecciones se conserva como puente hacia 4.8, donde se demuestra con dos colecciones reales.",
"challenge": "Construye mentalmente una consulta con dos colecciones incluidas y anticipa cómo crecerían las filas; compruébalo después en 4.8.",
"errors": [
"No materializar con ToList antes de pedir ToQueryString.",
"No exponer IQueryable desde Application.",
"No resolver servicios Scoped desde el proveedor raíz."
]},
2: {
"focus": [
"Tracking, AsTracking, AsNoTracking y coste del ChangeTracker.",
"Conteo de entidades rastreadas y comparación aislada entre consultas.",
"Tracking de grafos con entidades relacionadas."
],
"adaptation": "La fuente proponía contextos separados para aislar mediciones. AceriaData usa ChangeTracker.Clear() antes de cada escenario, que elimina la contaminación entre mediciones dentro del E2E determinista.",
"challenge": "Carga un grafo con relaciones con y sin tracking y razona qué entidades quedarían registradas.",
"errors": [
"No interpretar SQL idéntico como coste idéntico de materialización.",
"No reutilizar estado previo del ChangeTracker al medir.",
"No registrar DbContext como Singleton."
]},
3: {
"focus": [
"AsNoTracking frente a AsNoTrackingWithIdentityResolution.",
"Conteo por referencia usando ReferenceEqualityComparer.",
"Escenario donde una misma clave aparece varias veces en el resultado."
],
"adaptation": "La fuente usaba planchas compartidas, pero PlanchaAcero pertenece a una sola orden. La práctica definitiva usa Aleacion, que sí es una entidad compartida por varias relaciones y permite demostrar identidad duplicada de forma real.",
"challenge": "Compara por referencia las instancias de una aleación compartida con y sin Identity Resolution.",
"errors": [
"No usar una entidad que nunca puede repetirse para demostrar resolución de identidad.",
"No confundir igualdad de clave con igualdad de referencia.",
"No dejar tracking previo activo durante la comparación."
]},
4: {
"focus": [
"Identificación de N+1, sus causas y relación con navegaciones.",
"Conteo real de comandos SQL y comparación con una alternativa sin N+1.",
"Variantes conceptuales con Lazy Loading, consultas en bucle, FirstOrDefault y proyecciones."
],
"adaptation": "Lazy Loading permanece desactivado en la baseline. Por eso el N+1 se provoca explícitamente mediante una consulta por orden y se mide con DbCommandInterceptor, sin depender de comportamiento oculto.",
"challenge": "Provoca N+1 al consultar detalle por orden y compáralo conceptualmente con una carga anticipada o proyección.",
"errors": [
"No asumir que acceder a una navegación ejecutará SQL cuando Lazy Loading está desactivado.",
"No inferir N+1 por intuición: contar comandos reales.",
"No mezclar estado previo del contexto en la medición."
]},
5: {
"focus": [
"Include y ThenInclude para cargar grafos.",
"Proyecciones para obtener solo los datos necesarios.",
"AsSplitQuery como alternativa cuando existen varias colecciones."
],
"adaptation": "El checkpoint compara alternativas contando comandos reales. SplitQuery no se presenta como regla universal: se usa en un grafo con dos colecciones donde el trade-off es observable.",
"challenge": "Combina Include, ThenInclude, Identity Resolution y SplitQuery en un grafo con planchas y aleaciones y justifica el número de comandos.",
"errors": [
"No aplicar SplitQuery por defecto sin observar la forma del grafo.",
"No comparar tiempos sin aislar tracking y dataset.",
"No confundir evitar N+1 con garantizar una única consulta."
]},
6: {
"focus": [
"Over-fetching de columnas y de filas.",
"Proyecciones, filtros y paginación para reducir datos transferidos.",
"Inspección del SQL para comparar entidad completa frente a shape reducido."
],
"adaptation": "El checkpoint 4.6 demuestra directamente el over-fetching de columnas con SQL real. El over-fetching de filas y la paginación se mantienen en teoría y se ejecutan de forma específica en 4.10.",
"challenge": "Compara el SELECT de entidad completa y proyección y relaciona las columnas eliminadas con transferencia y materialización.",
"errors": [
"No aplicar Skip sin un OrderBy determinista.",
"No materializar antes de terminar filtros y proyecciones.",
"No medir solo tiempo cuando el objetivo es demostrar volumen de datos."
]},
7: {
"focus": [
"Filtros no traducibles y frontera cliente/servidor.",
"Funciones aplicadas a columnas y posible pérdida de sargabilidad.",
"Reescritura de expresiones y uso de collation cuando corresponda."
],
"adaptation": "Se corrige la fuente: EF Core 8 no filtra silenciosamente en memoria dentro de Where. El checkpoint exige observar InvalidOperationException y solo después demuestra evaluación cliente explícita con AsEnumerable().",
"challenge": "Reescribe una validación de formato para usar operaciones traducibles y explica qué parte debe seguir ejecutándose en SQL.",
"errors": [
"No afirmar que un Where no traducible se ejecuta automáticamente en memoria.",
"No aplicar ToLower a la columna sin analizar el impacto sobre el índice.",
"No ocultar una frontera cliente implícita: hacerla explícita."
]},
8: {
"focus": [
"AsSingleQuery frente a AsSplitQuery con varias colecciones.",
"Explosión cartesiana, duplicación de datos y roundtrips.",
"Coherencia entre varios comandos y configuración global de Split Queries."
],
"adaptation": "La configuración global se conserva como contenido de estudio, pero no se activa en la baseline porque ocultaría la comparación docente. La coherencia se explica en términos de aislamiento/transacción, no como una transacción independiente por subconsulta.",
"challenge": "Analiza cómo cambiaría el comportamiento si SplitQuery fuera global y qué advertencias querrías convertir en señal de diagnóstico.",
"errors": [
"No afirmar que cada subconsulta de SplitQuery crea su propia transacción.",
"No afirmar que una sola colección nunca puede beneficiarse; evaluar volumen y roundtrips.",
"No comparar Single/Split con grafos distintos."
]},
9: {
"focus": [
"EF.CompileQuery y EF.CompileAsyncQuery, parámetros y proyecciones.",
"Caché interna de consultas de EF Core y coste que realmente evita una compiled query.",
"Medición en hot paths sin prometer una mejora universal."
],
"adaptation": "El checkpoint ejecutable usa una compiled query síncrona parametrizada para validar equivalencia. Async, proyección y variantes se conservan en teoría y como ampliación, sin inventar una ventaja temporal obligatoria.",
"challenge": "Diseña una compiled query proyectada y explica qué coste de EF evita frente al coste de red y SQL Server.",
"errors": [
"No compilar el delegado en cada llamada.",
"No afirmar que EF.CompileQuery almacena el plan de ejecución de SQL Server.",
"No usar un umbral de tiempo como condición de éxito del E2E."
]},
10: {
"focus": [
"Offset pagination con Skip/Take.",
"Keyset pagination con orden totalmente determinista.",
"Filtro, proyección y dirección de paginación."
],
"adaptation": "La fuente advertía del riesgo de usar solo fecha; el checkpoint lo corrige con cursor compuesto FechaCreacion + Id y añade datos suficientes para recorrer varias páginas.",
"challenge": "Añade mentalmente un filtro de estado a la paginación y conserva el mismo orden compuesto para no saltar ni repetir filas.",
"errors": [
"No paginar sin OrderBy.",
"No usar una clave de ordenación no única como cursor único.",
"No dejar tracking activo para listados paginados de solo lectura."
]},
11: {
"focus": [
"LogTo, categorías, niveles, ILoggerFactory, SensitiveDataLogging, DetailedErrors y ConfigureWarnings.",
"Tiempo, número de comandos, filas y tracking como métricas observables.",
"DiagnosticSource/DiagnosticListener y detección de consultas lentas."
],
"adaptation": "La fuente propone un DiagnosticObserver. La baseline validada usa LogTo + DbCommandInterceptor + TagWith para contar comandos y correlacionar consultas de forma determinista. DiagnosticSource se conserva en teoría y como ampliación, no se elimina silenciosamente.",
"challenge": "Diseña un observador de consultas lentas con un umbral configurable y explica qué aporta frente al interceptor de conteo.",
"errors": [
"No incrementar contadores manualmente dentro del repositorio.",
"No habilitar SensitiveDataLogging indiscriminadamente en producción.",
"No usar una única métrica temporal como diagnóstico completo."
]},
12: {
"focus": [
"Checklist, ciclo medir-identificar-aplicar-verificar-documentar y anti-patrones.",
"Métricas de tiempo, comandos, volumen, memoria y coste de materialización.",
"Estado acumulativo final de AceriaData y documentación de decisiones."
],
"adaptation": "Se conserva el checklist, pero se corrige la idea de aplicar todas las técnicas a toda consulta. El cierre exige justificar también por qué Include, SplitQuery o CompiledQuery no aplican a una consulta concreta.",
"challenge": "Audita una consulta completa y documenta cada decisión: aplicada, descartada y evidencia que la sustenta.",
"errors": [
"No optimizar antes de medir.",
"No forzar todas las técnicas del módulo sobre una misma consulta.",
"No considerar una micro-medición aislada como prueba concluyente."
]}
}

FINAL_PROJECT_STATE = [
"Arquitectura en cuatro proyectos: Domain, Application, Infrastructure y Console.",
"Dominio con OrdenFabricacion, PlanchaAcero, Aleacion, EstadoOrden, DetalleOrden, CertificadoCalidad y OrdenAleacion.",
"Application mantiene interfaces, DTOs y casos de uso sin depender de Microsoft.EntityFrameworkCore.",
"Infrastructure contiene AceriaDbContext, configuraciones Fluent, repositorios, UnitOfWork, migraciones y observabilidad de comandos.",
"Modelo con relaciones uno-a-muchos, uno-a-uno y muchos-a-muchos, claves e índices heredados.",
"Soft Delete y filtros globales heredados permanecen activos.",
"M4 añade análisis SQL, tracking/no-tracking, Identity Resolution, diagnóstico N+1, proyecciones, Split Queries, compiled queries, paginación y métricas.",
"Lazy Loading no se activa en M4: los escenarios que podrían producir N+1 se hacen explícitos y medibles."
]

def normalize(text):
    text = html.unescape(text).replace("&#x20;","")
    out=[]
    for raw in text.splitlines():
        line=raw.rstrip()
        if line.endswith("\\"):
            line=line[:-1].rstrip()
        line=line.replace("\\<","<").replace("\\>",">").replace("\\_","_")
        if line.startswith("\\"):
            line=line[1:]
        out.append(line)
    return "\n".join(out)

def load_source():
    a=(M4/"SOURCE"/"M04_FUENTE_4_1_4_6.txt").read_text(encoding="utf-8")
    b=(M4/"SOURCE"/"M04_FUENTE_4_7_4_12.md").read_text(encoding="utf-8")
    return normalize(a+"\n"+b)

def split_points(source):
    ms=list(re.finditer(r"(?m)^Punto 4\.(\d+)\s+[–-].*$",source))
    out={}
    for i,m in enumerate(ms):
        n=int(m.group(1))
        if n not in POINTS:
            continue
        end=ms[i+1].start() if i+1<len(ms) else len(source)
        out[n]=source[m.start():end].strip()
    return out

SOURCE_CODE_MARKERS = {"csharp","sql","bash","text","powershell","json","xml"}

SOURCE_PROSE_PREFIXES = (
    "La ", "El ", "Las ", "Los ", "Esta ", "Este ", "Estas ", "Estos ",
    "Resultado", "Observaciones", "Error común", "Solución", "A partir ",
    "En ", "Cuando ", "Como ", "Por ", "Aunque ", "Si ", "EF Core ",
    "Línea ", "Líneas "
)

def looks_like_source_heading(text):
    s=text.strip()
    if not s or len(s)>105:
        return False
    if s.lower() in SOURCE_CODE_MARKERS:
        return False
    if s.startswith(("//","#","- ","* ","1.","2.","3.","4.","5.","6.","7.","8.","9.")):
        return False
    if any(x in s for x in (";", "{", "}", "=>", "==", "!=", ".ToList", ".Where", ".Select", ".Include", "SELECT ", "FROM ", "WHERE ", "ORDER BY ")):
        return False
    return not s.endswith((".", ";", ",", ":"))

def source_code_fixes(code,n,lang):
    fixed=code.rstrip()
    if n==3 and lang=="csharp":
        fixed=fixed.replace(
            "// AsNoTracking: instancias duplicadas",
            "// AsNoTracking: puede crear instancias distintas si una misma clave reaparece"
        )
        fixed=fixed.replace(
            "// AsNoTrackingWithIdentityResolution: instancias únicas",
            "// IdentityResolution: reutiliza una instancia por clave dentro de esta consulta"
        )
    if n==5 and lang=="csharp":
        fixed=fixed.replace(
            "// AsSplitQuery con una colección: 1 consulta",
            "// AsSplitQuery con una colección: principal + colección (2 comandos)"
        )
    if n==7 and lang=="csharp":
        fixed=fixed.replace(
            "// Sin función: usa el índice",
            "// Comparación directa: conserva mejor la sargabilidad; verificar el plan"
        )
        fixed=fixed.replace(
            "// Con función: no usa el índice",
            "// Función sobre columna: puede dificultar un index seek; verificar el plan"
        )
    if n==8 and lang=="csharp":
        fixed=fixed.replace(
            "// No usar AsSplitQuery con una sola colección",
            "// Una sola colección: el beneficio típico es menor; medir volumen y roundtrips"
        )
        fixed=fixed.replace(
            "using var transaction = context.Database.BeginTransaction();",
            "using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);"
        )
    if n==9 and lang=="csharp":
        if "EF.CompileAsyncQuery" in fixed:
            fixed = """var consultaCompiladaAsync = EF.CompileAsyncQuery(
    (AceriaDbContext context, string estado) =>
        context.OrdenesFabricacion
            .Where(o => o.Estado == estado));

await foreach (var orden in consultaCompiladaAsync(context, "Pendiente"))
{
    Console.WriteLine(orden.NumeroOrden);
}"""
        elif "EF.CompileQuery" in fixed:
            fixed=fixed.replace(".ToList());", ");")
            fixed=re.sub(
                r"(?m)^(\s*)\.OrderBy\(o => o\.FechaCreacion\)\n(\s*)\);",
                r"\1.OrderBy(o => o.FechaCreacion)\n\1.Select(o => o)\n\2);",
                fixed,
            )
            fixed=fixed.replace(
                "Func<AceriaDbContext, string, List<OrdenFabricacion>>",
                "Func<AceriaDbContext, string, IEnumerable<OrdenFabricacion>>",
            )
            fixed=fixed.replace(
                "public List<OrdenFabricacion> ObtenerPorEstado",
                "public IEnumerable<OrdenFabricacion> ObtenerPorEstado",
            )
        fixed=fixed.replace(
            "// No aporta beneficio: consulta simple ejecutada una vez",
            "// Consulta simple ejecutada una vez: normalmente no es candidata prioritaria; medir"
        )
    return fixed

def source_example_note(n, code, lang):
    if n==3 and lang=="csharp" and ".Include(o => o.Planchas)" in code:
        return (
            "Este ejemplo conserva la sintaxis de la fuente, pero el grafo Orden-Planchas no demuestra por sí solo "
            "identidad repetida porque cada PlanchaAcero pertenece a una sola orden. La demostración ejecutable del "
            "checkpoint 4.3 usa Aleacion, que sí puede reaparecer con la misma clave."
        )
    if n==4 and lang=="csharp" and ".Planchas" in code and "_context.PlanchasAcero" not in code and "context.PlanchasAcero" not in code:
        return (
            "En la baseline de M4, Lazy Loading está desactivado. Acceder por sí solo a una navegación no cargada no "
            "dispara SQL. Este patrón solo produce N+1 si Lazy Loading está habilitado; el checkpoint 4.4 provoca N+1 "
            "de forma explícita mediante una consulta relacionada dentro del bucle y cuenta los comandos reales."
        )
    if n==4 and lang=="csharp" and ".Select(o => new" in code and "Planchas = o.Planchas.Select" in code:
        return (
            "Una proyección correlacionada de una colección no constituye por sí misma un N+1 en EF Core 8. El proveedor "
            "puede traducirla al servidor; hay que inspeccionar el SQL y contar comandos en lugar de inferir N+1 por la sintaxis."
        )
    if n==7 and lang=="csharp" and ("MiMetodoPersonalizado" in code or "EsPendiente(" in code or "Regex.IsMatch" in code or ".IsNormalized()" in code):
        return (
            "En EF Core 8, si este predicado no puede traducirse y está dentro de Where, la consulta lanza "
            "InvalidOperationException. La evaluación en cliente solo aparece tras una frontera explícita como AsEnumerable()."
        )
    if n==7 and lang=="csharp" and ".ToLower()" in code:
        return (
            "Aplicar una función a la columna puede reducir la sargabilidad, pero el uso real de índices depende del "
            "esquema, la collation, los índices y el plan de SQL Server; debe verificarse con el plan de ejecución."
        )
    if n==8 and lang=="csharp" and ".AsSplitQuery()" in code and code.count(".Include(")==1:
        return (
            "Con una sola colección no existe explosión cartesiana entre colecciones. SplitQuery no queda prohibido, "
            "pero su beneficio típico es menor y debe compararse con el coste de roundtrips."
        )
    if n==8 and lang=="csharp" and "BeginTransaction" in code:
        return (
            "Una transacción explícita con aislamiento Serializable se usa aquí solo para ilustrar consistencia entre "
            "los varios comandos; el aislamiento tiene coste y debe elegirse según el escenario."
        )
    if n==9 and lang=="csharp" and ("EF.CompileQuery" in code or "EF.CompileAsyncQuery" in code):
        return (
            "La consulta compilada omite la búsqueda en la caché de forma de consulta de EF. No almacena el plan de "
            "ejecución de SQL Server y debe medirse en el hot path real."
        )
    if n==10 and lang=="sql" and ("OFFSET 0 ROWS" in code or "FETCH NEXT" in code):
        return (
            "Este SQL es ilustrativo. Para una consulta keyset con solo Take, el proveedor SQL Server puede generar TOP "
            "en lugar de OFFSET 0/FETCH. La forma autoritativa para este curso es la salida real de ToQueryString()."
        )
    if n==10 and lang=="csharp" and ".Skip(" in code and ".OrderBy(o => o.FechaCreacion)" in code and ".ThenBy(o => o.Id)" not in code:
        return (
            "La fuente usa aquí una ordenación simplificada por fecha. Para paginación determinista, AceriaData añade "
            "ThenBy(o => o.Id), como se demuestra en el checkpoint ejecutable 4.10."
        )
    return ""

def parse_source_theory(lines,n):
    elements=[]
    i=0
    while i<len(lines):
        raw=lines[i]
        token=raw.strip()
        low=token.lower()
        if low in SOURCE_CODE_MARKERS:
            lang=low
            i+=1
            code=[]
            while i<len(lines):
                cur=lines[i]
                stripped=cur.strip()
                lowcur=stripped.lower()
                if lowcur in SOURCE_CODE_MARKERS and code:
                    break
                if not stripped:
                    code.append(cur)
                    i+=1
                    continue
                # Las explicaciones de la fuente pueden mencionar operadores como ==,
                # nombres de métodos o fragmentos de código. Si la línea empieza con
                # un prefijo de prosa, debe cerrar el bloque igualmente.
                is_prose = stripped.startswith(SOURCE_PROSE_PREFIXES)
                if code and is_prose:
                    break
                code.append(cur)
                i+=1
            while code and not code[-1].strip():
                code.pop()
            if code:
                fixed=source_code_fixes("\n".join(code),n,lang)
                elements.append(("code",lang,fixed))
            continue
        elements.append(("prose","",raw))
        i+=1
    return elements

def render_source_theory(lines,n):
    elements=parse_source_theory(lines,n)
    out=[]
    example_no=0
    for idx,(kind,lang,payload) in enumerate(elements):
        if kind=="code":
            example_no+=1
            out+=["",f"**Ejemplo docente de la fuente {example_no} ({lang.upper()}).**","",fence(payload,lang)]
            note=source_example_note(n,payload,lang)
            if note:
                out+=["","> **Validación EF Core 8 / AceriaData.** "+note]
            out.append("")
            continue
        raw=payload.strip()
        if not raw:
            out.append("")
            continue
        prose=semantic_fixes(raw,n)
        nxt=""
        for future in elements[idx+1:]:
            if future[0]=="prose" and future[2].strip():
                nxt=future[2].strip()
                break
            if future[0]=="code":
                break
        heading=(
            len(raw)<=100
            and not raw.endswith((".",";",",",":"))
            and "\t" not in raw
            and nxt
            and not re.match(r"^(SELECT|FROM|WHERE|ORDER|LEFT|INNER|var |return |public |private )",raw,re.I)
            and not raw.startswith(("La ","El SQL ","Línea ","Líneas "))
        )
        out.append(("#### "+prose) if heading else prose)
    return "\n".join(out),example_no

def semantic_fixes(text,n):
    pairs=[
    ("Las consultas con proyección a tipos anónimos o DTOs no registran entidades en el Change Tracker, incluso si se ejecutan con Tracking. Esto es porque las proyecciones no devuelven entidades completas, sino objetos nuevos.",
     "Una proyección escalar o DTO que no contiene entidades no añade entidades al ChangeTracker; si una proyección personalizada contiene una entidad, esa entidad puede seguir siendo rastreada."),
    ("Los filtros no traducibles provocan que la consulta se ejecute en memoria.",
     "Un predicado no traducible dentro de una parte que debe ejecutarse en servidor provoca una excepción en EF Core 8; la evaluación cliente debe elegirse explícitamente."),
    ("Cuando EF Core encuentra una expresión que no puede traducir, lanza una excepción o ejecuta la parte no traducible en memoria, según el caso.",
     "Cuando EF Core 8 encuentra una expresión no traducible fuera de la proyección superior permitida, lanza una excepción. Para continuar en cliente hay que establecer una frontera explícita."),
    ("Si la expresión está en un Where y EF Core puede evaluarla en el cliente, la consulta se materializa antes de tiempo y el filtro se aplica en memoria.",
     "Si una expresión no traducible está dentro de Where, EF Core 8 falla; solo después de una frontera cliente explícita el filtro pasa a LINQ to Objects."),
    ("AsNoTracking produce instancias duplicadas en consultas con relaciones.",
     "AsNoTracking no realiza resolución de identidad; si una misma clave aparece varias veces pueden materializarse instancias distintas."),
    ("Cada consulta de una Split Query se ejecuta en una transacción separada.",
     "Una Split Query ejecuta varios comandos. Sin una transacción con aislamiento adecuado no existe garantía de que todos observen la misma instantánea frente a cambios concurrentes."),
    ("AsSplitQuery no se debe usar con una sola colección.",
     "Con una sola colección no existe explosión cartesiana entre colecciones, por lo que el beneficio típico de SplitQuery es menor; la decisión sigue dependiendo de volumen, duplicación y roundtrips."),
    ("La consulta compilada reutiliza el SQL generado y el plan de ejecución.",
     "La compiled query evita parte del pipeline de EF; el plan de ejecución pertenece a SQL Server y no queda almacenado en el delegado de EF."),
    ("En una consulta normal, EF Core analiza el árbol de expresión cada vez que se ejecuta la consulta.",
     "En una consulta normal, EF Core compara la forma con su caché interna; una compiled query permite omitir esa búsqueda para una forma preparada.")
    ]
    for old,new in pairs:
        text=text.replace(old,new)
    if n==3:
        repl={
        "las planchas que aparecen en varias órdenes":"las entidades compartidas que aparecen varias veces",
        "una plancha apareciera dos veces":"una misma entidad compartida apareciera dos veces",
        "Como AsNoTracking no usa la caché de identidad, cada plancha se crea como una instancia nueva. Si una misma entidad compartida apareciera dos veces en el resultado, habría dos instancias distintas.":
        "AsNoTracking no hace resolución de identidad. Este grafo con PlanchaAcero no demuestra por sí solo repetición de clave; la evidencia reproducible del checkpoint usa Aleacion compartida entre relaciones.",
        "Las entidades no se registran en el Change Tracker, pero las entidades compartidas que aparecen varias veces se resuelven a la misma instancia.":
        "Las entidades no se registran en el Change Tracker. La resolución de identidad solo produce una diferencia observable cuando una misma clave reaparece en el resultado; AceriaData lo demuestra con Aleacion.",
        "La segunda es ligeramente más lenta porque mantiene la caché de identidad temporal.":
        "La resolución de identidad añade trabajo de materialización; el impacto real debe medirse y no se presupone una diferencia temporal fija."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==2:
        repl={
        "Son más eficientes en consultas de solo lectura porque consumen menos memoria y menos CPU.":
        "Evitan el trabajo del Change Tracker y pueden reducir memoria y CPU en consultas de solo lectura; el efecto temporal concreto debe medirse.",
        "La primera sección mide el tiempo de la consulta con Tracking. La segunda sección mide el tiempo de la consulta sin Tracking. La segunda es más rápida porque no realiza el trabajo del Change Tracker.":
        "La primera sección mide Tracking y la segunda No Tracking. No Tracking elimina trabajo del Change Tracker, pero una medición concreta no debe darse por ganada de antemano.",
        "AsNoTracking es como decirle al supervisor que no anote las planchas: se cargan más rápido, pero no se pueden modificar.":
        "AsNoTracking es como decirle al supervisor que no anote las planchas: evita el coste de seguimiento; si después se quieren persistir cambios, esas instancias no están siendo rastreadas por el contexto."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==3:
        repl={
        "AsNoTracking no usa caché de identidad: cada fila produce una instancia nueva.":
        "AsNoTracking no realiza resolución de identidad: si una misma clave aparece varias veces en el resultado, pueden materializarse instancias distintas.",
        "Se ha comprobado que la resolución de identidad tiene un coste en memoria y CPU, pero garantiza instancias únicas en consultas con relaciones.":
        "Se ha comprobado que la resolución de identidad añade trabajo de materialización y reutiliza una misma instancia cuando una clave se repite dentro de la consulta."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==4:
        repl={
        "La solución al problema N+1 es cargar todas las entidades relacionadas en una sola consulta con Include. Esto reduce el número de consultas de N+1 a 1.":
        "Una solución habitual es la carga anticipada con Include, que evita una consulta por cada entidad principal. Con una colección y el comportamiento por defecto puede resolverse con un único comando; con SplitQuery puede usar varios comandos acotados sin convertirse en N+1.",
        "La primera sección ejecuta N+1 consultas. La segunda sección ejecuta una sola consulta con Include. La segunda es mucho más eficiente.":
        "La primera sección solo ejecutaría N+1 por el acceso a la navegación si Lazy Loading estuviera habilitado. La segunda usa carga anticipada y evita consultas por entidad; el número exacto de comandos depende de Single/Split Query.",
        "La segunda es el acceso a propiedades de navegación en un bucle sin Include.":
        "La segunda es ejecutar explícitamente una consulta relacionada dentro de un bucle; acceder a una navegación no cargada no dispara SQL cuando Lazy Loading está desactivado.",
        "La tercera es el uso de Select que proyecta una colección de navegación sin materializarla correctamente.":
        "Una proyección correlacionada no es por sí misma una causa de N+1 en EF Core 8; debe comprobarse la traducción y el número real de comandos.",
        "La segunda sección muestra el acceso en bucle.":
        "La segunda sección muestra acceso a navegación; solo implicaría consultas adicionales con Lazy Loading habilitado.",
        "La tercera sección muestra la proyección sin ToList.":
        "La tercera sección muestra una proyección correlacionada que debe analizarse por su SQL, no etiquetarse automáticamente como N+1.",
        "El uso de Select que proyecta una colección de navegación sin ToList puede provocar el problema N+1. Si la colección no se materializa, EF Core ejecuta una consulta por cada entidad principal para cargar la colección.":
        "Una proyección de colección puede traducirse a SQL en EF Core 8 y no debe clasificarse automáticamente como N+1. La evidencia válida es el SQL generado y el número de comandos ejecutados.",
        "La primera línea inicia la consulta. La segunda línea proyecta la colección de planchas sin materializarla. La tercera línea materializa la consulta. El bucle itera sobre los resultados. En cada iteración, se accede a item.Planchas.Count(). Si la colección no se ha materializado, se ejecuta una consulta adicional por cada orden.":
        "La consulta proyecta una colección correlacionada y después materializa el resultado. En EF Core 8 debe observarse la traducción concreta; Count sobre la colección ya proyectada no implica por sí mismo una nueva consulta por orden.",
        "El uso de Include evita el problema N+1 porque carga todas las entidades relacionadas en una sola consulta. Sin embargo, si se accede a una propiedad de navegación de segundo nivel sin ThenInclude, se puede producir el problema N+1 en el segundo nivel.":
        "Include evita la carga relacionada mediante una consulta por cada principal. El acceso posterior a otra navegación solo generará SQL adicional si existe un mecanismo de carga como Lazy Loading o una consulta explícita.",
        "La primera línea inicia la consulta. La segunda línea incluye la colección de planchas. La tercera línea materializa la consulta. El bucle itera sobre las órdenes y sus planchas. En cada iteración, se accede a plancha.Orden. Si la propiedad Orden no se ha cargado con Include, se ejecuta una consulta adicional por cada plancha.":
        "La orden principal ya forma parte del grafo materializado y EF Core puede realizar relationship fixup de la referencia inversa. Este ejemplo no demuestra N+1 en la baseline de M4; para demostrarlo se debe consultar explícitamente una relación dentro del bucle o habilitar Lazy Loading.",
        "El acceso a propiedades de navegación en un bucle sin Include también lo provoca.":
        "El acceso a una navegación dentro de un bucle provoca N+1 cuando existe Lazy Loading; sin él, se necesita una consulta explícita por iteración para producir N+1.",
        "La proyección sin ToList lo provoca.":
        "Una proyección correlacionada no se clasifica como N+1 sin observar primero su traducción y sus comandos.",
        "Include evita el problema N+1 en el primer nivel.":
        "Include evita consultas relacionadas por cada principal al cargar la navegación anticipadamente.",
        "ThenInclude evita el problema N+1 en el segundo nivel.":
        "ThenInclude permite cargar anticipadamente navegaciones de niveles posteriores; su necesidad depende del grafo y de cómo se acceda después."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==5:
        repl={
        "La solución más directa al problema N+1 es usar Include para cargar las entidades relacionadas en una sola consulta. Include genera un LEFT JOIN que trae las entidades principales y las relacionadas en un solo resultado.":
        "Una solución directa es usar Include para carga anticipada. En modo Single Query, una colección suele resolverse mediante JOIN en un único comando; en modo Split Query, EF separa la colección en un comando adicional.",
        "Include con una sola colección ejecuta una sola consulta con un LEFT JOIN. AsSplitQuery con una sola colección también ejecuta una sola consulta. La diferencia aparece cuando se incluyen varias colecciones: Include genera un producto cartesiano, mientras que AsSplitQuery divide la consulta en varias.":
        "Include con una sola colección en modo Single Query usa normalmente un comando con JOIN. AsSplitQuery con una colección genera el comando de principales y otro para la colección. Con varias colecciones hermanas, Single Query puede sufrir explosión cartesiana y SplitQuery añade un comando por colección.",
        "La primera consulta usa Include con una colección. La segunda usa AsSplitQuery con una colección. La tercera usa Include con dos colecciones y genera un producto cartesiano. La cuarta usa AsSplitQuery con dos colecciones y ejecuta tres consultas sin producto cartesiano.":
        "La primera consulta usa Single Query con una colección. La segunda usa Split Query y requiere dos comandos. La tercera incluye dos colecciones hermanas y puede generar explosión cartesiana. La cuarta usa SplitQuery y ejecuta tres comandos: principal más uno por colección.",
        "Las proyecciones permiten seleccionar solo las columnas necesarias y evitar el problema N+1. Al proyectar a un DTO o a un tipo anónimo, EF Core genera una sola consulta con las columnas proyectadas.":
        "Las proyecciones permiten seleccionar solo las columnas necesarias y pueden evitar consultas por entidad cuando toda la forma se traduce al servidor. Debe verificarse la traducción y el número de comandos reales.",
        "La segunda es más eficiente porque transfiere menos datos.":
        "La segunda transfiere menos columnas; el impacto total debe medirse junto con cardinalidad, materialización y plan del servidor."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==7:
        repl={
        "Detectar consultas que se ejecutan en memoria por falta de traducción.":
        "Detectar consultas no traducibles y distinguirlas de la evaluación en memoria elegida explícitamente.",
        "En este punto se profundiza en las consultas que no se traducen completamente a SQL y que provocan que parte del trabajo se realice en memoria.":
        "En este punto se estudian las consultas que no se traducen completamente a SQL: en EF Core 8 un predicado no traducible dentro de Where falla, salvo que el desarrollador establezca explícitamente una frontera hacia evaluación cliente.",
        "La consulta se materializa antes de tiempo y el filtro se aplica en memoria. Todas las órdenes se cargan y después se filtran.":
        "En EF Core 8 este Where no traducible provoca InvalidOperationException. Para filtrar en memoria debe establecerse una frontera explícita, por ejemplo con AsEnumerable().",
        "La segunda consulta se materializa antes de tiempo y el filtro se aplica en memoria.":
        "La segunda consulta provoca una excepción de traducción en EF Core 8 mientras el método personalizado permanezca dentro de Where.",
        "La segunda consulta aplica ToLower sobre la columna. SQL Server no puede usar el índice porque la función se aplica a cada fila.":
        "La segunda consulta aplica LOWER sobre la columna; esto puede reducir la sargabilidad. El uso efectivo del índice debe verificarse en el plan de SQL Server.",
        "La segunda es más lenta porque no usa el índice.":
        "La segunda puede tener un plan menos eficiente; el resultado temporal debe medirse y no se presupone.",
        "El índice sobre Cliente puede usarse si la collation lo permite.":
        "La posibilidad de usar un índice depende de la collation, el patrón, el esquema y el plan de ejecución."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==8:
        repl={
        "Para garantizar la coherencia, se debe usar una transacción explícita.":
        "Si se necesita una instantánea consistente, debe elegirse una transacción y un nivel de aislamiento que proporcionen esa garantía.",
        "AsSplitQuery no se debe usar cuando se incluye una sola colección, porque el producto cartesiano no se produce.":
        "Con una sola colección no existe explosión cartesiana entre colecciones; el beneficio típico de SplitQuery suele ser menor, pero la decisión depende de volumen y roundtrips.",
        "Tampoco se debe usar cuando se necesita coherencia transaccional entre las consultas, porque cada consulta se ejecuta en una transacción separada.":
        "Si se necesita coherencia entre los comandos, debe elegirse explícitamente una estrategia transaccional y un nivel de aislamiento adecuados.",
        "La primera consulta usa AsSplitQuery con una sola colección. No aporta beneficio porque no hay producto cartesiano.":
        "La primera consulta usa AsSplitQuery con una sola colección. No hay explosión cartesiana entre colecciones; el posible beneficio o coste debe medirse."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==8:
        extra={
        "Observaciones: SplitQuery es ligeramente más rápida porque no genera el producto cartesiano. El SQL de SingleQuery incluye tres LEFT JOIN con todas las columnas. El SQL de SplitQuery incluye tres consultas separadas. El número de filas transferidas es menor en SplitQuery.":
        "Observaciones: SplitQuery evita la explosión cartesiana entre colecciones, pero añade roundtrips. No se presupone que sea más rápida: se comparan comandos, filas, tamaño de datos y tiempo en el entorno real.",
        "AsSplitQuery es útil cuando se incluyen varias colecciones con muchas filas.":
        "AsSplitQuery puede ser útil cuando varias colecciones producen duplicación o explosión cartesiana; su coste en roundtrips también debe medirse."
        }
        for old,new in extra.items(): text=text.replace(old,new)
    if n==9:
        repl={
        "EF Core traduce las consultas LINQ a SQL mediante un proceso de compilación que incluye el análisis del árbol de expresión, la generación del SQL y la creación del plan de ejecución. Este proceso tiene un coste que se paga cada vez que se ejecuta una consulta. Una Compiled Query paga ese coste una sola vez y lo reutiliza en las ejecuciones posteriores.":
        "EF Core procesa la forma de la consulta y almacena en caché la salida de compilación. Una compiled query crea un delegado explícito que evita la búsqueda por forma en la caché interna; no crea ni almacena el plan de ejecución de SQL Server.",
        "Cada vez que se ejecuta una consulta LINQ, EF Core realiza varios pasos: analiza el árbol de expresión, aplica las convenciones, genera el SQL, crea el plan de ejecución y lo almacena en la caché de consultas. Este proceso tiene un coste en CPU y memoria.":
        "EF Core mantiene una caché por forma de consulta. En una consulta normal todavía debe comparar el árbol de expresión con las formas cacheadas; una compiled query permite omitir ese trabajo de búsqueda.",
        "El bucle ejecuta la misma consulta mil veces. Cada ejecución compila la consulta, genera el SQL y lo ejecuta. El coste de compilación se paga mil veces. Aunque EF Core tiene una caché de consultas, la primera compilación de cada consulta es la más costosa.":
        "El bucle ejecuta la misma forma muchas veces. EF Core reutiliza su caché interna; la medición sirve para observar el coste total, no para afirmar que la consulta se recompila por completo en cada iteración.",
        "EF Core mantiene una caché de consultas que almacena las consultas compiladas. Cuando se ejecuta una consulta que ya está en la caché, EF Core reutiliza el SQL generado y el plan de ejecución.":
        "EF Core almacena en caché la salida de compilación asociada a la forma de la consulta. El plan de ejecución pertenece a SQL Server y se gestiona independientemente.",
        "La segunda es más rápida que la primera porque no compila la consulta.":
        "La segunda puede beneficiarse de la caché interna de EF y de las cachés del servidor, pero no se presupone una ventaja temporal fija sin medir."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==9:
        extra={
        "Una Compiled Query es una consulta LINQ que se compila una sola vez y se reutiliza muchas veces.":
        "Una Compiled Query es un delegado LINQ compilado explícitamente que puede invocarse muchas veces, evitando la búsqueda por forma en la caché interna de consultas de EF.",
        "Una Compiled Query se compila una sola vez y se almacena en una variable estática o en un campo de la clase. Se puede invocar muchas veces con parámetros distintos. La consulta compilada mantiene el SQL generado y el plan de ejecución durante toda la vida de la aplicación.":
        "Una Compiled Query suele conservarse en un campo estático o equivalente y se invoca con parámetros distintos. El delegado pertenece a EF; el SQL concreto y el plan de ejecución son responsabilidades separadas de la ejecución y de SQL Server.",
        "Contexto del proyecto: En el punto 4.8 se estudiaron las Split Queries, incluyendo el producto cartesiano y la configuración del comportamiento por defecto. En este punto se profundiza en las Compiled Queries, que permiten reutilizar el plan de ejecución de las consultas más frecuentes. Esta técnica se usará en el punto 4.10 para la paginación eficiente.":
        "Contexto del proyecto: En el punto 4.8 se estudiaron las Split Queries. En este punto se profundiza en Compiled Queries, que permiten omitir la búsqueda por forma en la caché interna de EF en hot paths medidos. No almacenan el plan de ejecución de SQL Server.",
        "Observaciones: la consulta compilada es más rápida porque el plan de ejecución se reutiliza. La consulta no compilada paga el coste de compilación en cada ejecución, aunque EF Core tiene una caché de consultas que reduce el coste. La diferencia es mayor cuando la consulta es compleja o cuando la caché de consultas se llena.":
        "Observaciones: la consulta compilada evita la búsqueda por forma de EF. La consulta normal ya aprovecha la caché interna; cualquier diferencia temporal debe medirse y no se atribuye a que el delegado almacene el plan de SQL Server.",
        "Se ha comprobado que las consultas compiladas reutilizan el plan de ejecución y que aportan beneficios cuando la misma consulta se ejecuta muchas veces.":
        "Se ha comprobado qué trabajo interno de EF puede evitar una compiled query y que su conveniencia debe demostrarse con benchmark en un hot path real."
        }
        for old,new in extra.items(): text=text.replace(old,new)
    if n==10:
        repl={
        "SQL Server usa el índice sobre FechaCreacion e Id para localizar las filas.":
        "Para que el seek compuesto sea eficiente conviene un índice cuyo orden empiece por FechaCreacion e Id. La baseline de M4 no añade una migración ni un índice nuevo, por lo que el plan real debe verificarse.",
        "La primera consulta usa offset pagination. La segunda usa keyset pagination. La segunda es más eficiente en tablas grandes porque no lee las filas anteriores.":
        "La primera usa offset y la segunda keyset. Con un índice adecuado y navegación secuencial, keyset evita el coste creciente de saltar filas; el plan real sigue dependiendo de índices y selectividad.",
        "La keyset pagination es más eficiente en tablas grandes.":
        "La keyset pagination suele escalar mejor para navegación siguiente/anterior cuando existe una ordenación única e índices adecuados.",
        "Observaciones: keyset pagination es más rápida porque no lee las filas anteriores. El SQL de offset pagination incluye OFFSET 3. El SQL de keyset pagination incluye el filtro por la clave compuesta y OFFSET 0. La diferencia es pequeña con pocas filas, pero crece con el número de filas.":
        "Observaciones: keyset evita un desplazamiento creciente, pero la ventaja temporal depende del índice y del plan. El SQL real debe inspeccionarse con ToQueryString; una consulta keyset con Take puede usar TOP en SQL Server."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    if n==11:
        text=text.replace(
            "El plan de ejecución muestra cómo SQL Server ejecuta una consulta. Se puede obtener con SQL Server Management Studio o con SET STATISTICS IO ON. El plan de ejecución permite identificar table scans, index seeks y otras operaciones costosas.",
            "El plan de ejecución muestra cómo SQL Server ejecuta una consulta y puede inspeccionarse desde SSMS u otras herramientas de plan. SET STATISTICS IO ON aporta métricas de E/S, pero no sustituye al plan de ejecución."
        )
    if n==12:
        repl={
        "Los problemas de rendimiento más comunes en EF Core son: tracking innecesario, over-fetching, N+1, producto cartesiano, consultas en memoria, funciones en Where, falta de paginación y compilación repetida. Cada problema tiene una técnica de solución asociada.":
        "Entre los problemas habituales están tracking innecesario, over-fetching, N+1, explosión cartesiana, fronteras cliente mal elegidas, expresiones poco sargables y paginación inadecuada. Las técnicas se eligen según evidencia; no existe una receta que deba aplicarse completa a cada consulta.",
        "La primera línea inicia la consulta. La segunda línea aplica AsNoTracking. La tercera línea aplica AsSplitQuery. La cuarta línea proyecta. La quinta línea materializa. AsSplitQuery no aporta beneficio porque solo hay una colección. Es una optimización prematura.":
        "La consulta combina técnicas sin demostrar que todas aporten valor. Si la proyección escalar elimina las navegaciones, SplitQuery deja de tener un grafo de colecciones que dividir; el checklist debe justificar técnicas aplicadas y descartadas.",
        "Reto: Tomar una consulta sin optimizar que cargue las órdenes con sus planchas y detalles, y aplicar el checklist completo: AsNoTracking, proyección, Include, AsSplitQuery, filtro, paginación y documentación. Medir el tiempo antes y después.":
        "Reto: auditar una consulta que carga órdenes y relaciones. Evaluar cada técnica del checklist y aplicar solo las justificadas por la forma de la consulta y por la medición; documentar también las técnicas descartadas.",
        "Resultado esperado: la consulta optimizada es más rápida y transfiere menos datos. La documentación XML explica las decisiones aplicadas.":
        "Resultado esperado: la consulta final presenta evidencia medible de sus decisiones —SQL, comandos, filas, tracking y shape— sin exigir de antemano que todas las técnicas ni todos los tiempos mejoren."
        }
        for old,new in repl.items(): text=text.replace(old,new)
    return text

def theory_parts(block,n):
    lines=block.splitlines()
    audience=next((x for x in lines if x.startswith("Audiencia:")),"")
    project=next((x for x in lines if x.startswith("Proyecto:")),"")
    oi=lines.index("Objetivos de aprendizaje")
    ti=lines.index("Teoría")
    pi=lines.index("Práctica") if "Práctica" in lines else len(lines)
    objectives=[semantic_fixes(x.strip(),n) for x in lines[oi+1:ti] if x.strip()]
    body,source_example_count=render_source_theory(lines[ti+1:pi],n)
    return audience,project,objectives,body,source_example_count

def fence(code,lang="csharp"):
    return BT*3+lang+"\n"+code.rstrip()+"\n"+BT*3

def explain(line):
    s=line.strip()
    if not s: return "Separa bloques lógicos."
    if s.startswith("using "): return "Importa tipos o extensiones requeridos por esta implementación."
    if s.startswith("namespace "): return "Sitúa el archivo en la capa y espacio de nombres correspondiente."
    if "public sealed class" in s or "public sealed partial class" in s: return "Declara la clase concreta usada por el checkpoint."
    if s.startswith("private readonly "): return "Declara la dependencia conservada por la instancia."
    if "UseCase(" in s and "public " in s: return "Constructor del caso de uso e inyección de la unidad de trabajo."
    if "=> _unidad = unidad" in s: return "Asigna la dependencia inyectada sin acoplar Application a EF Core."
    if s.startswith("public void Ejecutar"): return "Define el flujo principal validado por el E2E."
    if "Console.WriteLine" in s: return "Publica evidencia observable en la consola."
    if "Database.EnsureDeleted" in s: return "Reinicia la base de demostración para un E2E determinista."
    if "Database.Migrate" in s: return "Aplica la historia real de migraciones heredada."
    if "DemoData.Seed" in s: return "Carga el dataset determinista de AceriaData."
    if "ChangeTracker.Clear" in s: return "Limpia tracking antes de la demostración."
    if "AddScoped<" in s: return "Registra el caso de uso con ciclo de vida Scoped."
    if "GetRequiredService<" in s: return "Resuelve una dependencia obligatoria desde el ámbito."
    if ".AsNoTrackingWithIdentityResolution()" in s: return "Activa NoTracking con resolución temporal de identidad."
    if ".AsNoTracking()" in s: return "Desactiva tracking para esta consulta de lectura."
    if ".AsTracking()" in s: return "Fuerza tracking para hacer observable el ChangeTracker."
    if ".AsSplitQuery()" in s: return "Divide la carga relacionada en varios comandos SQL."
    if ".AsSingleQuery()" in s: return "Fuerza un único comando para la comparación."
    if ".AsEnumerable()" in s: return "Establece explícitamente la frontera hacia LINQ to Objects."
    if ".Where(" in s: return "Añade el predicado de filtrado a la forma de consulta."
    if ".OrderBy(" in s or ".ThenBy(" in s: return "Forma parte del orden determinista."
    if ".Skip(" in s: return "Aplica el desplazamiento de la paginación offset."
    if ".Take(" in s: return "Limita el número máximo de elementos."
    if ".Select(" in s: return "Proyecta la forma de resultado y controla datos materializados."
    if ".Include(" in s or ".ThenInclude(" in s: return "Define la navegación relacionada que debe cargarse."
    if "ToQueryString" in s: return "Obtiene la representación SQL sin materializar la consulta."
    if "ToList" in s: return "Materializa la consulta y ejecuta SQL si sigue siendo IQueryable."
    if "Stopwatch" in s: return "Participa en la medición temporal observacional."
    if "SqlCommandCounterInterceptor" in s: return "Mide comandos SQL reales ejecutados."
    if "EF.CompileQuery" in s: return "Prepara un delegado de compiled query de EF."
    if "TagWith" in s: return "Etiqueta el SQL para correlacionarlo con logs."
    if s.startswith("if ") or s.startswith("if("): return "Comprueba una condición contractual del E2E."
    if s.startswith("throw "): return "Hace fallar el checkpoint si la evidencia no coincide."
    if s.startswith("var "): return "Calcula y conserva el resultado que será validado o mostrado."
    if s in ("{","}"): return "Delimita el bloque sintáctico asociado."
    if s.startswith("return "): return "Devuelve el resultado calculado al llamador."
    if ".SetBasePath(" in s: return "Fija el directorio base desde el que Configuration localizará los archivos de configuración."
    if ".AddJsonFile(" in s: return "Añade appsettings.json como origen obligatorio de configuración."
    if ".AddEnvironmentVariables(" in s: return "Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos."
    if s==".Build();": return "Construye el objeto de configuración a partir de los proveedores añadidos."
    if s.startswith("?? throw new InvalidOperationException"): return "Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada."
    if "services.AddAceriaInfrastructure" in s: return "Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada."
    if "ValidateOnBuild" in s: return "Ordena validar el grafo de dependencias al construir el proveedor de servicios."
    if "ValidateScopes" in s: return "Activa la comprobación de ciclos de vida Scoped para detectar resoluciones incorrectas."
    if s.startswith("useCase.Ejecutar"): return "Ejecuta el caso de uso del checkpoint después de preparar base de datos y datos de demostración."
    if s.startswith("for (") or s.startswith("for("): return "Repite la operación para obtener una medición observacional sobre varias ejecuciones."
    if s.startswith("_ = "): return "Fuerza la ejecución y descarta el valor porque en este bloque interesa medir el coste de la operación."
    if s.endswith(".Stop();"): return "Detiene el cronómetro inmediatamente después del bloque que se está midiendo."
    if "cursor.FechaCreacion" in s: return "Pasa la fecha del cursor anterior como primera componente del seek compuesto."
    if "cursor.Id" in s: return "Pasa el Id del cursor anterior como desempate determinista del seek."
    if s.endswith(".Any())") or s==".Any())": return "Comprueba si existe alguna coincidencia sin materializar toda la secuencia."
    if s.startswith('$"') or (s.startswith('"') and s.endswith((",",");"))): return "Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo."
    if s.endswith("||") or s.endswith("&&"): return "Continúa una condición compuesta usada para validar la equivalencia del resultado."
    if re.match(r"^[A-Za-z_][A-Za-z0-9_]*\s*=\s*.+[,;]?$", s): return "Asigna el valor calculado a la propiedad o variable correspondiente del resultado."
    if re.match(r"^(public|private|internal|protected)\\s+.*\\([^;]*\\)\\s*(=>)?$", s):
        return "Declara un método concreto del checkpoint, con su tipo de retorno, nombre y parámetros."
    if re.match(r"^(public|private|internal|protected)\\s+(static\\s+)?readonly\\s+", s):
        return "Declara un campo de solo lectura que conserva una dependencia o delegado reutilizable."
    if s.startswith("try"):
        return "Abre el bloque protegido cuya excepción forma parte de la evidencia del escenario."
    if s.startswith("catch "):
        return "Captura explícitamente la excepción esperada para distinguir el fallo de traducción."
    if s.startswith("foreach ") or s.startswith("foreach("):
        return "Recorre los elementos materializados para observar o validar cada resultado."
    if s.startswith("await foreach "):
        return "Enumera de forma asíncrona el resultado de la consulta compilada."
    if s.startswith("."):
        m=re.match(r"^\\.([A-Za-z0-9_]+)", s)
        member=m.group(1) if m else "operación"
        return "Continúa la composición fluida invocando "+member+" sobre el resultado de la línea anterior."
    if re.match(r"^_[A-Za-z0-9_]+\\.[A-Za-z0-9_]+\\(", s):
        m=re.match(r"^_([A-Za-z0-9_]+)\\.([A-Za-z0-9_]+)", s)
        return "Invoca "+m.group(2)+" sobre la dependencia _"+m.group(1)+" para ejecutar la operación concreta."
    if "=>" in s:
        return "Define la expresión lambda que EF Core o el caso de uso empleará en esta operación."
    if s in (");", "));", "});", "];", ")", "};"):
        return "Cierra la llamada, expresión o inicializador abierto en las líneas anteriores."
    if s.endswith(","):
        return "Aporta un argumento o componente intermedio a la construcción multilínea en curso."
    if s.startswith("new "):
        return "Crea la instancia concreta que se devolverá o utilizará como resultado."
    return "Conserva esta expresión concreta dentro del flujo del checkpoint y su efecto queda cubierto por el E2E: "+s

def line_notes(code,label):
    out=["#### Explicación línea a línea — "+label,""]
    for i,line in enumerate(code.splitlines(),1):
        if not line.strip():
            continue
        shown=line.replace(BT,"´")
        out.append("Línea "+str(i)+": "+BT+shown+BT+" → "+explain(line))
        out.append("")
    return "\n".join(out)

def make_theory(points):
    out=["# Módulo 4 — Optimización y rendimiento","",
    "**12 puntos · 6 horas · AceriaData · .NET 8 · Entity Framework Core 8 · SQL Server LocalDB**","",
    "Este módulo continúa el estado validado M03/PROYECTO/3.12. La fuente original se conserva en M04/SOURCE; las afirmaciones técnicas se contrastan con EF Core 8 y con E2E reales.","",
    "## Mapa del módulo","",
    "| Punto | Tema | Duración de referencia |","|---|---|---:|"]
    for n,p in POINTS.items():
        out.append("| 4."+str(n)+" | "+p[0]+" | 30 min |")
    out+=["","> Criterio del módulo: ninguna técnica se considera optimización por su nombre; debe relacionarse con SQL, roundtrips, filas/columnas, materialización, tracking y medición.",""]
    for n in range(1,13):
        title,use,repo,extra,correction,challenge,analogy=POINTS[n]
        audience,project,objectives,body,source_example_count=theory_parts(points[n],n)
        out+=["## Punto 4."+str(n)+" — "+title,"","**"+audience+"**","","**"+project+"**","",
        "### Objetivos de aprendizaje",""]
        out += ["- "+x.rstrip(".")+"." for x in objectives]
        out+=["","### Precisión técnica validada para EF Core 8","",correction,""]
        if n in OFFICIAL: out+=["Referencia técnica de contraste: "+OFFICIAL[n],""]
        d=M4/"PROYECTO"/("4."+str(n))
        infra=d/"src"/"AceriaData.Infrastructure"
        repocode=(infra/"Repositories"/repo).read_text(encoding="utf-8").strip()
        out+=["### Desarrollo teórico","",body,"",f"**Cobertura de ejemplos de la fuente: {source_example_count} bloques teóricos conservados/adaptados.**","",
        "### Anclaje en AceriaData","",
        "El concepto está materializado en M04/PROYECTO/4."+str(n)+" y el checkpoint ha pasado restore, build, migraciones y E2E sobre SQL Server LocalDB.","",
        "### Ejemplo ejecutable del concepto","",
        "El siguiente archivo no es pseudocódigo ni una adaptación editorial: es la implementación de Infrastructure del checkpoint validado 4."+str(n)+".","",
        fence(repocode),"",
        line_notes(repocode,repo),"",
        "### Qué debe observarse en ejecución","",
        "La lectura del código debe completarse con la evidencia de ejecución: SQL traducido cuando corresponda, número de comandos, cardinalidad, columnas materializadas, estado del ChangeTracker y cualquier aserción específica del checkpoint. El hecho de que el código compile no demuestra por sí solo una mejora de rendimiento; por eso cada punto termina en una comprobación observable.","",
        "**Reto conceptual.** "+challenge,"","**Analogía operativa.** "+analogy,"",
        "### Criterios de salida","",
        "- Relacionar LINQ con SQL o comandos ejecutados.",
        "- Distinguir coste de servidor, transferencia, materialización y tracking.",
        "- Justificar técnicas aplicadas y descartadas.",
        "- Ejecutar el checkpoint y obtener 4."+str(n)+" OK.","","---",""]
    return "\n".join(out)

def interface_methods(text):
    return set(re.findall(
        r"(?m)^\s*[^\n;{}]*?\b([A-ZÁÉÍÓÚÑ]\w*)\s*\([^;{}]*\)\s*;",
        text,
    ))

def contract_delta(n):
    current=(M4/"PROYECTO"/f"4.{n}"/"src"/"AceriaData.Application"/"Interfaces.cs").read_text(encoding="utf-8")
    if n==1:
        previous=(ROOT/"M03"/"PROYECTO"/"3.12"/"src"/"AceriaData.Application"/"Interfaces.cs").read_text(encoding="utf-8")
    else:
        previous=(M4/"PROYECTO"/f"4.{n-1}"/"src"/"AceriaData.Application"/"Interfaces.cs").read_text(encoding="utf-8")
    prev=interface_methods(previous)
    cur=interface_methods(current)
    return sorted(cur-prev), sorted(prev-cur)

def versionable_file_map(root):
    return {
        p.relative_to(root).as_posix(): p.read_bytes()
        for p in root.rglob("*")
        if p.is_file() and "bin" not in p.parts and "obj" not in p.parts
    }

def physical_delta(n):
    current_root=M4/"PROYECTO"/f"4.{n}"
    previous_root=(
        ROOT/"M03"/"PROYECTO"/"3.12"
        if n==1
        else M4/"PROYECTO"/f"4.{n-1}"
    )
    previous=versionable_file_map(previous_root)
    current=versionable_file_map(current_root)
    added=sorted(set(current)-set(previous))
    removed=sorted(set(previous)-set(current))
    changed=sorted(
        p for p in set(current)&set(previous)
        if current[p] != previous[p]
    )
    return added,changed,removed

def make_practice():
    out=["# Curso Profesional de Entity Framework Core 8","","# Módulo 4 — Prácticas: Optimización y rendimiento","",
    "**Autor: JAIME GALLO**","",
    "Cada práctica trabaja sobre un checkpoint completo. La secuencia es acumulativa desde M03/PROYECTO/3.12.",""]
    previous="M03/PROYECTO/3.12"
    for n in range(1,13):
        title,use,repo,extra,correction,challenge,analogy=POINTS[n]
        d=M4/"PROYECTO"/("4."+str(n))
        app=d/"src"/"AceriaData.Application"
        infra=d/"src"/"AceriaData.Infrastructure"
        con=d/"src"/"AceriaData.Console"
        usecode=(app/use).read_text(encoding="utf-8").strip()
        repocode=(infra/"Repositories"/repo).read_text(encoding="utf-8").strip()
        program=(con/"Program.cs").read_text(encoding="utf-8").strip()
        added_methods,removed_methods=contract_delta(n)
        added_files,changed_files,removed_files=physical_delta(n)
        contract_lines=[
            "**Métodos añadidos al contrato:** "+(", ".join(BT+x+BT for x in added_methods) if added_methods else "ninguno")+".",
            "**Métodos retirados del contrato:** "+(", ".join(BT+x+BT for x in removed_methods) if removed_methods else "ninguno")+".",
            "",
            "#### Inventario físico exacto del delta",
            "",
            "**Archivos añadidos:**",
        ]
        contract_lines += ["- "+BT+x+BT for x in added_files] if added_files else ["- Ninguno."]
        contract_lines += ["","**Archivos modificados:**"]
        contract_lines += ["- "+BT+x+BT for x in changed_files] if changed_files else ["- Ninguno."]
        contract_lines += ["","**Archivos eliminados:**"]
        contract_lines += ["- "+BT+x+BT for x in removed_files] if removed_files else ["- Ninguno."]
        out+=["## Punto 4."+str(n)+" — "+title,"","### Contexto del proyecto","",
        "Este checkpoint continúa "+previous+". Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.","",
        "### Objetivo práctico","",correction,"",
        "### Paso 1: Abrir el checkpoint","",fence("cd M04/PROYECTO/4."+str(n)+"\ndotnet restore AceriaData.sln","powershell"),"",
        "### Paso 2: Comprobar migraciones","",fence("dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console","powershell"),"",
        "Debe seguir apareciendo M2_2_12_Architecture.","",
        "### Paso 3: Identificar el delta docente","",
        "El archivo principal del delta es src/AceriaData.Infrastructure/Repositories/"+repo+". El checkpoint conserva todo el estado anterior.",""] + contract_lines + ["",
        "El contrato anterior se conserva íntegro salvo los cambios declarados arriba; los métodos nuevos se ejercen desde el caso de uso del punto.","",
        "### Paso 4: Implementar y estudiar Infrastructure","",fence(repocode),"",line_notes(repocode,repo),""]
        for rel in extra:
            code=(d/rel).read_text(encoding="utf-8").strip()
            lang="csharp" if rel.endswith(".cs") else ("json" if rel.endswith(".json") else "text")
            out+=["Archivo complementario: "+rel,"",fence(code,lang),""]
            if rel.endswith(".cs"):
                out += [line_notes(code,Path(rel).name),""]
        out+=["Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.","",
        "### Paso 5: Implementar el caso de uso","",fence(usecode),"",line_notes(usecode,use),"",
        "### Paso 6: Preparar el composition root","",fence(program),"",line_notes(program,"Program.cs"),"",
        "### Paso 7: Compilar","",fence("dotnet build AceriaData.sln --configuration Release","powershell"),"",
        "El build debe finalizar sin errores; el delta se propaga a los estados posteriores.","",
        "### Paso 8: Ejecutar en LocalDB","",fence("dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release","powershell"),"",
        "La salida debe terminar con 4."+str(n)+" OK. Las aserciones internas fallan si la evidencia no coincide.","",
        "### Paso 9: Diagnóstico técnico","",
        "**Reto resuelto.** "+challenge,"",
        "No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.","",
        "### Paso 10: Cierre acumulativo","",
        "1. Confirma el marcador E2E.",
        "2. Comprueba que EnsureCreated no aparece.",
        "3. Conserva las migraciones heredadas.",
        "4. Verifica que Application no depende de EF Core.",
        "5. Compara con el checkpoint anterior y documenta el delta.","",
        "### Errores comunes revisados","",
        "- Confundir una medición aislada con una conclusión de rendimiento.",
        "- Materializar antes de terminar filtros o proyecciones sin intención.",
        "- Aplicar una técnica por regla general en lugar de observar la consulta.",
        "- Relajar una aserción para ocultar un fallo en vez de corregir su causa.","",
        "### Trazabilidad con la práctica fuente","",
        "La práctica fuente de 4."+str(n)+" incluía además los siguientes focos docentes:",
        ""]
        out += ["- "+item for item in SOURCE_COVERAGE[n]["focus"]]
        out += ["",
        "**Tratamiento en el M4 definitivo.** "+SOURCE_COVERAGE[n]["adaptation"],"",
        "**Reto de ampliación procedente de la fuente.** "+SOURCE_COVERAGE[n]["challenge"],"",
        "#### Errores de la fuente que deben seguir siendo diagnosticables",""]
        out += ["- "+item for item in SOURCE_COVERAGE[n]["errors"]]
        if n == 12:
            out += ["","### Estado acumulativo real de AceriaData al cerrar M4",""]
            out += ["- "+item for item in FINAL_PROJECT_STATE]
        out += ["",
        "### Analogía operativa","",analogy,"",
        "### Resultado esperado","",
        "El checkpoint 4."+str(n)+" queda ejecutable, trazado y reproducible, y sirve como baseline física del punto siguiente.","",
        "### Conexión con el siguiente punto","",
        ("El siguiente estado es 4."+str(n+1)+" y parte físicamente de este checkpoint." if n<12 else "Este punto cierra el código acumulativo de M4 y consolida el checklist."),"","---",""]
        previous="M04/PROYECTO/4."+str(n)
    return "\n".join(out)

def make_source_traceability():
    source_points=split_points(load_source())
    out=["# Trazabilidad de la fuente docente - Módulo 4","",
    "Esta matriz demuestra que la fuente 4.1-4.12 se conserva como especificación docente, distinguiendo lo que se mantiene, lo que se adapta al AceriaData real y lo que se corrige por comportamiento de EF Core 8.","",
    "| Punto | Cobertura | Tratamiento principal |","|---|---|---|"]
    for n in range(1,13):
        status="CONSERVADO / ADAPTADO"
        if n in (3,7,8,9,12):
            status="CONSERVADO / ADAPTADO / CORREGIDO"
        out.append("| 4."+str(n)+" | "+status+" | "+SOURCE_COVERAGE[n]["adaptation"].replace("|","/")+" |")
    out += ["","## Criterios de conservación","",
    "- Todos los objetivos de aprendizaje de la fuente aparecen en la teoría definitiva.",
    "- Los subtemas teóricos se mantienen salvo correcciones técnicas explícitas.",
    "- Los bloques de código, SQL y texto técnico de la teoría fuente se conservan o se adaptan explícitamente; no se sustituyen por un único ejemplo final.",
    "- Los retos y errores comunes relevantes se reintroducen en la práctica definitiva como trazabilidad y ampliación.",
    "- El código de la práctica no copia ejemplos esquemáticos que contradicen el modelo real; usa los checkpoints validados 4.1-4.12.",
    "- Las correcciones de EF Core 8 no eliminan el objetivo docente original: lo reformulan con comportamiento reproducible.",
    ""]
    for n in range(1,13):
        out += ["## 4."+str(n)+" - "+POINTS[n][0],"",
        "**Focos conservados:**",""]
        out += ["- "+x for x in SOURCE_COVERAGE[n]["focus"]]
        _,_,_,_,example_count=theory_parts(source_points[n],n)
        out += ["","**Ejemplos teóricos de la fuente conservados/adaptados:** "+str(example_count)+".","",
        "**Adaptación/corrección:** "+SOURCE_COVERAGE[n]["adaptation"],"",
        "**Reto conservado/adaptado:** "+SOURCE_COVERAGE[n]["challenge"],""]
    return "\n".join(out)

def main():
    points=split_points(load_source())
    if set(points)!=set(range(1,13)):
        raise RuntimeError("Fuente incompleta: "+str(sorted(points)))
    theory=make_theory(points)
    practice=make_practice()
    (M4/"TEORIA").mkdir(parents=True,exist_ok=True)
    (M4/"PRACTICA").mkdir(parents=True,exist_ok=True)
    traceability=make_source_traceability()
    (M4/"TEORIA"/"M04_TEORIA.md").write_text(theory,encoding="utf-8")
    (M4/"PRACTICA"/"M04_PRACTICA.md").write_text(practice,encoding="utf-8")
    (M4/"TRAZABILIDAD_FUENTE_M04.md").write_text(traceability,encoding="utf-8")
    print("TEORIA chars="+str(len(theory)))
    print("PRACTICA chars="+str(len(practice)))

if __name__=="__main__":
    main()
