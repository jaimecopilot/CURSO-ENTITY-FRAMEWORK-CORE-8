from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
M4 = ROOT / "M04"
THEORY = M4 / "TEORIA" / "M04_TEORIA.md"
PRACTICE = M4 / "PRACTICA" / "M04_PRACTICA.md"
TRACE = M4 / "TRAZABILIDAD_FUENTE_M04.md"
BT = chr(96)

if not THEORY.is_file() or not PRACTICE.is_file() or not TRACE.is_file():
    raise RuntimeError("Faltan Markdown definitivos/trazabilidad de M4")

theory = THEORY.read_text(encoding="utf-8")
practice = PRACTICE.read_text(encoding="utf-8")

USES = {
    1: ("AnalisisSqlUseCase.cs", "Rendimiento41.cs", ["src/AceriaData.Infrastructure/DependencyInjection.cs"]),
    2: ("TrackingUseCase.cs", "Rendimiento42.cs", []),
    3: ("IdentityResolutionUseCase.cs", "Rendimiento43.cs", []),
    4: ("NMasUnoUseCase.cs", "Rendimiento44.cs", ["src/AceriaData.Infrastructure/SqlCommandCounterInterceptor.cs"]),
    5: ("SolucionesNMasUnoUseCase.cs", "Rendimiento45.cs", []),
    6: ("OverFetchingUseCase.cs", "Rendimiento46.cs", []),
    7: ("TraduccionConsultasUseCase.cs", "Rendimiento47.cs", []),
    8: ("SplitQueriesUseCase.cs", "Rendimiento48.cs", []),
    9: ("CompiledQueriesUseCase.cs", "Rendimiento49.cs", []),
    10: ("PaginacionUseCase.cs", "Rendimiento410.cs", ["src/AceriaData.Console/DemoData.cs"]),
    11: ("DiagnosticoRendimientoUseCase.cs", "Rendimiento411.cs", []),
    12: ("ChecklistRendimientoUseCase.cs", "Rendimiento412.cs", []),
}

def point_section(markdown: str, n: int) -> str:
    start = markdown.index(f"## Punto 4.{n} ")
    end = markdown.index(f"## Punto 4.{n+1} ", start) if n < 12 else len(markdown)
    return markdown[start:end]

def assert_no_escaped_code(label: str, markdown: str) -> None:
    in_fence = False
    suspicious = []
    for line_no, raw in enumerate(markdown.splitlines(), 1):
        stripped = raw.strip()
        if stripped.startswith(BT * 3):
            in_fence = not in_fence
            continue
        if in_fence:
            continue
        if (
            re.match(r"^(?:SELECT|FROM|INNER JOIN|LEFT JOIN|WHERE|ORDER BY)\b", stripped, re.I)
            or re.match(r"^(?:var|return|public|private|protected|using|namespace)\s", stripped)
            or re.match(r"^\.(?:Where|Select|Include|ThenInclude|OrderBy|ThenBy|Skip|Take|ToList|AsNoTracking)", stripped)
        ):
            suspicious.append(f"{line_no}: {raw}")
    if in_fence:
        raise RuntimeError(f"{label}: bloque Markdown sin cierre")
    if suspicious:
        raise RuntimeError(
            f"{label}: código fuera de bloque Markdown: " + " | ".join(suspicious[:12])
        )

def assert_no_prose_inside_code(label: str, markdown: str) -> None:
    in_fence = False
    language = ""
    suspicious = []
    for line_no, raw in enumerate(markdown.splitlines(), 1):
        stripped = raw.strip()
        if stripped.startswith(BT * 3):
            if in_fence:
                in_fence = False
                language = ""
            else:
                in_fence = True
                language = stripped[3:].strip().lower()
            continue
        if (
            in_fence
            and language in {"sql", "csharp", "bash", "powershell"}
            and re.match(r"^(?:La|El|Las|Los|Esta|Este|Estas|Estos)\b", stripped)
            and re.search(r"[.!?]$", stripped)
        ):
            suspicious.append(f"{line_no}: {raw}")
    if suspicious:
        raise RuntimeError(
            f"{label}: prosa explicativa dentro de bloque de código: "
            + " | ".join(suspicious[:12])
        )

