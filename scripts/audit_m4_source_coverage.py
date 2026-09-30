from pathlib import Path
import html
import re
import unicodedata

ROOT = Path(__file__).resolve().parents[1]
M4 = ROOT / "M04"

def norm(text: str) -> str:
    text = html.unescape(text)
    text = unicodedata.normalize("NFKD", text)
    text = "".join(c for c in text if not unicodedata.combining(c))
    text = text.lower()
    text = re.sub(r"[^a-z0-9áéíóúñü]+", " ", text)
    return re.sub(r"\s+", " ", text).strip()

def clean_source_line(text: str) -> str:
    return text.strip().rstrip("\\").strip()

source = (
    (M4 / "SOURCE" / "M04_FUENTE_4_1_4_6.txt").read_text(encoding="utf-8")
    + "\n"
    + (M4 / "SOURCE" / "M04_FUENTE_4_7_4_12.md").read_text(encoding="utf-8")
)
theory = (M4 / "TEORIA" / "M04_TEORIA.md").read_text(encoding="utf-8")
practice = (M4 / "PRACTICA" / "M04_PRACTICA.md").read_text(encoding="utf-8")
trace = (M4 / "TRAZABILIDAD_FUENTE_M04.md").read_text(encoding="utf-8")

source = html.unescape(source).replace("&#x20;", "")
source = source.replace("\\<", "<").replace("\\>", ">").replace("\\_", "_")

point_matches = list(re.finditer(r"(?m)^Punto 4\.(\d+)\s+[–-].*$", source))
if len(point_matches) != 12:
    raise RuntimeError(f"Fuente M4: se esperaban 12 puntos y se encontraron {len(point_matches)}")

for i, match in enumerate(point_matches):
    n = int(match.group(1))
    end = point_matches[i + 1].start() if i + 1 < len(point_matches) else len(source)
    block = source[match.start():end]
    lines = block.splitlines()

    try:
        oi = next(j for j, x in enumerate(lines) if clean_source_line(x) == "Objetivos de aprendizaje")
        ti = next(j for j, x in enumerate(lines) if clean_source_line(x) == "Teoría")
    except StopIteration:
        raise RuntimeError(f"Fuente 4.{n}: faltan Objetivos o Teoría")

    objectives = [clean_source_line(x) for x in lines[oi + 1:ti] if clean_source_line(x)]
    final_block = theory.split(f"## Punto 4.{n} ", 1)[1]
    if n < 12:
        final_block = final_block.split(f"## Punto 4.{n+1} ", 1)[0]
    final_norm = norm(final_block)

    for objective in objectives:
        if norm(objective) not in final_norm:
            raise RuntimeError(f"4.{n}: objetivo de fuente no conservado: {objective}")

    practice_block = practice.split(f"## Punto 4.{n} ", 1)[1]
    if n < 12:
        practice_block = practice_block.split(f"## Punto 4.{n+1} ", 1)[0]
    for required in (
        "### Trazabilidad con la práctica fuente",
        "Reto de ampliación procedente de la fuente",
        "Errores de la fuente que deben seguir siendo diagnosticables",
    ):
        if required not in practice_block:
            raise RuntimeError(f"4.{n}: falta sección de cobertura de fuente: {required}")

    if f"## 4.{n} -" not in trace:
        raise RuntimeError(f"TRAZABILIDAD_FUENTE_M04: falta 4.{n}")

for forbidden in (
    "Los filtros no traducibles provocan que la consulta se ejecute en memoria.",
    "Cada consulta de una Split Query se ejecuta en una transacción separada.",
    "La consulta compilada reutiliza el SQL generado y el plan de ejecución.",
    "AsSplitQuery no se debe usar con una sola colección.",
):
    if forbidden.lower() in theory.lower() or forbidden.lower() in practice.lower():
        raise RuntimeError(f"Permanece afirmación técnica incorrecta: {forbidden}")

required_terms = {
    1: ("ToQueryString", "Soft Delete", "múltiples"),
    2: ("ChangeTracker", "AsTracking", "Clear"),
    3: ("ReferenceEqualityComparer", "Aleacion", "misma clave"),
    4: ("N+1", "comandos", "Lazy Loading"),
    5: ("ThenInclude", "AsSplitQuery", "proyección"),
    6: ("columnas", "filas", "4.10"),
    7: ("InvalidOperationException", "AsEnumerable", "collation"),
    8: ("AsSingleQuery", "aislamiento", "configuración global"),
    9: ("CompileAsyncQuery", "hot paths", "SQL Server"),
    10: ("FechaCreacion", "Id", "cursor"),
    11: ("DiagnosticSource", "DbCommandInterceptor", "TagWith"),
    12: ("anti-patrones", "descartada", "Estado acumulativo real"),
}
combined = theory + "\n" + practice + "\n" + trace
for n, terms in required_terms.items():
    for term in terms:
        if term.lower() not in combined.lower():
            raise RuntimeError(f"Cobertura fuente 4.{n}: falta concepto {term}")

print("M4 SOURCE COVERAGE QA PASS: 12/12 puntos, objetivos y prácticas trazados")
