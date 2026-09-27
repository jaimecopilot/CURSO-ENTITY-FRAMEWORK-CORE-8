from pathlib import Path
import re, html, unicodedata
import mistune
from pygments import highlight
from pygments.lexers import get_lexer_by_name, TextLexer
from pygments.formatters import HtmlFormatter
from weasyprint import HTML
from pypdf import PdfReader

ROOT = Path(__file__).resolve().parents[1] / 'M01'
ALIASES = {'csharp':'csharp','cs':'csharp','c#':'csharp','bash':'bash','sh':'bash','powershell':'powershell','ps1':'powershell','sql':'sql','json':'json','xml':'xml','text':'text','plaintext':'text'}

def slugify(s):
    s = unicodedata.normalize('NFKD', s).encode('ascii','ignore').decode('ascii')
    return re.sub(r'[^a-zA-Z0-9]+','-',s).strip('-').lower() or 'seccion'

class Renderer(mistune.HTMLRenderer):
    def __init__(self):
        super().__init__(escape=True); self.ids={}
    def heading(self,text,level,**attrs):
        plain=re.sub('<[^>]+>','',text); base=slugify(plain); n=self.ids.get(base,0)+1; self.ids[base]=n
        ident=base if n==1 else f'{base}-{n}'
        return f'<h{level} id="{ident}">{text}</h{level}>\n'
    def block_code(self,code,info=None):
        lang=(info or '').strip().split()[0].lower() if info else ''
        try: lexer=get_lexer_by_name(ALIASES.get(lang,lang)) if lang and ALIASES.get(lang,lang)!='text' else TextLexer()
        except Exception: lexer=TextLexer()
        return highlight(code,lexer,HtmlFormatter(cssclass='highlight'))

PYG=HtmlFormatter().get_style_defs('.highlight')
CSS=r'''@page{size:A4;margin:18mm 16mm 20mm 16mm;@top-left{content:"Curso Entity Framework Core 8 - Módulo 1";font-family:"DejaVu Sans";font-size:7.5pt;color:#5b6470}@top-right{content:"AceriaData";font-family:"DejaVu Sans";font-size:7.5pt;color:#5b6470}@bottom-center{content:"Página " counter(page) " de " counter(pages);font-family:"DejaVu Sans";font-size:7.5pt;color:#667085}}@page cover{margin:0;@top-left{content:none}@top-right{content:none}@bottom-center{content:none}}*{box-sizing:border-box}body{font-family:"DejaVu Sans",Arial,sans-serif;color:#182230;font-size:9.35pt;line-height:1.48;margin:0}.cover{page:cover;height:297mm;padding:38mm 28mm;display:flex;flex-direction:column;justify-content:center;background:linear-gradient(145deg,#f4f7fb 0%,#fff 55%,#edf2f7 100%)}.cover .kicker{font-size:11pt;letter-spacing:1.2px;text-transform:uppercase;color:#475467;margin-bottom:12mm}.cover h1{font-size:29pt;line-height:1.08;margin:0 0 7mm;color:#101828;border:none}.cover h2{font-size:18pt;line-height:1.2;margin:0 0 18mm;color:#344054;border:none}.cover .meta{border-top:1px solid #98a2b3;padding-top:8mm;color:#475467;font-size:10pt;line-height:1.7}.toc{break-before:page;break-after:page}.toc h1{font-size:22pt;margin-top:0}.toc ol{padding-left:0;list-style:none}.toc li{display:flex;gap:6px;margin:0 0 5.5mm;border-bottom:1px dotted #d0d5dd;padding-bottom:2mm}.toc a{color:#344054;text-decoration:none;width:100%}.toc a::after{content:leader('.') target-counter(attr(href),page);float:right;color:#667085}h1{font-size:23pt;line-height:1.15;color:#101828;margin:0 0 8mm;padding-bottom:4mm;border-bottom:2px solid #344054}h2{font-size:17pt;line-height:1.22;color:#101828;margin:0 0 7mm;padding-top:1mm;break-before:page}h3{font-size:12.5pt;line-height:1.3;color:#344054;margin:6mm 0 2.5mm;break-after:avoid}h4{font-size:10.8pt;color:#475467;margin:4mm 0 2mm;break-after:avoid}p{margin:0 0 3.2mm;orphans:3;widows:3}ul,ol{margin:0 0 3.5mm 6mm;padding-left:5mm}li{margin:0 0 1.4mm}strong{color:#101828}code{font-family:"DejaVu Sans Mono",Consolas,monospace;font-size:8.2pt;background:#f2f4f7;padding:.3mm .7mm;border-radius:2px}.highlight{font-family:"DejaVu Sans Mono",Consolas,monospace;font-size:7.45pt;line-height:1.38;background:#f8fafc;border:1px solid #d0d5dd;border-left:3px solid #667085;padding:3mm 3.2mm;margin:3mm 0 4mm;white-space:pre-wrap;overflow-wrap:anywhere;word-break:break-word}.highlight pre{white-space:pre-wrap;margin:0}blockquote{margin:4mm 0;padding:3mm 4mm;border-left:3px solid #667085;background:#f8fafc;color:#344054}table{width:100%;border-collapse:collapse;margin:4mm 0 5mm;font-size:8.4pt}thead{display:table-header-group}th{background:#eef2f6;color:#101828;font-weight:700}th,td{border:.5pt solid #cfd4dc;padding:2mm;vertical-align:top;overflow-wrap:anywhere}tr{break-inside:avoid}hr{border:0;border-top:1px solid #d0d5dd;margin:7mm 0}a{color:#175cd3}'''

