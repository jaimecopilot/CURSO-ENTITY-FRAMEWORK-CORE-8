from pathlib import Path
import fitz
ROOT = Path(__file__).resolve().parents[1] / "M05"
OUT = ROOT / "QA_RENDER"
DOCS = [
    ("TEORIA", ROOT / "TEORIA" / "M05_TEORIA.pdf"),
    ("PRACTICA", ROOT / "PRACTICA" / "M05_PRACTICA.pdf"),
]
matrix = fitz.Matrix(1.35, 1.35)
for label, path in DOCS:
    doc = fitz.open(path)
    dest = OUT / label
    dest.mkdir(parents=True, exist_ok=True)
    for pno, page in enumerate(doc, 1):
        pix = page.get_pixmap(matrix=matrix, alpha=False)
        pix.save(dest / f"page-{pno:03d}.png")
    print(f"RENDER {label}: {len(doc)} páginas")
