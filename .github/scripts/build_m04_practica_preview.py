from pathlib import Path

import mistune
from pygments import highlight
from pygments.formatters import HtmlFormatter
from pygments.lexers import TextLexer, get_lexer_by_name
from pygments.util import ClassNotFound
from weasyprint import HTML

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "M04" / "PRACTICA" / "M04_PRACTICA.md"
OUT_DIR = ROOT / "M04" / "PRACTICA" / "_preview"
FINAL_PDF = ROOT / "M04" / "PRACTICA" / "M04_PRACTICA.pdf"


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
        rendered = highlight(code, lexer, self.formatter)
        # Los bloques pedagógicos cortos deben permanecer juntos. El Program.cs
        # acumulativo puede superar una página completa: en ese caso permitir
        # fragmentación evita desplazar todo el bloque a la página siguiente.
        if code.count("\n") + 1 >= 40:
            rendered = rendered.replace(
                'class="highlight"',
                'class="highlight highlight-long"',
                1,
            )
        return rendered


def extract_point(markdown_text: str, point: str, next_point: str | None = None) -> str:
    start = markdown_text.find(f"## Punto {point}")
    if start < 0:
        raise RuntimeError(f"No se puede extraer {point}.")
    end = markdown_text.find(f"## Punto {next_point}", start + 1) if next_point else len(markdown_text)
    if next_point and end < 0:
        raise RuntimeError(f"No se puede localizar el punto siguiente {next_point} para extraer {point}.")
    return markdown_text[start:end].strip()


def course_css(renderer, full_document: bool = False) -> str:
    pygments_css = renderer.formatter.get_style_defs(".highlight")
    point_break = """
    body > h2 { break-before: page; }
    """ if full_document else ""

    return f"""
    @page {{
        size: A4;
        margin: 16mm 15mm 18mm 15mm;
        @top-center {{
            content: "CURSO: Curso Profesional de Entity Framework Core 8 · MÓDULO 4. Optimización y rendimiento - PRÁCTICAS · AUTOR: JAIME GALLO";
            font-family: "DejaVu Sans", sans-serif;
            font-size: 6.8pt;
            color: #6b7280;
        }}
        @bottom-left {{
            content: "AceriaData · Módulo 4 · Prácticas";
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
    h1 {{
        color: #123f67;
        font-size: 20pt;
        line-height: 1.15;
        border-bottom: 1.6pt solid #c7d8e6;
        margin: 3mm 0 6mm;
        padding-bottom: 2.5mm;
    }}
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
    h4 {{
        color: #1f3345;
        font-size: 9.8pt;
        line-height: 1.2;
        margin: 4mm 0 1.8mm;
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
        break-inside: avoid;
    }}
    .highlight.highlight-long {{
        break-inside: auto;
    }}
    .highlight.highlight-long pre {{
        orphans: 3;
        widows: 3;
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
    {point_break}
    {pygments_css}
    /* Pygments clasifica directivas como #nullable parcialmente como Error.
       Su estilo por defecto dibuja un recuadro rojo; en C# válido ese borde
       es un artefacto visual, no un error del material. */
    .highlight .err {{
        border: 0 !important;
        background: transparent !important;
    }}
    """


def preserve_line_explanations(markdown_text: str) -> str:
    import re

    lines = markdown_text.splitlines()
    out = []
    in_fence = False

    for line in lines:
        if line.lstrip().startswith("```"):
            in_fence = not in_fence
            out.append(line)
            continue

        if not in_fence and re.match(r"^Línea\s+\d+:", line):
            out.append(line)
            out.append("")
        else:
            out.append(line)

    return "\n".join(out)

def render_pdf(renderer, markdown, markdown_text: str, out_pdf: Path, title: str, full_document: bool = False):
    markdown_text = preserve_line_explanations(markdown_text)
    body = markdown(markdown_text)
    css = course_css(renderer, full_document=full_document)
    document = f"""<!doctype html>
    <html lang="es">
    <head>
      <meta charset="utf-8">
      <title>{title}</title>
      <style>{css}</style>
    </head>
    <body>{body}</body>
    </html>"""
    HTML(string=document, base_url=str(ROOT)).write_pdf(str(out_pdf))
    print(out_pdf)


def build_preview(renderer, markdown, source, point, next_point=None):
    section = extract_point(source, point, next_point)
    out_pdf = OUT_DIR / f"M04_PRACTICA_{point}_PREVIEW.pdf"
    render_pdf(
        renderer,
        markdown,
        section,
        out_pdf,
        f"M04 Práctica {point} Preview",
        full_document=False,
    )


def build_official_pdf(renderer, markdown, source):
    render_pdf(
        renderer,
        markdown,
        source,
        FINAL_PDF,
        "Módulo 4 - Prácticas de optimización y rendimiento en Entity Framework Core 8",
        full_document=True,
    )


def main():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    source = SOURCE.read_text(encoding="utf-8")
    renderer = CourseRenderer()
    markdown = mistune.create_markdown(renderer=renderer, plugins=["table"])

    build_preview(renderer, markdown, source, "4.1", "4.2")
    build_preview(renderer, markdown, source, "4.2", "4.3")
    build_preview(renderer, markdown, source, "4.3", "4.4")
    build_preview(renderer, markdown, source, "4.4", "4.5")
    build_preview(renderer, markdown, source, "4.5", "4.6")
    build_preview(renderer, markdown, source, "4.6", "4.7")
    build_preview(renderer, markdown, source, "4.7", "4.8")
    build_preview(renderer, markdown, source, "4.8", "4.9")
    build_preview(renderer, markdown, source, "4.9", "4.10")
    build_preview(renderer, markdown, source, "4.10", "4.11")
    build_preview(renderer, markdown, source, "4.11", "4.12")
    build_preview(renderer, markdown, source, "4.12")
    build_official_pdf(renderer, markdown, source)


if __name__ == "__main__":
    main()