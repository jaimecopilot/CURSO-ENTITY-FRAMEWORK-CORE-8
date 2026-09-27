from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
PRACTICE = ROOT / "M01" / "PRACTICA" / "M01_PRACTICA.md"
TEXT = PRACTICE.read_text(encoding="utf-8")

def section(n):
    m = re.search(rf"^## Punto 1\.{n}\b.*$", TEXT, re.M)
    if not m:
        raise RuntimeError(f"No existe el punto 1.{n}")
    if n < 12:
        nxt = re.search(rf"^## Punto 1\.{n+1}\b.*$", TEXT[m.end():], re.M)
        end = m.end() + nxt.start() if nxt else len(TEXT)
    else:
        end = len(TEXT)
    return TEXT[m.start():end]

def fenced(sec):
    return re.findall(r"```([^\n]*)\n(.*?)```", sec, re.S)

def full_program(n):
    candidates = [
        code for lang, code in fenced(section(n))
        if lang.strip().lower() in ("csharp", "cs")
        and "public class Program" in code
        and "using Microsoft.EntityFrameworkCore;" in code
    ]
    if not candidates:
        raise RuntimeError(f"1.{n}: no existe Program.cs completo en la práctica")
    return max(candidates, key=len)

def challenge_cs(n):
    sec = section(n)
    h = re.search(r"^### Reto resuelto:.*$", sec, re.M)
    if not h:
        return []
    return [
        code for lang, code in fenced(sec[h.end():])
        if lang.strip().lower() in ("csharp", "cs")
    ]

def block_end(src, open_brace):
    depth = 0
    for i in range(open_brace, len(src)):
        if src[i] == "{":
            depth += 1
        elif src[i] == "}":
            depth -= 1
            if depth == 0:
                return i
    raise RuntimeError("Llaves desbalanceadas")

def inject_program_method_and_call(src, method, call):
    pc = src.index("public class Program")
    p_open = src.index("{", pc)
    p_end = block_end(src, p_open)
    indented = method.strip().replace("\n", "\n    ")
    src = src[:p_end] + "\n\n    " + indented + "\n" + src[p_end:]

    pc = src.index("public class Program")
    markers = ("public static void Main()", "public static async Task Main()")
    pos = next((src.find(x, pc) for x in markers if src.find(x, pc) >= 0), -1)
    if pos < 0:
        raise RuntimeError("No se encuentra Main")
    m_open = src.index("{", pos)
    m_end = block_end(src, m_open)
    call_indented = call.strip().replace("\n", "\n        ")
    return src[:m_end] + "\n        " + call_indented + "\n" + src[m_end:]

changed = []
for n in range(5, 11):
    src = full_program(n)
    if n in (5, 6, 7, 8, 9):
        reto = challenge_cs(n)
        if len(reto) < 2:
            raise RuntimeError(f"1.{n}: reto resuelto incompleto")
        src = inject_program_method_and_call(src, reto[0], reto[1])

    target = ROOT / "checkpoints" / f"M1-CP{n:02d}" / "Program.cs"
    current = target.read_text(encoding="utf-8") if target.exists() else ""
    normalized = src.rstrip() + "\n"
    if current != normalized:
        target.write_text(normalized, encoding="utf-8")
        changed.append(str(target.relative_to(ROOT)))

print("CHECKPOINT SYNC:", "sin cambios" if not changed else ", ".join(changed))
