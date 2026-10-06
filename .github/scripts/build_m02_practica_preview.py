from pathlib import Path

import mistune
from pygments import highlight
from pygments.formatters import HtmlFormatter
from pygments.lexers import TextLexer, get_lexer_by_name
from pygments.util import ClassNotFound
from weasyprint import HTML

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "M02" / "PRACTICA" / "M02_PRACTICA.md"
OUT_DIR = ROOT / "M02" / "PRACTICA" / "_preview"

class CourseRenderer(mistune.HTMLRenderer):
    def __init__(self):
        super().__init__(escape=False)
        self.formatter = HtmlFormatter(cssclass="highlight", style="friendly")

    def block_code(self, code, info=None):
        language = (info or "text").strip().split()[0]
        try:
            lexer = get_lexer_by_name(language)
        except ClassNotFound:
            lexer = TextLexer()
        return highlight(code, lexer, self.formatter)

def extract_point(markdown_text: str, point: str, next_point: str) -> str:
    start = markdown_text.find(f"## Punto {point}")
    end = markdown_text.find(f"## Punto {next_point}", start + 1)
    if start < 0 or end < 0:
        raise RuntimeError(f"No se puede extraer {point}.")
    return markdown_text[start:end].strip()

def build_pdf(renderer, markdown, source, point, next_point):
    section = extract_point(source, point, next_point)
    body = markdown(section)
    pygments_css = renderer.formatter.get_style_defs(".highlight")

    css = f"""
    @page {{
        size: A4;
        margin: 16mm 15mm 18mm 15mm;
        @top-center {{
            content: "CURSO: Curso Profesional de Entity Framework Core 8 · MÓDULO 2. Modelado de datos con Entity Framework Core - PRÁCTICAS · AUTOR: JAIME GALLO";
            font-family: "DejaVu Sans", sans-serif;
            font-size: 6.8pt;
            color: #6b7280;
        }}
        @bottom-left {{
            content: "AceriaData · Módulo 2 · Prácticas";
            font-family: "DejaVu Sans", sans-serif;
            font-size: 6.8pt;
            color: #9ca3af;
        }}
        @bottom-right {{
            content: "Página " counter(page) " de " counter(pages);
            font-family: "DejaVu Sans", sans-serif;
            font-size: 6.8pt;
            color: #9ca3af;
        }}
    }}
    html {{ font-family: "DejaVu Sans", Arial, sans-serif; color: #273444; }}
    body {{ font-size: 9.4pt; line-height: 1.38; }}
    h2 {{
        color: #123f67;
        font-size: 17pt;
        line-height: 1.15;
        border-bottom: 1.4pt solid #c7d8e6;
        margin: 2mm 0 5mm;
        padding-bottom: 2mm;
    }}
    h3 {{
        color: #174f7a;
        font-size: 11.5pt;
        line-height: 1.2;
        margin: 5mm 0 2.2mm;
        padding-left: 2.5mm;
        border-left: 3pt solid #2a79a8;
        break-after: avoid;
    }}
    p {{ margin: 0 0 2.6mm; text-align: left; }}
    ul, ol {{ margin: 1.5mm 0 3mm 6mm; padding-left: 4mm; }}
    li {{ margin-bottom: 1mm; }}
    strong {{ color: #1f3345; }}
    code {{
        font-family: "DejaVu Sans Mono", monospace;
        font-size: 0.92em;
        color: #17324a;
        background: #f1f5f8;
        padding: 0.2mm 0.7mm;
        border-radius: 1mm;
    }}
    .highlight {{
        background: #f7f9fb;
        border: 0.6pt solid #d9e2e8;
        border-left: 3pt solid #5b8fb3;
        border-radius: 1.2mm;
        margin: 2.3mm 0 3.2mm;
        padding: 2.5mm 3mm;
        break-inside: auto;
    }}
    .highlight pre {{
        margin: 0;
        white-space: pre-wrap;
        overflow-wrap: anywhere;
        font-family: "DejaVu Sans Mono", monospace;
        font-size: 7.6pt;
        line-height: 1.33;
    }}
    table {{
        width: 100%;
        border-collapse: collapse;
        margin: 2.5mm 0 4mm;
        font-size: 8.2pt;
    }}
    th {{
        background: #e8f1f7;
        color: #173f5f;
        font-weight: 700;
        text-align: left;
        border: 0.6pt solid #c5d3dd;
        padding: 2mm;
    }}
    td {{
        border: 0.6pt solid #d5dee5;
        padding: 2mm;
        vertical-align: top;
    }}
    tr:nth-child(even) td {{ background: #fafcfd; }}
    blockquote {{
        margin: 2.5mm 0;
        padding: 2.5mm 3mm;
        background: #eef6fb;
        border-left: 3pt solid #2c78a4;
    }}
    {pygments_css}
    """

    document = f"""<!doctype html>
    <html lang="es">
    <head>
      <meta charset="utf-8">
      <title>M02 Práctica {point} Preview</title>
      <style>{css}</style>
    </head>
    <body>{body}</body>
    </html>"""

    out_pdf = OUT_DIR / f"M02_PRACTICA_{point}_PREVIEW.pdf"
    HTML(string=document, base_url=str(ROOT)).write_pdf(str(out_pdf))
    print(out_pdf)

def main():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    source = SOURCE.read_text(encoding="utf-8")
    renderer = CourseRenderer()
    markdown = mistune.create_markdown(renderer=renderer, plugins=["table"])

    build_pdf(renderer, markdown, source, "2.1", "2.2")
    build_pdf(renderer, markdown, source, "2.2", "2.3")

if __name__ == "__main__":
    main()
