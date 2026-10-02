from pathlib import Path
import re, html, unicodedata
import mistune
from bs4 import BeautifulSoup
from pygments import highlight
from pygments.lexers import get_lexer_by_name, TextLexer
from pygments.formatters import HtmlFormatter
from weasyprint import HTML
from pypdf import PdfReader
ROOT=Path(__file__).resolve().parents[1] / 'M05'
ALIASES={'csharp':'csharp','cs':'csharp','c#':'csharp','bash':'bash','sh':'bash','powershell':'powershell','ps1':'powershell','sql':'sql','json':'json','xml':'xml','text':'text','plaintext':'text'}
def slug(s):
    s=unicodedata.normalize('NFKD',s).encode('ascii','ignore').decode('ascii')
    return re.sub(r'[^a-zA-Z0-9]+','-',s).strip('-').lower() or 'seccion'
class Renderer(mistune.HTMLRenderer):
    def __init__(self): super().__init__(escape=True); self.ids={}
    def heading(self,text,level,**attrs):
        plain=re.sub('<[^>]+>','',text); base=slug(plain); n=self.ids.get(base,0)+1; self.ids[base]=n; ident=base if n==1 else f'{base}-{n}'
        cls=''
        if level==3:
            low=plain.lower()
            if low.startswith('objetivos de aprendizaje'): cls=' class="section-title"'
            elif low.startswith('resumen de la teoría'): cls=' class="final-summary-title"'
            elif low.startswith(('bloque ','paso ','reto resuelto','solución','resultado esperado','conclusión')): cls=' class="accent-title"'
        return f'<h{level} id="{ident}"{cls}>{text}</h{level}>\n'
    def block_code(self,code,info=None):
        lang=(info or '').strip().split()[0].lower() if info else ''; alias=ALIASES.get(lang,lang)
        try: lexer=get_lexer_by_name(alias) if alias and alias!='text' else TextLexer()
        except Exception: lexer=TextLexer()
        rendered=highlight(code,lexer,HtmlFormatter(cssclass='highlight'))
        return '<div class="code-block">'+f'<div class="code-lang">{html.escape((lang or "text").upper())}</div>'+rendered+'</div>'
