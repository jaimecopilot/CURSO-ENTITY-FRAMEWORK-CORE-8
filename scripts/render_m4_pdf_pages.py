from pathlib import Path
import fitz

ROOT = Path(__file__).resolve().parents[1] / "M04"
OUT = ROOT / "QA_RENDER"
DOCS = [
    ("TEORIA", ROOT / "TEORIA" / "M04_TEORIA.pdf"),
    ("PRACTICA", ROOT / "PRACTICA" / "M04_PRACTICA.pdf"),
]
matrix = fitz.Matrix(1.35, 1.35)
for label, path in DOCS:
    doc = fitz.open(path)
    d = OUT / label
    d.mkdir(parents=True, exist_ok=True)
    for pno, page in enumerate(doc, 1):
        pix = page.get_pixmap(matrix=matrix, alpha=False)
        pix.save(d / f"page-{pno:03d}.png")
    print(f"RENDER {label}: {len(doc)} páginas")
