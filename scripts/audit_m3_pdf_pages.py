from pathlib import Path
import re
import fitz

ROOT = Path(__file__).resolve().parents[1] / "M03"
DOCS = [
    ("TEORIA", ROOT / "TEORIA" / "M03_TEORIA.pdf", 40),
    ("PRACTICA", ROOT / "PRACTICA" / "M03_PRACTICA.pdf", 60),
]
FORBIDDEN = ("the user wants","we need to","let me think","esperando confirmación","material fuente","corrección técnica:")

for label, path, minimum in DOCS:
    doc = fitz.open(path)
    if len(doc) < minimum:
        raise RuntimeError(f"{label}: PDF demasiado corto: {len(doc)} < {minimum}")
    joined = []
    for idx, page in enumerate(doc, 1):
        text = page.get_text("text")
        joined.append(text)
        if len(re.sub(r"\s+","",text)) < 70:
            raise RuntimeError(f"{label}: página {idx} prácticamente vacía")
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
    body_blocks = [
        b for b in last_page.get_text("blocks")
        if b[1] >= 35 and b[3] <= last_page.rect.height - 35
        and re.sub(r"\\s+", "", str(b[4]))
    ]
    if body_blocks:
        body_bottom = max(b[3] for b in body_blocks)
        if body_bottom < 180:
            raise RuntimeError(
                f"{label}: última página huérfana; contenido útil termina en y={body_bottom:.1f}"
            )
    full = "\n".join(joined)
    for n in range(1,13):
        if f"Punto 3.{n}" not in full:
            raise RuntimeError(f"{label}: falta Punto 3.{n}")
    print(f"PDF PAGE QA PASS {label}: {len(doc)} páginas revisadas y renderizables")
print("PDF PAGE QA PASS: teoría y práctica M3 verificadas página por página.")