def toc(md):
    out=[]
    for line in md.splitlines():
        if line.startswith('## Punto 1.'):
            title=line[3:].strip(); out.append((slugify(title),title))
    return out

def build(src,dst,kind,min_pages):
    md=src.read_text(encoding='utf-8')
    for forbidden in ('The user says','Let me develop','Esperando confirmación para continuar','&#x20;','&nbsp;'):
        if forbidden in md: raise RuntimeError(f'Metacontenido no permitido: {forbidden}')
    renderer=Renderer(); parser=mistune.create_markdown(renderer=renderer,plugins=['table','strikethrough'],hard_wrap=True)
    body=parser(md)
    items=''.join(f'<li><a href="#{i}">{html.escape(t)}</a></li>' for i,t in toc(md))
    doc=f'''<!doctype html><html><head><meta charset="utf-8"><title>M01 {kind}</title><style>{CSS}\n{PYG}</style></head><body><div class="cover"><div class="kicker">Curso profesional · .NET 8 · Entity Framework Core 8</div><h1>Módulo 1</h1><h2>Fundamentos de Entity Framework Core — {kind}</h2><div class="meta">Proyecto acumulativo: <strong>AceriaData</strong><br>Entorno principal: Visual Studio Community · SQL Server LocalDB<br>Contenido: puntos 1.1 a 1.12</div></div><section class="toc"><h1>Índice</h1><ol>{items}</ol></section>{body}</body></html>'''
    HTML(string=doc,base_url=str(src.parent)).write_pdf(str(dst))
    pages=len(PdfReader(str(dst)).pages)
    if pages < min_pages: raise RuntimeError(f'{dst.name}: PDF inesperadamente corto ({pages} páginas)')
    return pages

th=build(ROOT/'TEORIA'/'M01_TEORIA.md',ROOT/'TEORIA'/'M01_TEORIA.pdf','Teoría',60)
pr=build(ROOT/'PRACTICA'/'M01_PRACTICA.md',ROOT/'PRACTICA'/'M01_PRACTICA.pdf','Práctica',90)
print(f'PDF OK: teoría={th} páginas, práctica={pr} páginas')
