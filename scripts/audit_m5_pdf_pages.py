from pathlib import Path
import re
import fitz
ROOT=Path(__file__).resolve().parents[1] / 'M05'
DOCS=[('TEORIA',ROOT/'TEORIA'/'M05_TEORIA.pdf',50),('PRACTICA',ROOT/'PRACTICA'/'M05_PRACTICA.pdf',60)]
FORBIDDEN=('the user wants','we need to','let me think','esperando confirmación','material fuente','corrección técnica:','checkpoint','e2e','baseline','trazabil','checksum','sha256','qa interno','delta docente','inventario físico','source routes')
for label,path,minimum in DOCS:
    doc=fitz.open(path)
    if len(doc)<minimum: raise RuntimeError(f'{label}: PDF demasiado corto: {len(doc)} < {minimum}')
    joined=[]
    def meaningful(page):
        result=[]
        for block in page.get_text('blocks'):
            compact=re.sub(r'\s+',' ',str(block[4]).strip())
            if not compact: continue
            if compact.startswith('CURSO: Curso Profesional de Entity Framework Core 8'): continue
            if compact.startswith('AceriaData · Módulo 5'): continue
            if re.fullmatch(r'Página \d+ de \d+',compact): continue
            result.append(block)
        return result
    for idx,page in enumerate(doc,1):
        text=page.get_text('text'); joined.append(text); body=meaningful(page)
        if len(re.sub(r'\s+','','\n'.join(str(b[4]) for b in body)))<20: raise RuntimeError(f'{label}: página {idx} sin contenido útil')
        if 'JAIME GALLO' not in text: raise RuntimeError(f'{label}: falta autor en página {idx}')
        low=text.lower()
        if any(x in low for x in FORBIDDEN): raise RuntimeError(f'{label}: metacontenido en página {idx}')
        for block in page.get_text('blocks'):
            x0,y0,x1,y1,*_=block
            if x0 < -2 or y0 < -2 or x1 > page.rect.width+2 or y1 > page.rect.height+2:
                raise RuntimeError(f'{label}: bloque fuera de página {idx}')
    body=meaningful(doc[-1]); bottom=max(b[3] for b in body) if body else 0
    if bottom<180: raise RuntimeError(f'{label}: última página huérfana; y={bottom:.1f}')
    full='\n'.join(joined)
    for n in range(1,13):
        if f'Punto 5.{n}' not in full: raise RuntimeError(f'{label}: falta Punto 5.{n}')
    print(f'PDF PAGE QA PASS {label}: {len(doc)} páginas')
print('PDF PAGE QA PASS: teoría y práctica M5 verificadas página por página.')
