from pathlib import Path
import re, sys
import fitz

ROOT = Path(__file__).resolve().parents[1] / "M01"
DOCS = [
    ("TEORIA", ROOT / "TEORIA" / "M01_TEORIA.pdf", 50),
    ("PRACTICA", ROOT / "PRACTICA" / "M01_PRACTICA.pdf", 100),
]
FORBIDDEN = (
    "material original",
    "material fuente",
    "original suministrado",
    "trazabilidad acordada",
    "prevista por la trazabilidad",
    "corrección técnica:",
    "metacontenido conversacional",
    "se han separado del material",
    "consolidación práctica de",
    "the user says",
    "let me develop",
    "esperando confirmación",
)

for label, path, min_pages in DOCS:
    doc = fitz.open(path)
    if len(doc) < min_pages:
        raise RuntimeError(f"{label}: número de páginas inesperadamente bajo: {len(doc)}")

    joined = []
    for idx, page in enumerate(doc, 1):
        text = page.get_text("text")
        joined.append(text)

        # Revisión de contenido por página.
        compact = re.sub(r"\s+", "", text)
        if len(compact) < 80:
            raise RuntimeError(f"{label}: página {idx} prácticamente vacía ({len(compact)} caracteres útiles)")

        if "JAIME GALLO" not in text:
            raise RuntimeError(f"{label}: falta AUTOR: JAIME GALLO en página {idx}")

        lower = text.lower()
        found = [x for x in FORBIDDEN if x in lower]
        if found:
            raise RuntimeError(f"{label}: página {idx} contiene metanotas internas: {found}")

        # Comprobar geometría de todos los bloques de texto.
        rect = page.rect
        for block in page.get_text("blocks"):
            x0, y0, x1, y1 = block[:4]
            if x0 < -0.5 or y0 < -0.5 or x1 > rect.width + 0.5 or y1 > rect.height + 0.5:
                raise RuntimeError(
                    f"{label}: bloque fuera de página {idx}: {(x0, y0, x1, y1)} / {rect}"
                )

        # Render real de cada página para detectar errores de composición.
        pix = page.get_pixmap(matrix=fitz.Matrix(0.35, 0.35), colorspace=fitz.csGRAY, alpha=False)
        samples = pix.samples
        # Una página válida debe contener suficiente tinta distinta del blanco.
        nonwhite = sum(1 for b in samples if b < 248)
        if nonwhite / max(1, len(samples)) < 0.002:
            raise RuntimeError(f"{label}: página {idx} parece visualmente vacía al renderizar")

        # Pie de página: salvo la portada, debe reflejar su número real.
        if idx > 1 and f"Página {idx} de {len(doc)}" not in text:
            raise RuntimeError(f"{label}: numeración incorrecta o ausente en página {idx}")

    full_text = "\n".join(joined)
    for n in range(1, 13):
        if f"Punto 1.{n}" not in full_text:
            raise RuntimeError(f"{label}: falta el Punto 1.{n}")

    print(f"PDF PAGE QA PASS {label}: {len(doc)} páginas revisadas y renderizadas")

# Comprobación específica de la zona donde se detectó el fallo de secuencia.
practice = fitz.open(ROOT / "PRACTICA" / "M01_PRACTICA.pdf")
zone = "\n".join(practice[i].get_text("text") for i in range(15, 22))
for expected in ("Paso 11:", "Paso 12:", "Paso 13:", "Paso 14:", "Paso 15:"):
    if expected not in zone:
        raise RuntimeError(f"PRACTICA: falta {expected} en la zona de 1.3")
if "Consolidación práctica de PlanchaAcero procedente del material original" in zone:
    raise RuntimeError("PRACTICA: reapareció una anotación interna en 1.3")

print("PDF PAGE QA PASS: teoría y práctica verificadas página por página.")
