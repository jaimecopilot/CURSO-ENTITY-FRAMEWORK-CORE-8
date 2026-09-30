from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]

M3_PDF = (ROOT / "scripts/generate_m3_pdfs.py").read_text(encoding="utf-8")
M4_PDF = (ROOT / "scripts/generate_m4_pdfs.py").read_text(encoding="utf-8")
M3_THEORY = (ROOT / "M03/TEORIA/M03_TEORIA.md").read_text(encoding="utf-8")
M3_PRACTICE = (ROOT / "M03/PRACTICA/M03_PRACTICA.md").read_text(encoding="utf-8")
M4_THEORY = (ROOT / "M04/TEORIA/M04_TEORIA.md").read_text(encoding="utf-8")
M4_PRACTICE = (ROOT / "M04/PRACTICA/M04_PRACTICA.md").read_text(encoding="utf-8")

def extract(name: str, text: str, start: str, end: str) -> str:
    a = text.index(start) + len(start)
    b = text.index(end, a)
    return text[a:b]

# El sistema visual base debe ser exactamente el de M3.
css3 = extract("M3 CSS", M3_PDF, 'CSS = r"""', '"""\n\ndef toc')
css4 = extract("M4 CSS", M4_PDF, 'CSS = r"""', '"""\n\ndef toc')
if css3 != css4:
    raise RuntimeError("M4 PDF: el CSS base ya no es idéntico al sistema visual validado de M3")

pyg3 = re.search(r'PYG = (.+)', M3_PDF).group(1)
pyg4 = re.search(r'PYG = (.+)', M4_PDF).group(1)
if pyg3 != pyg4:
    raise RuntimeError("M4 PDF: la configuración Pygments difiere de M3")

def function_body(text: str, name: str, next_name: str) -> str:
    a = text.index(f"def {name}(")
    b = text.index(f"def {next_name}(", a)
    return text[a:b].strip()

if function_body(M3_PDF, "line_rows", "build") != function_body(M4_PDF, "line_rows", "build"):
    raise RuntimeError("M4 PDF: line_rows ya no coincide con M3")

# La estructura docente de práctica mantiene el contrato fuerte de M3: 12 puntos,
# pasos 1..10 continuos y explicaciones línea a línea.
def point_section(md: str, module: int, n: int) -> str:
    start = md.index(f"## Punto {module}.{n}")
    end = md.index(f"## Punto {module}.{n+1}", start) if n < 12 else len(md)
    return md[start:end]

for module, practice in ((3, M3_PRACTICE), (4, M4_PRACTICE)):
    for n in range(1,13):
        sec = point_section(practice,module,n)
        steps = [int(x) for x in re.findall(r"(?m)^### Paso (\d+):", sec)]
        if steps != list(range(1,11)):
            raise RuntimeError(f"M{module} {module}.{n}: secuencia de pasos no es 1..10: {steps}")
        if "Explicación línea a línea" not in sec:
            raise RuntimeError(f"M{module} {module}.{n}: falta patrón de explicación línea a línea")

# La teoría de M4 no puede ser más pobre en ejemplos que su propia fuente ni
# volver al patrón anómalo de un único bloque por punto.
for n in range(1,13):
    sec = point_section(M4_THEORY,4,n)
    source_examples = len(re.findall(r"(?m)^\*\*Ejemplo \d+ \([A-Z]+\)\.\*\*$", sec))
    if source_examples < 8:
        raise RuntimeError(f"M4 4.{n}: densidad de ejemplos teóricos anómala: {source_examples}")

# M3 sirve de referencia de riqueza de teoría: todos sus puntos tienen >1 bloque.
for n in range(1,13):
    sec = point_section(M3_THEORY,3,n)
    fences = len(re.findall(r"(?m)^\x60\x60\x60\w*\s*$", sec)) // 2
    if fences <= 1:
        raise RuntimeError(f"M3 3.{n}: patrón de referencia inesperado ({fences} bloques)")

print("M4 PRIOR-PATTERN PARITY PASS: estructura MD y sistema visual PDF alineados con M3")