def note_entries(section: str, label: str) -> dict[int, tuple[str, str]]:
    heading = f"#### Explicación línea a línea — {label}"
    start = section.find(heading)
    if start < 0:
        raise RuntimeError(f"Falta bloque línea a línea: {heading}")
    tail = section[start + len(heading):]
    next_heading = re.search(r"(?m)^#{3,4}\s+", tail)
    block = tail[:next_heading.start()] if next_heading else tail
    rx = re.compile(
        r"(?m)^Línea (\d+): " + re.escape(BT) + r"([^\n]*)"
        + re.escape(BT) + r" → (.+)$"
    )
    entries = {}
    for m in rx.finditer(block):
        num = int(m.group(1))
        if num in entries:
            raise RuntimeError(f"{label}: línea {num} explicada más de una vez")
        entries[num] = (m.group(2), m.group(3).strip())
    return entries

def assert_exact_line_notes(point: str, code: str, section: str, label: str) -> None:
    entries = note_entries(section, label)
    expected = {
        i: line.replace(BT, "´")
        for i, line in enumerate(code.splitlines(), 1)
        if line.strip()
    }
    if set(entries) != set(expected):
        missing = sorted(set(expected) - set(entries))
        extra = sorted(set(entries) - set(expected))
        raise RuntimeError(
            f"{point}/{label}: trazabilidad línea a línea incompleta; "
            f"faltan={missing}, sobran={extra}"
        )
    for line_no, src in expected.items():
        shown, desc = entries[line_no]
        if shown != src:
            raise RuntimeError(
                f"{point}/{label}: línea {line_no} no coincide; "
                f"doc={shown!r}, real={src!r}"
            )
        if len(desc) < 24:
            raise RuntimeError(
                f"{point}/{label}: explicación demasiado breve en línea "
                f"{line_no}: {desc!r}"
            )

def interface_methods(text: str) -> set[str]:
    return set(re.findall(
        r"(?m)^\s*[^\n;{}]*?\b([A-ZÁÉÍÓÚÑ]\w*)\s*\([^;{}]*\)\s*;",
        text,
    ))

for n in range(1, 13):
    title = f"## Punto 4.{n} "
    if title not in theory:
        raise RuntimeError(f"TEORIA: falta encabezado 4.{n}")
    if title not in practice:
        raise RuntimeError(f"PRACTICA: falta encabezado 4.{n}")

for step in range(1, 11):
    count = practice.count(f"### Paso {step}:")
    if count != 12:
        raise RuntimeError(
            f"PRACTICA: Paso {step} aparece {count} veces; se esperaban 12"
        )

forbidden = (
    "Esperando confirmación",
    "&#x20;",
    "Los filtros no traducibles provocan que la consulta se ejecute en memoria.",
    "Cada consulta de una Split Query se ejecuta en una transacción separada.",
    "La consulta compilada reutiliza el SQL generado y el plan de ejecución.",
    "AsSplitQuery no se debe usar con una sola colección.",
)
for bad in forbidden:
    if bad.lower() in theory.lower() or bad.lower() in practice.lower():
        raise RuntimeError(
            f"Documentación contiene afirmación/metacontenido prohibido: {bad}"
        )

required_theory = {
    3: ("Aleacion", "misma clave"),
    4: ("Lazy Loading desactivado", "N+1"),
    7: ("EF Core 8", "AsEnumerable", "InvalidOperationException"),
    8: ("aislamiento", "varios comandos"),
    9: ("SQL Server", "cachea consultas"),
    10: ("FechaCreacion", "Id"),
    11: ("TagWith", "tracking"),
    12: ("descartadas", "SQL"),
}
for n, tokens in required_theory.items():
    block = point_section(theory, n)
    for token in tokens:
        if token.lower() not in block.lower():
            raise RuntimeError(f"TEORIA 4.{n}: falta evidencia corregida {token}")

previous_interface = interface_methods(
    (ROOT / "M03/PROYECTO/3.12/src/AceriaData.Application/Interfaces.cs")
    .read_text(encoding="utf-8")
)