PYG=HtmlFormatter(style='friendly').get_style_defs('.highlight')+'\n.highlight .err{border:none}'
CSS=r'''*{box-sizing:border-box}body{font-family:"DejaVu Sans",Arial,sans-serif;color:#26384d;font-size:8.55pt;line-height:1.32;margin:0}.cover{page:cover;position:relative;height:297mm;padding:44mm 16mm 24mm;background:#fff}.cover:before{content:"";position:absolute;top:0;left:0;right:0;height:7mm;background:#173f6b}.cover .course{margin:0 0 7mm;font-size:27pt;line-height:1.08;font-weight:700;color:#173f6b;max-width:170mm}.cover .module{margin:0 0 12mm;font-size:16.5pt;line-height:1.18;font-weight:700;color:#2c6693}.cover .badge{display:inline-block;background:#173f6b;color:#fff;font-weight:700;letter-spacing:1.4px;font-size:8.5pt;padding:3.2mm 7mm;border-radius:2.4mm;margin-bottom:14mm}.cover .author{font-size:8.7pt;font-weight:700;color:#365b7d;margin-bottom:5mm}.cover .meta{color:#667f98;font-size:8pt;line-height:1.45;max-width:158mm}.toc{break-before:page;break-after:page}.toc h1{color:#173f6b;font-size:21pt;border-bottom:1.2pt solid #173f6b;padding-bottom:3mm;margin:0 0 8mm}.toc ol{list-style:none;padding:0;margin:0}.toc li{margin:0 0 4mm;padding-bottom:1.5mm;border-bottom:.45pt dotted #bdcbd8}.toc a{color:#304760;text-decoration:none;width:100%}.toc a:after{content:leader('.') target-counter(attr(href),page);float:right;color:#6d8092}h1{color:#173f6b;font-size:20pt;line-height:1.14;margin:0 0 6mm}h2{color:#173f6b;font-size:15.4pt;line-height:1.18;margin:7mm 0 5mm;padding-bottom:2.2mm;border-bottom:.7pt solid #b7cfdf;break-after:avoid}h3{color:#173f6b;font-size:11.4pt;line-height:1.24;font-weight:700;margin:5.2mm 0 2.2mm;break-after:avoid}h3.section-title{border-bottom:.65pt solid #b7cfdf;padding-bottom:1.7mm}h3.accent-title{border-left:3.2pt solid #2f7dac;padding-left:2.5mm}h4{color:#345c7c;font-size:9.5pt;margin:4mm 0 1.8mm;break-after:avoid}p{margin:0 0 2.5mm;orphans:3;widows:3}ul,ol{margin:0 0 3mm 5mm;padding-left:5mm}li{margin:0 0 1.1mm}strong{color:#173f6b}a{color:#246a9a}code{font-family:"DejaVu Sans Mono",Consolas,monospace;font-size:7.7pt;background:#edf2f6;padding:.15mm .5mm;border-radius:1mm}blockquote{margin:3mm 0 4mm;padding:3mm 4mm;border-left:3pt solid #2f7dac;background:#eaf4fb;color:#34516b}.code-block{position:relative;margin:2.5mm 0 3.2mm;border:.55pt solid #d6dee7;border-radius:2mm;background:#f5f7f9;break-inside:auto;box-decoration-break:clone}.code-lang{position:absolute;top:1.3mm;right:2.2mm;font-size:5.4pt;font-weight:700;letter-spacing:.7px;color:#71869a;z-index:2}.highlight{font-family:"DejaVu Sans Mono",Consolas,monospace;font-size:6.95pt;line-height:1.28;background:transparent!important;padding:3.2mm 3mm 2.8mm;margin:0;white-space:pre-wrap;overflow-wrap:anywhere;word-break:break-word}.highlight pre{white-space:pre-wrap;margin:0}.line-row{display:table;width:100%;table-layout:fixed;border-left:.55pt solid #d6dee7;border-right:.55pt solid #d6dee7;border-bottom:.55pt solid #d6dee7;margin:0;break-inside:avoid}.line-row.first{border-top:.55pt solid #d6dee7;margin-top:2.2mm}.line-row.last{margin-bottom:3.2mm}.line-label{display:table-cell;width:24mm;padding:2.1mm 2.5mm;background:#eaf4fb;color:#205d86;font-weight:700;vertical-align:top}.line-desc{display:table-cell;padding:2.1mm 2.8mm;background:#fbfcfd;color:#334a60;vertical-align:top;overflow-wrap:anywhere}.line-code{font-family:"DejaVu Sans Mono",Consolas,monospace;font-size:7.65pt;color:#244c6b}.line-arrow{font-family:"DejaVu Sans",Arial,sans-serif;color:#334a60;padding:0 1.1mm}table{width:100%;border-collapse:collapse;margin:3mm 0 4mm;font-size:7.8pt}thead{display:table-header-group}th{background:#eaf4fb;color:#173f6b;font-weight:700}th,td{border:.5pt solid #d3dee7;padding:1.8mm 2mm;vertical-align:top;overflow-wrap:anywhere}tr{break-inside:avoid}hr{border:0;border-top:.7pt solid #c9d7e2;margin:5mm 0}.final-point p{margin-bottom:2.15mm}.final-point ul,.final-point ol{margin-bottom:2.5mm}.final-point li{margin-bottom:.85mm}.final-point h2{margin-top:6.4mm;margin-bottom:4.4mm}.final-point h3{margin-top:4.7mm;margin-bottom:1.9mm}.final-point h4{margin-top:3.5mm;margin-bottom:1.5mm}.final-point .code-block{margin-top:2.1mm;margin-bottom:2.7mm}.final-point .highlight{padding-top:2.8mm;padding-bottom:2.45mm;line-height:1.25}.final-point .line-label,.final-point .line-desc{padding-top:1.75mm;padding-bottom:1.75mm}.final-point .line-row.first{margin-top:1.9mm}.final-point .line-row.last{margin-bottom:2.6mm}.final-point table{margin-top:2.5mm;margin-bottom:3.2mm}'''
def toc(md): return [(slug(x[3:].strip()),x[3:].strip()) for x in md.splitlines() if x.startswith('## Punto 5.')]
def line_rows(body):
    soup=BeautifulSoup(body,'html.parser'); rx=re.compile(r'^(Línea(?:s)?\s+[^:→]+)(?::|\s+→)\s*(.*)$',re.S); rows=[]
    for p in list(soup.find_all('p')):
        txt=' '.join(p.get_text(' ',strip=True).split()); m=rx.match(txt)
        if not m:
            if rows: rows[-1]['class']=rows[-1].get('class',[])+['last']; rows=[]
            continue
        row=soup.new_tag('div'); row['class']=['line-row']+(['first'] if not rows else [])
        left=soup.new_tag('div'); left['class']=['line-label']; left.string=m.group(1).strip()
        right=soup.new_tag('div'); right['class']=['line-desc']; payload=m.group(2).strip()
        if '→' in payload:
            code_text,desc_text=payload.split('→',1); cs=soup.new_tag('span'); cs['class']=['line-code']; cs.string=code_text.strip(); ar=soup.new_tag('span'); ar['class']=['line-arrow']; ar.string='→'; ds=soup.new_tag('span'); ds.string=desc_text.strip(); right.extend([cs,ar,ds])
        else: right.string=payload
        row.extend([left,right]); p.replace_with(row); rows.append(row)
    if rows: rows[-1]['class']=rows[-1].get('class',[])+['last']
    return str(soup)
