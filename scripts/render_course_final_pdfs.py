from pathlib import Path
import fitz
import tempfile

ROOT = Path(__file__).resolve().parents[1]
pdfs = []
for n in range(1, 6):
    base = ROOT / f"M0{n}"
    pdfs.extend([
        (f"M{n} teoría", base / "TEORIA" / f"M0{n}_TEORIA.pdf"),
        (f"M{n} práctica", base / "PRACTICA" / f"M0{n}_PRACTICA.pdf"),
    ])

with tempfile.TemporaryDirectory(prefix="course-final-render-") as tmp:
    out = Path(tmp)
    total = 0
    for label, path in pdfs:
        doc = fitz.open(path)
        if len(doc) == 0:
            raise RuntimeError(f"{label}: PDF vacío")
        target = out / label.replace(" ", "-")
        target.mkdir(parents=True, exist_ok=True)
        for idx, page in enumerate(doc, 1):
            pix = page.get_pixmap(matrix=fitz.Matrix(0.55, 0.55), alpha=False)
            if pix.width <= 0 or pix.height <= 0 or not pix.samples:
                raise RuntimeError(f"{label}: render inválido en página {idx}")
            pix.save(target / f"page-{idx:03d}.png")
        total += len(doc)
        print(f"COURSE PDF RENDER PASS {label}: {len(doc)} páginas")
    print(f"COURSE PDF RENDER PASS TOTAL: {total} páginas renderizadas en 10 manuales.")