for n, (use, repo, extra) in USES.items():
    d = M4 / "PROYECTO" / f"4.{n}"
    use_text = (
        d / "src/AceriaData.Application" / use
    ).read_text(encoding="utf-8").strip()
    repo_text = (
        d / "src/AceriaData.Infrastructure/Repositories" / repo
    ).read_text(encoding="utf-8").strip()
    program = (
        d / "src/AceriaData.Console/Program.cs"
    ).read_text(encoding="utf-8").strip()
    sec = point_section(practice, n)

    for label, code in (("usecase", use_text), ("repo", repo_text), ("program", program)):
        if code not in sec:
            raise RuntimeError(
                f"PRACTICA 4.{n}: el código real de {label} no aparece literal"
            )

    assert_exact_line_notes(f"4.{n}", repo_text, sec, repo)
    assert_exact_line_notes(f"4.{n}", use_text, sec, use)
    assert_exact_line_notes(f"4.{n}", program, sec, "Program.cs")

    for rel in extra:
        code = (d / rel).read_text(encoding="utf-8").strip()
        if code not in sec:
            raise RuntimeError(
                f"PRACTICA 4.{n}: falta archivo complementario literal {rel}"
            )
        if rel.endswith(".cs"):
            assert_exact_line_notes(f"4.{n}", code, sec, Path(rel).name)

    if f"4.{n} OK" not in sec:
        raise RuntimeError(f"PRACTICA 4.{n}: falta marcador E2E")

    current_interface = interface_methods(
        (d / "src/AceriaData.Application/Interfaces.cs")
        .read_text(encoding="utf-8")
    )
    added = current_interface - previous_interface
    removed = previous_interface - current_interface
    paso3 = sec[sec.index("### Paso 3:"):sec.index("### Paso 4:")]
    for method in sorted(added | removed):
        if BT + method + BT not in paso3:
            raise RuntimeError(
                f"PRACTICA 4.{n}: Paso 3 no documenta delta contractual {method}"
            )
    for method in sorted(added):
        if "." + method + "(" not in use_text:
            raise RuntimeError(
                f"PRACTICA 4.{n}: el caso de uso no ejerce el método nuevo {method}"
            )
    previous_interface = current_interface


# Cada cambio físico real debe aparecer nombrado en el Paso 3 del punto.
def file_map(root: Path) -> dict[str, bytes]:
    return {
        p.relative_to(root).as_posix(): p.read_bytes()
        for p in root.rglob("*")
        if p.is_file() and "bin" not in p.parts and "obj" not in p.parts
    }

prev_root = ROOT / "M03/PROYECTO/3.12"
prev_map = file_map(prev_root)
for n in range(1,13):
    cur_root = M4 / "PROYECTO" / f"4.{n}"
    cur_map = file_map(cur_root)
    added = sorted(set(cur_map) - set(prev_map))
    removed = sorted(set(prev_map) - set(cur_map))
    changed = sorted(
        p for p in set(cur_map) & set(prev_map)
        if cur_map[p] != prev_map[p]
    )
    sec = point_section(practice,n)
    paso3 = sec[sec.index("### Paso 3:"):sec.index("### Paso 4:")]
    for path in added + changed + removed:
        if BT + path + BT not in paso3:
            raise RuntimeError(
                f"PRACTICA 4.{n}: Paso 3 no nombra el cambio físico {path}"
            )
    prev_map = cur_map

assert_no_escaped_code("TEORIA M4", theory)
assert_no_escaped_code("PRACTICA M4", practice)
assert_no_prose_inside_code("TEORIA M4", theory)
assert_no_prose_inside_code("PRACTICA M4", practice)

generic = "Participa directamente en el flujo validado del checkpoint:"
all_notes = re.findall(r"(?m)^Línea \d+: .* → .+$", practice)
generic_notes = [x for x in all_notes if generic in x]
ratio = len(generic_notes) / max(1, len(all_notes))
if ratio > 0.08:
    raise RuntimeError(
        f"PRACTICA M4: demasiadas explicaciones genéricas "
        f"({len(generic_notes)}/{len(all_notes)} = {ratio:.1%}); máximo 8%"
    )

if len(theory) < 250000:
    raise RuntimeError(
        f"TEORIA demasiado breve tras restaurar ejemplos: {len(theory)} caracteres"
    )
if len(practice) < 210000:
    raise RuntimeError(
        f"PRACTICA demasiado breve tras ampliar trazabilidad: {len(practice)} caracteres"
    )

fence = BT * 3
for label, text_doc in (("TEORIA", theory), ("PRACTICA", practice)):
    if text_doc.count(fence) % 2:
        raise RuntimeError(f"{label}: bloque de código sin cerrar")

print(
    f"M4 DOC QA PASS: teoría={len(theory)} chars, "
    f"práctica={len(practice)} chars, líneas explicadas={len(all_notes)}, "
    f"genéricas={len(generic_notes)} ({ratio:.1%})"
)