src=ROOT/'TEORIA'/'M05_TEORIA.md'; dst=ROOT/'TEORIA'/'M05_TEORIA.pdf'; md=src.read_text(encoding='utf-8')
renderer=Renderer(); parser=mistune.create_markdown(renderer=renderer,plugins=['table','strikethrough'],hard_wrap=True); body=line_rows(parser(md))
soup=BeautifulSoup(body,'html.parser'); final_heading=soup.find('h2',id=re.compile(r'^punto-5-12(?:-|$)')); section=soup.new_tag('section'); section['class']=['final-point']; final_heading.insert_before(section); node=final_heading
while node is not None: nxt=node.next_sibling; section.append(node.extract()); node=nxt
body=str(soup); items=''.join(f'<li><a href="#{ident}">{html.escape(title)}</a></li>' for ident,title in toc(md)); header='CURSO: Curso Profesional de Entity Framework Core 8 · MÓDULO 5. Persistencia empresarial - TEORÍA · AUTOR: JAIME GALLO'; footer='AceriaData · Módulo 5 · Teoría'; page_css=f'''@page{{size:A4;margin:15mm 15mm 17mm 15mm;@top-center{{content:"{header}";font-family:"DejaVu Sans";font-size:5.6pt;color:#54718b}}@bottom-left{{content:"{footer}";font-family:"DejaVu Sans";font-size:5.6pt;color:#8a9bab}}@bottom-right{{content:"Página " counter(page) " de " counter(pages);font-family:"DejaVu Sans";font-size:5.6pt;color:#8a9bab}}}}@page cover{{size:A4;margin:0;@top-center{{content:none}}@bottom-left{{content:none}}@bottom-right{{content:none}}}}'''
doc=f'''<!doctype html><html><head><meta charset="utf-8"><title>M05 Teoría</title><style>{page_css}\n{CSS}\n{PYG}</style></head><body class="theory-doc"><section class="cover"><div class="course">Curso Profesional de Entity Framework Core 8</div><div class="module">Módulo 5 - Persistencia empresarial</div><div class="badge">TEORÍA</div><div class="author">AUTOR: JAIME GALLO</div><div class="meta">.NET 8 · Entity Framework Core 8 · Visual Studio Community · SQL Server LocalDB · Proyecto AceriaData</div></section><section class="toc"><h1>Índice</h1><ol>{items}</ol></section>{body}</body></html>'''
HTML(string=doc,base_url=str(src.parent)).write_pdf(str(dst)); pages=len(PdfReader(str(dst)).pages); print('pages',pages)
