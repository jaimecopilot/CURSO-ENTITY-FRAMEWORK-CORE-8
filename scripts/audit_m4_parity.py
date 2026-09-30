from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
M3GEN = (ROOT / "scripts" / "generate_m3_pdfs.py").read_text(encoding="utf-8")
M4GEN = (ROOT / "scripts" / "generate_m4_pdfs.py").read_text(encoding="utf-8")
PRACTICE = (ROOT / "M04" / "PRACTICA" / "M04_PRACTICA.md").read_text(encoding="utf-8")
THEORY = (ROOT / "M04" / "TEORIA" / "M04_TEORIA.md").read_text(encoding="utf-8")

def extract(text: str, start: str, end: str) -> str:
    a = text.index(start) + len(start)
    b = text.index(end, a)
    return text[a:b]

css3 = extract(M3GEN, 'CSS = r"""', '"""\n\ndef toc')
css4 = extract(M4GEN, 'CSS = r"""', '"""\n\ndef toc')
if css3 != css4:
    raise RuntimeError("M4: el CSS PDF ya no coincide exactamente con el sistema visual de M3")

line_rows3 = extract(M3GEN, "def line_rows(body):", "def build(src, dst, kind, min_pages):")
line_rows4 = extract(M4GEN, "def line_rows(body):", "def build(src, dst, kind, min_pages):")
if line_rows3 != line_rows4:
    raise RuntimeError("M4: el renderer de explicaciones línea a línea difiere de M3")

for token in (
    "size:A4",
    "margin:15mm 15mm 17mm 15mm",
    "AUTOR: JAIME GALLO",
    'class="cover"',
    'class="toc"',
    "line-row",
    "code-block",
    "final-point",
):
    if token not in M4GEN:
        raise RuntimeError(f"M4 PDF: falta patrón visual heredado de M3: {token}")

required_practice_headings = (
    "### Contexto del proyecto",
    "### Objetivo práctico",
    "### Paso 1: Abrir el proyecto del punto",
    "### Paso 2: Comprobar migraciones",
    "### Paso 3: Revisar los cambios de este punto",
    "### Paso 4: Implementar y estudiar Infrastructure",
    "### Paso 5: Implementar el caso de uso",
    "### Paso 6: Preparar el composition root",
    "### Paso 7: Compilar",
    "### Paso 8: Ejecutar en LocalDB",
    "### Paso 9: Diagnóstico técnico",
    "### Paso 10: Cierre acumulativo",
    "### Errores comunes",
    "### Analogía operativa",
    "### Resultado esperado",
)

for n in range(1, 13):
    start = PRACTICE.index(f"## Punto 4.{n} ")
    end = PRACTICE.index(f"## Punto 4.{n+1} ", start) if n < 12 else len(PRACTICE)
    sec = PRACTICE[start:end]
    for heading in required_practice_headings:
        if heading not in sec:
            raise RuntimeError(f"4.{n}: falta patrón de práctica heredado: {heading}")
    steps = [int(x) for x in re.findall(r"(?m)^### Paso (\d+):", sec)]
    if steps != list(range(1, 11)):
        raise RuntimeError(f"4.{n}: secuencia de pasos no coincide con patrón M3: {steps}")
    if sec.count("#### Explicación línea a línea — ") < 3:
        raise RuntimeError(
            f"4.{n}: se esperaban al menos Repository, UseCase y Program explicados línea a línea"
        )

for n in range(1, 13):
    start = THEORY.index(f"## Punto 4.{n} ")
    end = THEORY.index(f"## Punto 4.{n+1} ", start) if n < 12 else len(THEORY)
    sec = THEORY[start:end]
    if sec.count("**Ejemplo ") < 5:
        raise RuntimeError(f"4.{n}: densidad de ejemplos teóricos inferior al patrón fuente/M3")
    if "### Ejemplo ejecutable del concepto" not in sec:
        raise RuntimeError(f"4.{n}: falta anclaje ejecutable de AceriaData")

print("M4 PARITY QA PASS: estructura docente, renderer y CSS alineados con M3")
