from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
M4 = ROOT / "M04"
THEORY = M4 / "TEORIA" / "M04_TEORIA.md"
PRACTICE = M4 / "PRACTICA" / "M04_PRACTICA.md"
TRACE = M4 / "TRAZABILIDAD_FUENTE_M04.md"

if not THEORY.is_file() or not PRACTICE.is_file() or not TRACE.is_file():
    raise RuntimeError("Faltan Markdown definitivos/trazabilidad de M4")

theory = THEORY.read_text(encoding="utf-8")
practice = PRACTICE.read_text(encoding="utf-8")

for n in range(1, 13):
    title = f"## Punto 4.{n} "
    if title not in theory:
        raise RuntimeError(f"TEORIA: falta encabezado 4.{n}")
    if title not in practice:
        raise RuntimeError(f"PRACTICA: falta encabezado 4.{n}")

for step in range(1, 11):
    count = practice.count(f"### Paso {step}:")
    if count != 12:
        raise RuntimeError(f"PRACTICA: Paso {step} aparece {count} veces; se esperaban 12")

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
        raise RuntimeError(f"Documentación contiene afirmación/metacontenido prohibido: {bad}")

required_theory = {
    3: ("Aleacion", "misma clave"),
    4: ("Lazy Loading desactivado", "N+1"),
    7: ("EF Core 8", "AsEnumerable"),
    8: ("aislamiento", "varios comandos"),
    9: ("SQL Server", "cachea consultas"),
    10: ("FechaCreacion", "Id"),
    11: ("TagWith", "tracking"),
    12: ("descartadas", "SQL"),
}
for n, tokens in required_theory.items():
    block = theory.split(f"## Punto 4.{n} ", 1)[1]
    if n < 12:
        block = block.split(f"## Punto 4.{n+1} ", 1)[0]
    for token in tokens:
        if token.lower() not in block.lower():
            raise RuntimeError(f"TEORIA 4.{n}: falta evidencia corregida {token}")

uses = {
    1: ("AnalisisSqlUseCase.cs", "Rendimiento41.cs"),
    2: ("TrackingUseCase.cs", "Rendimiento42.cs"),
    3: ("IdentityResolutionUseCase.cs", "Rendimiento43.cs"),
    4: ("NMasUnoUseCase.cs", "Rendimiento44.cs"),
    5: ("SolucionesNMasUnoUseCase.cs", "Rendimiento45.cs"),
    6: ("OverFetchingUseCase.cs", "Rendimiento46.cs"),
    7: ("TraduccionConsultasUseCase.cs", "Rendimiento47.cs"),
    8: ("SplitQueriesUseCase.cs", "Rendimiento48.cs"),
    9: ("CompiledQueriesUseCase.cs", "Rendimiento49.cs"),
    10: ("PaginacionUseCase.cs", "Rendimiento410.cs"),
    11: ("DiagnosticoRendimientoUseCase.cs", "Rendimiento411.cs"),
    12: ("ChecklistRendimientoUseCase.cs", "Rendimiento412.cs"),
}
for n, (use, repo) in uses.items():
    d = M4 / "PROYECTO" / f"4.{n}"
    use_text = (d / "src" / "AceriaData.Application" / use).read_text(encoding="utf-8").strip()
    repo_text = (d / "src" / "AceriaData.Infrastructure" / "Repositories" / repo).read_text(encoding="utf-8").strip()
    program = (d / "src" / "AceriaData.Console" / "Program.cs").read_text(encoding="utf-8").strip()
    for label, code in (("usecase", use_text), ("repo", repo_text), ("program", program)):
        if code not in practice:
            raise RuntimeError(f"PRACTICA 4.{n}: el código real de {label} no aparece literal")
    if f"4.{n} OK" not in practice:
        raise RuntimeError(f"PRACTICA 4.{n}: falta marcador E2E")

if len(theory) < 165000:
    raise RuntimeError(f"TEORIA demasiado breve: {len(theory)} caracteres")
if len(practice) < 195000:
    raise RuntimeError(f"PRACTICA demasiado breve: {len(practice)} caracteres")

fence = chr(96) * 3
for label, text in (("TEORIA", theory), ("PRACTICA", practice)):
    if text.count(fence) % 2:
        raise RuntimeError(f"{label}: bloque de código sin cerrar")

print(f"M4 DOC QA PASS: teoría={len(theory)} chars, práctica={len(practice)} chars")
