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

def strip_code(lines):
    out=[]; in_code=False
    markers={"csharp","sql","bash","text","powershell"}
    prose=("La ","El ","Las ","Los ","Esta ","Este ","Resultado","Observaciones","Error común","Solución","A partir ","En ","Cuando ","Como ","Por ","Aunque ","Si ","EF Core ")
    for line in lines:
        s=line.strip()
        if s.lower() in markers:
            in_code=True; continue
        if in_code:
            if not s:
                continue
            if s.startswith(prose) and not any(x in s for x in ("=>","==",";","{","}")):
                in_code=False
            else:
                continue
        out.append(line)
    return out

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
    ("Los filtros no traducibles provocan que la consulta se ejecute en memoria.",
     "Los filtros no traducibles en Where provocan una excepción salvo que el desarrollador establezca explícitamente la frontera cliente."),
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
    for a,b in pairs:
        text=text.replace(a,b)
    if n==3:
        text=text.replace("las planchas que aparecen en varias órdenes","las entidades compartidas que aparecen varias veces")
        text=text.replace("una plancha apareciera dos veces","una misma entidad compartida apareciera dos veces")
    if n==8:
        text=text.replace("Para garantizar la coherencia, se debe usar una transacción explícita.","Si se necesita una instantánea consistente, debe elegirse una transacción y un nivel de aislamiento que proporcionen esa garantía.")
    return text

def theory_parts(block,n):
    lines=block.splitlines()
    audience=next((x for x in lines if x.startswith("Audiencia:")),"")
    project=next((x for x in lines if x.startswith("Proyecto:")),"")
    oi=lines.index("Objetivos de aprendizaje")
    ti=lines.index("Teoría")
    pi=lines.index("Práctica") if "Práctica" in lines else len(lines)
    objectives=[x.strip() for x in lines[oi+1:ti] if x.strip()]
    body=strip_code(lines[ti+1:pi])
    md=[]
    for i,raw in enumerate(body):
        s=raw.strip()
        if not s:
            md.append(""); continue
        nxt=next((x.strip() for x in body[i+1:] if x.strip()),"")
        heading=(len(s)<=100 and not s.endswith((".",";",",",":")) and "\t" not in s and nxt and not re.match(r"^(SELECT|FROM|WHERE|ORDER|LEFT|INNER|var |return |public |private )",s,re.I))
        md.append(("#### "+s) if heading else s)
    return audience,project,objectives,semantic_fixes("\n".join(md),n)

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
    return "Participa directamente en el flujo validado del checkpoint: "+s

def line_notes(code,label):
    out=["#### Explicación línea a línea — "+label,""]
    for i,line in enumerate(code.splitlines(),1):
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
        audience,project,objectives,body=theory_parts(points[n],n)
        out+=["## Punto 4."+str(n)+" — "+title,"","**"+audience+"**","","**"+project+"**","",
        "### Objetivos de aprendizaje",""]
        out += ["- "+x.rstrip(".")+"." for x in objectives]
        out+=["","### Precisión técnica validada para EF Core 8","",correction,""]
        if n in OFFICIAL: out+=["Referencia técnica de contraste: "+OFFICIAL[n],""]
        out+=["### Desarrollo teórico","",body,"",
        "### Anclaje en AceriaData","",
        "El concepto está materializado en M04/PROYECTO/4."+str(n)+" y el checkpoint ha pasado restore, build, migraciones y E2E sobre SQL Server LocalDB.","",
        "**Reto conceptual.** "+challenge,"","**Analogía operativa.** "+analogy,"",
        "### Criterios de salida","",
        "- Relacionar LINQ con SQL o comandos ejecutados.",
        "- Distinguir coste de servidor, transferencia, materialización y tracking.",
        "- Justificar técnicas aplicadas y descartadas.",
        "- Ejecutar el checkpoint y obtener 4."+str(n)+" OK.","","---",""]
    return "\n".join(out)

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
        out+=["## Punto 4."+str(n)+" — "+title,"","### Contexto del proyecto","",
        "Este checkpoint continúa "+previous+". Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.","",
        "### Objetivo práctico","",correction,"",
        "### Paso 1: Abrir el checkpoint","",fence("cd M04/PROYECTO/4."+str(n)+"\ndotnet restore AceriaData.sln","powershell"),"",
        "### Paso 2: Comprobar migraciones","",fence("dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console","powershell"),"",
        "Debe seguir apareciendo M2_2_12_Architecture.","",
        "### Paso 3: Identificar el delta docente","",
        "El archivo principal del delta es src/AceriaData.Infrastructure/Repositories/"+repo+". El checkpoint conserva todo el estado anterior.","",
        "### Paso 4: Implementar y estudiar Infrastructure","",fence(repocode),""]
        for rel in extra:
            code=(d/rel).read_text(encoding="utf-8").strip()
            out+=["Archivo complementario: "+rel,"",fence(code,"csharp" if rel.endswith(".cs") else ""),""]
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
        "### Analogía operativa","",analogy,"",
        "### Resultado esperado","",
        "El checkpoint 4."+str(n)+" queda ejecutable, trazado y reproducible, y sirve como baseline física del punto siguiente.","",
        "### Conexión con el siguiente punto","",
        ("El siguiente estado es 4."+str(n+1)+" y parte físicamente de este checkpoint." if n<12 else "Este punto cierra el código acumulativo de M4 y consolida el checklist."),"","---",""]
        previous="M04/PROYECTO/4."+str(n)
    return "\n".join(out)

def main():
    points=split_points(load_source())
    if set(points)!=set(range(1,13)):
        raise RuntimeError("Fuente incompleta: "+str(sorted(points)))
    theory=make_theory(points)
    practice=make_practice()
    (M4/"TEORIA").mkdir(parents=True,exist_ok=True)
    (M4/"PRACTICA").mkdir(parents=True,exist_ok=True)
    (M4/"TEORIA"/"M04_TEORIA.md").write_text(theory,encoding="utf-8")
    (M4/"PRACTICA"/"M04_PRACTICA.md").write_text(practice,encoding="utf-8")
    print("TEORIA chars="+str(len(theory)))
    print("PRACTICA chars="+str(len(practice)))

if __name__=="__main__":
    main()
