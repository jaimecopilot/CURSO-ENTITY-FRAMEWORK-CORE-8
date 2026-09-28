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

        # Detectar títulos duplicados consecutivos mediante tipografía y posición.
        heading_lines = []
        page_dict = page.get_text("dict")
        for block in page_dict.get("blocks", []):
            for line in block.get("lines", []):
                spans = line.get("spans", [])
                if not spans:
                    continue
                line_text = "".join(span.get("text", "") for span in spans).strip()
                if not line_text:
                    continue
                max_size = max(float(span.get("size", 0)) for span in spans)
                bold = any("bold" in span.get("font", "").lower() for span in spans)
                if bold and max_size >= 9.0 and not line_text.startswith(("CURSO:", "AUTOR:")):
                    y0 = min(float(span.get("bbox", [0, 0, 0, 0])[1]) for span in spans)
                    heading_lines.append((y0, line_text))

        heading_lines.sort()
        for a, b in zip(heading_lines, heading_lines[1:]):
            if a[1] == b[1] and len(a[1]) >= 8 and abs(b[0] - a[0]) < 35:
                raise RuntimeError(
                    f"{label}: página {idx} contiene título duplicado consecutivo: {a[1]}"
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

# Los puntos 2.1-2.12 deben conservar su secuencia completa de pasos en el PDF.
for step in range(1, 7):
    count = len(re.findall(rf"Paso {step}:", all_practice))
    if count != 12:
        raise RuntimeError(
            f"PRACTICA PDF: Paso {step} aparece {count} veces; se esperaban 12"
        )
# Desde la ampliación de 2.11, los pasos 7 y 8 aparecen en 2.11 y 2.12.
for step in (7, 8):
    count = len(re.findall(rf"Paso {step}:", all_practice))
    if count != 2:
        raise RuntimeError(
            f"PRACTICA PDF: Paso {step} aparece {count} veces; se esperaban 2"
        )

# Los pasos 9-12 pertenecen únicamente al recorrido avanzado de migraciones 2.11.
for step in range(9, 13):
    count = len(re.findall(rf"Paso {step}:", all_practice))
    if count != 1:
        raise RuntimeError(
            f"PRACTICA PDF: Paso {step} aparece {count} veces; se esperaba 1"
        )

# Verificar que las explicaciones Línea N han llegado al PDF y no se han perdido.
line_explanations = re.findall(r"Línea\s+\d+\b", all_practice)
if len(line_explanations) < 70:
    raise RuntimeError(
        f"PRACTICA PDF: pocas explicaciones Línea N renderizadas ({len(line_explanations)})"
    )

for token in (
    "Punto 2.10", "Filtros globales", "IgnoreQueryFilters",
    "Punto 2.11", "Migraciones en el modelado", "Soft Delete", "IsDeleted", "DeletedAt",
    "M2_2_11", "__EFMigrationsHistory", "idempotent",
    "Punto 2.12", "Arquitectura Hexagonal",
):
    if token not in all_practice:
        raise RuntimeError(f"PRACTICA: falta evidencia de {token}")

print("PDF PAGE QA PASS: teoría y práctica M2 verificadas página por página.")
