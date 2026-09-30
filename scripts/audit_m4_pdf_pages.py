from pathlib import Path
import re
import fitz

ROOT = Path(__file__).resolve().parents[1] / "M04"
DOCS = [
    ("TEORIA", ROOT / "TEORIA" / "M04_TEORIA.pdf", 50),
    ("PRACTICA", ROOT / "PRACTICA" / "M04_PRACTICA.pdf", 60),
]
FORBIDDEN = ("the user wants","we need to","let me think","esperando confirmación","material fuente","corrección técnica:")

for label, path, minimum in DOCS:
    doc = fitz.open(path)
    if len(doc) < minimum:
        raise RuntimeError(f"{label}: PDF demasiado corto: {len(doc)} < {minimum}")
    joined = []

    def meaningful_blocks(page):
        result = []
        for block in page.get_text("blocks"):
            block_text = str(block[4]).strip()
            compact = re.sub(r"\s+", " ", block_text)
            if not compact:
                continue
            if compact.startswith("CURSO: Curso Profesional de Entity Framework Core 8"):
                continue
            if compact.startswith("AceriaData · Módulo 4"):
                continue
            if re.fullmatch(r"Página \d+ de \d+", compact):
                continue
            result.append(block)
        return result

    for idx, page in enumerate(doc, 1):
        text = page.get_text("text")
        joined.append(text)
        body = meaningful_blocks(page)
        body_text = "\n".join(str(b[4]) for b in body)
        if len(re.sub(r"\s+", "", body_text)) < 20:
            raise RuntimeError(f"{label}: página {idx} sin contenido útil")
        if "JAIME GALLO" not in text:
            raise RuntimeError(f"{label}: falta autor en página {idx}")
        low = text.lower()
        if any(x in low for x in FORBIDDEN):
            raise RuntimeError(f"{label}: metacontenido en página {idx}")
        for block in page.get_text("blocks"):
            x0,y0,x1,y1,*_ = block
            if x0 < -2 or y0 < -2 or x1 > page.rect.width + 2 or y1 > page.rect.height + 2:
                raise RuntimeError(f"{label}: bloque fuera de página {idx}")
    last_page = doc[-1]
    body_blocks = meaningful_blocks(last_page)
    if body_blocks:
        body_bottom = max(b[3] for b in body_blocks)
        if body_bottom < 180:
            raise RuntimeError(
                f"{label}: última página huérfana; contenido útil termina en y={body_bottom:.1f}"
            )
    full = "\n".join(joined)
    for n in range(1,13):
        if f"Punto 4.{n}" not in full:
            raise RuntimeError(f"{label}: falta Punto 4.{n}")
    print(f"PDF PAGE QA PASS {label}: {len(doc)} páginas revisadas y renderizables")
print("PDF PAGE QA PASS: teoría y práctica M4 verificadas página por página.")
