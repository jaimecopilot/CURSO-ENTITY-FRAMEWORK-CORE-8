from pathlib import Path
import re
import fitz

ROOT = Path(__file__).resolve().parents[1] / "M02"
DOCS = [
    ("TEORIA", ROOT / "TEORIA" / "M02_TEORIA.pdf", 60),
    ("PRACTICA", ROOT / "PRACTICA" / "M02_PRACTICA.pdf", 70),
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
    "the user wants",
    "we need to",
    "let me think",
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

        compact = re.sub(r"\s+", "", text)
        if len(compact) < 80:
            raise RuntimeError(
                f"{label}: página {idx} prácticamente vacía "
                f"({len(compact)} caracteres útiles)"
            )

        if "JAIME GALLO" not in text:
            raise RuntimeError(f"{label}: falta AUTOR: JAIME GALLO en página {idx}")

        lower = text.lower()
        found = [x for x in FORBIDDEN if x in lower]
        if found:
            raise RuntimeError(
                f"{label}: página {idx} contiene metanotas internas: {found}"
            )

        rect = page.rect
        for block in page.get_text("blocks"):
            x0, y0, x1, y1 = block[:4]
            if (
                x0 < -0.5 or y0 < -0.5
                or x1 > rect.width + 0.5
                or y1 > rect.height + 0.5
            ):
                raise RuntimeError(
                    f"{label}: bloque fuera de página {idx}: "
                    f"{(x0, y0, x1, y1)} / {rect}"
                )

        pix = page.get_pixmap(
            matrix=fitz.Matrix(0.35, 0.35),
            colorspace=fitz.csGRAY,
            alpha=False,
        )
        samples = pix.samples
        nonwhite = sum(1 for b in samples if b < 248)
        if nonwhite / max(1, len(samples)) < 0.002:
            raise RuntimeError(
                f"{label}: página {idx} parece visualmente vacía al renderizar"
            )

        if idx > 1 and f"Página {idx} de {len(doc)}" not in text:
            raise RuntimeError(
                f"{label}: numeración incorrecta o ausente en página {idx}"
            )

    full_text = "\n".join(joined)
    for n in range(1, 13):
        if f"Punto 2.{n}" not in full_text:
            raise RuntimeError(f"{label}: falta el Punto 2.{n}")

    print(
        f"PDF PAGE QA PASS {label}: "
        f"{len(doc)} páginas revisadas y renderizadas"
    )

practice = fitz.open(ROOT / "PRACTICA" / "M02_PRACTICA.pdf")
all_practice = "\n".join(p.get_text("text") for p in practice)
for token in (
    "Punto 2.10", "Filtros globales", "IgnoreQueryFilters",
    "Punto 2.11", "Soft Delete", "IsDeleted", "DeletedAt",
    "Punto 2.12", "Arquitectura Hexagonal",
):
    if token not in all_practice:
        raise RuntimeError(f"PRACTICA: falta evidencia de {token}")

print("PDF PAGE QA PASS: teoría y práctica M2 verificadas página por página.")
